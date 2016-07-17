using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using System.Runtime.InteropServices;

namespace Org.BouncyCastle.Crypto.Signers
{
	/**
	 * EC-DSA as described in X9.62
	 */
	public class ECDsaSigner
		: IDsa
	{
		private ECKeyParameters key;
		private SecureRandom random;

		public string AlgorithmName
		{
			get { return "ECDSA"; }
		}

		public void Init(
			bool				forSigning,
			ICipherParameters	parameters)
		{
			if (forSigning)
			{
				if (parameters is ParametersWithRandom)
				{
					ParametersWithRandom rParam = (ParametersWithRandom) parameters;

					this.random = rParam.Random;
					parameters = rParam.Parameters;
				}
				else
				{
					this.random = new SecureRandom();
				}

				if (!(parameters is ECPrivateKeyParameters))
					throw new InvalidKeyException("EC private key required for signing");

				this.key = (ECPrivateKeyParameters) parameters;
			}
			else
			{
				if (!(parameters is ECPublicKeyParameters))
					throw new InvalidKeyException("EC public key required for verification");

				this.key = (ECPublicKeyParameters) parameters;
			}
		}

		// 5.3 pg 28
		/**
		 * Generate a signature for the given message using the key we were
		 * initialised with. For conventional DSA the message should be a SHA-1
		 * hash of the message of interest.
		 *
		 * @param message the message that will be verified later.
		 */
		public BigInteger[] GenerateSignature(
			byte[] message)
		{
			BigInteger n = key.Parameters.N;
			BigInteger e = calculateE(n, message);

            string messageStr = "";
            for (int i = 0; i < message.Length; i++)
            {
                messageStr += (char)message[i];
            }

			BigInteger r = null;
			BigInteger s = null;

			// 5.3.2
			do // Generate s
			{
				BigInteger k = null;

				do // Generate r
				{
					do
					{
						k = new BigInteger(n.BitLength, random);
					}
					while (k.SignValue == 0);

					ECPoint p = key.Parameters.G.Multiply(k);

					// 5.3.3
					BigInteger x = p.X.ToBigInteger();

					r = x.Mod(n);
				}
				while (r.SignValue == 0);

				BigInteger d = ((ECPrivateKeyParameters)key).D;

				s = k.ModInverse(n).Multiply(e.Add(d.Multiply(r))).Mod(n);
			}
			while (s.SignValue == 0);

			return new BigInteger[]{ r, s };
		}

        public BigInteger[] GenerateSignatureExternal(byte[] message)
        {
            var messageBigInt = new BigInteger(message);

            var firmaR = generar_Firma_ECDSA(messageBigInt.ToString(), ((FpCurve)key.Parameters.curve).Q.ToString(), key.Parameters.Curve.A.ToBigInteger().ToString(),
                key.Parameters.G.x.ToBigInteger().ToString(), key.Parameters.G.y.ToBigInteger().ToString(), key.Parameters.n.ToString(),
                ((ECPrivateKeyParameters)key).D.ToString());

            var firmaS = getFirmaS();

            return new BigInteger[] { new BigInteger(firmaR), new BigInteger(firmaS) };
        }

		// 5.4 pg 29    
		/**
		 * return true if the value r and s represent a DSA signature for
		 * the passed in message (for standard DSA the message should be
		 * a SHA-1 hash of the real message to be verified).
		 */
		public bool VerifySignature(
			byte[]		message,
			BigInteger	r,
			BigInteger	s)
		{
			BigInteger n = key.Parameters.N;

			// r and s should both in the range [1,n-1]
			if (r.SignValue < 1 || s.SignValue < 1
				|| r.CompareTo(n) >= 0 || s.CompareTo(n) >= 0)
			{
				return false;
			}

			BigInteger e = calculateE(n, message);
			BigInteger c = s.ModInverse(n);

			BigInteger u1 = e.Multiply(c).Mod(n);
			BigInteger u2 = r.Multiply(c).Mod(n);

			ECPoint G = key.Parameters.G;
			ECPoint Q = ((ECPublicKeyParameters) key).Q;

			ECPoint point = ECAlgorithms.SumOfTwoMultiplies(G, u1, Q, u2);

			BigInteger v = point.X.ToBigInteger().Mod(n);

			return v.Equals(r);
		}

        public bool VerifySignatureExternal(
            byte[] message,
            BigInteger r,
            BigInteger s)
        {
            var messageBigInt = new BigInteger(message);
            return verificar_firma_ECDSA(messageBigInt.ToString(), ((FpCurve)key.Parameters.curve).Q.ToString(), key.Parameters.curve.a.ToBigInteger().ToString(),
                key.Parameters.g.x.ToBigInteger().ToString(), key.Parameters.g.y.ToBigInteger().ToString(), key.Parameters.n.ToString(),
                ((ECPublicKeyParameters)key).Q.x.ToBigInteger().ToString(), ((ECPublicKeyParameters)key).Q.y.ToBigInteger().ToString(), r.ToString(), s.ToString());
        }

		private BigInteger calculateE(BigInteger n, byte[]	message)
		{
			int messageBitLength = message.Length * 8;
            BigInteger trunc = new BigInteger(1, message);

			if (n.BitLength < messageBitLength)
			{
				trunc = trunc.ShiftRight(messageBitLength - n.BitLength);
			}

			return trunc;
		}

        [DllImport(@"genCertificados.dll", EntryPoint = "generar_firma_ECDSA", CallingConvention = CallingConvention.StdCall)]
        public static extern string generar_Firma_ECDSA(string messageBigIntegerStr, string N, string A, string xG, string yG, string ordenCurva, string privateKey);

        [DllImport(@"genCertificados.dll", EntryPoint = "getFirmaS", CallingConvention = CallingConvention.StdCall)]
        public static extern string getFirmaS();

        [DllImport(@"genCertificados.dll", EntryPoint = "verificar_firma_ECDSA", CallingConvention = CallingConvention.StdCall)]
        public static extern bool verificar_firma_ECDSA(string messageBigIntegerStr, string N, string A, string xG, string yG, string ordenCurva, string pubX, string pubY, string r, string s);
	}
}
