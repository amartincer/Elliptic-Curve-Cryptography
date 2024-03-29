using System;
using System.IO;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Cms.Ecc;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Cms
{
	/**
	* the RecipientInfo class for a recipient who has been sent a message
	* encrypted using key agreement.
	*/
	public class KeyAgreeRecipientInformation
		: RecipientInformation
	{
		private KeyAgreeRecipientInfo info;
		private Asn1OctetString       encryptedKey;

		[Obsolete]
		public KeyAgreeRecipientInformation(
			KeyAgreeRecipientInfo	info,
			AlgorithmIdentifier		encAlg,
			Stream					data)
			: this(info, encAlg, null, null, data)
		{
		}

		[Obsolete]
		public KeyAgreeRecipientInformation(
			KeyAgreeRecipientInfo	info,
			AlgorithmIdentifier		encAlg,
			AlgorithmIdentifier		macAlg,
			Stream					data)
			: this(info, encAlg, macAlg, null, data)
		{
		}

		public KeyAgreeRecipientInformation(
			KeyAgreeRecipientInfo	info,
			AlgorithmIdentifier		encAlg,
			AlgorithmIdentifier		macAlg,
			AlgorithmIdentifier		authEncAlg,
			Stream					data)
			: base(encAlg, macAlg, authEncAlg, info.KeyEncryptionAlgorithm, data)
		{
			this.info = info;
			this.rid = new RecipientID();

			try
			{
				Asn1Sequence s = info.RecipientEncryptedKeys;
				RecipientEncryptedKey id = RecipientEncryptedKey.GetInstance(s[0]);

				Asn1.Cms.KeyAgreeRecipientIdentifier karid = id.Identifier;

				Asn1.Cms.IssuerAndSerialNumber iAndSN = karid.IssuerAndSerialNumber;
				if (iAndSN != null)
				{
					rid.Issuer = iAndSN.Name;
                    rid.SerialNumber = iAndSN.SerialNumber.Value;
				}
				else
				{
					Asn1.Cms.RecipientKeyIdentifier rKeyID = karid.RKeyID;

					// Note: 'date' and 'other' fields of RecipientKeyIdentifier appear to be only informational 

					rid.SubjectKeyIdentifier = rKeyID.SubjectKeyIdentifier.GetOctets();
				}

				encryptedKey = id.EncryptedKey;
			}
			catch (IOException e)
			{
				throw new ArgumentException("invalid rid in KeyAgreeRecipientInformation", e);
			}
		}

		private AsymmetricKeyParameter GetSenderPublicKey(
			AsymmetricKeyParameter		receiverPrivateKey,
			OriginatorIdentifierOrKey	originator)
		{
			OriginatorPublicKey opk = originator.OriginatorPublicKey;
			if (opk != null)
			{
				return GetPublicKeyFromOriginatorPublicKey(receiverPrivateKey, opk);
			}
			
			OriginatorID origID = new OriginatorID();
			
			Asn1.Cms.IssuerAndSerialNumber iAndSN = originator.IssuerAndSerialNumber;
			if (iAndSN != null)
			{
				origID.Issuer = iAndSN.Name;
				origID.SerialNumber = iAndSN.SerialNumber.Value;
			}
			else
			{
				SubjectKeyIdentifier ski = originator.SubjectKeyIdentifier;

				origID.SubjectKeyIdentifier = ski.GetKeyIdentifier();
			}

			return GetPublicKeyFromOriginatorID(origID);
		}

		private AsymmetricKeyParameter GetPublicKeyFromOriginatorPublicKey(
			AsymmetricKeyParameter	receiverPrivateKey,
			OriginatorPublicKey		originatorPublicKey)
		{
			PrivateKeyInfo privInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(receiverPrivateKey);
			SubjectPublicKeyInfo pubInfo = new SubjectPublicKeyInfo(
				privInfo.AlgorithmID,
				originatorPublicKey.PublicKey.GetBytes());
			return PublicKeyFactory.CreateKey(pubInfo);
		}

		private AsymmetricKeyParameter GetPublicKeyFromOriginatorID(
			OriginatorID origID)
		{
			// TODO Support all alternatives for OriginatorIdentifierOrKey
			// see RFC 3852 6.2.2
			throw new CmsException("No support for 'originator' as IssuerAndSerialNumber or SubjectKeyIdentifier");
		}

		private KeyParameter CalculateAgreedWrapKey(
			string					wrapAlg,
			AsymmetricKeyParameter	senderPublicKey,
			AsymmetricKeyParameter	receiverPrivateKey)
		{
			DerObjectIdentifier agreeAlgID = keyEncAlg.ObjectID;

			ICipherParameters senderPublicParams = senderPublicKey;
			ICipherParameters receiverPrivateParams = receiverPrivateKey;

			if (agreeAlgID.Id.Equals(CmsEnvelopedGenerator.ECMqvSha1Kdf))
			{
				byte[] ukmEncoding = info.UserKeyingMaterial.GetOctets();
				MQVuserKeyingMaterial ukm = MQVuserKeyingMaterial.GetInstance(
					Asn1Object.FromByteArray(ukmEncoding));

				AsymmetricKeyParameter ephemeralKey = GetPublicKeyFromOriginatorPublicKey(
					receiverPrivateKey, ukm.EphemeralPublicKey);

				senderPublicParams = new MqvPublicParameters(
					(ECPublicKeyParameters)senderPublicParams,
					(ECPublicKeyParameters)ephemeralKey);
				receiverPrivateParams = new MqvPrivateParameters(
					(ECPrivateKeyParameters)receiverPrivateParams,
					(ECPrivateKeyParameters)receiverPrivateParams);
			}

			IBasicAgreement agreement = AgreementUtilities.GetBasicAgreementWithKdf(
				agreeAlgID, wrapAlg);
			agreement.Init(receiverPrivateParams);
			BigInteger agreedValue = agreement.CalculateAgreement(senderPublicParams);

			int wrapKeySize = GeneratorUtilities.GetDefaultKeySize(wrapAlg) / 8;
			byte[] wrapKeyBytes = X9IntegerConverter.IntegerToBytes(agreedValue, wrapKeySize);
			return ParameterUtilities.CreateKeyParameter(wrapAlg, wrapKeyBytes);
		}

		private KeyParameter UnwrapSessionKey(
			string			wrapAlg,
			KeyParameter	agreedKey)
		{
			AlgorithmIdentifier aid = GetActiveAlgID();
			string alg = aid.ObjectID.Id;
			byte[] encKeyOctets = encryptedKey.GetOctets();

			IWrapper keyCipher = WrapperUtilities.GetWrapper(wrapAlg);
			keyCipher.Init(false, agreedKey);
			byte[] sKeyBytes = keyCipher.Unwrap(encKeyOctets, 0, encKeyOctets.Length);
			return ParameterUtilities.CreateKeyParameter(alg, sKeyBytes);
		}

		internal KeyParameter GetSessionKey(
			AsymmetricKeyParameter receiverPrivateKey)
		{
			try
			{
				string wrapAlg = DerObjectIdentifier.GetInstance(
					Asn1Sequence.GetInstance(keyEncAlg.Parameters)[0]).Id;

				AsymmetricKeyParameter senderPublicKey = GetSenderPublicKey(
					receiverPrivateKey, info.Originator);

				KeyParameter agreedWrapKey = CalculateAgreedWrapKey(wrapAlg,
					senderPublicKey, receiverPrivateKey);

				return UnwrapSessionKey(wrapAlg, agreedWrapKey);
			}
			catch (SecurityUtilityException e)
			{
				throw new CmsException("couldn't create cipher.", e);
			}
			catch (InvalidKeyException e)
			{
				throw new CmsException("key invalid in message.", e);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.StackTrace);
				throw new CmsException("originator key invalid.", e);
			}
		}

		/**
		* decrypt the content and return an input stream.
		*/
		public override CmsTypedStream GetContentStream(
			ICipherParameters key)
		{
			if (!(key is AsymmetricKeyParameter))
				throw new ArgumentException("KeyAgreement requires asymmetric key", "key");

			AsymmetricKeyParameter receiverPrivateKey = (AsymmetricKeyParameter) key;

			if (!receiverPrivateKey.IsPrivate)
				throw new ArgumentException("Expected private key", "key");

			KeyParameter sKey = GetSessionKey(receiverPrivateKey);

			return GetContentFromSessionKey(sKey);
		}
	}
}
