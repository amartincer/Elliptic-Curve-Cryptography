using System;
using System.Globalization;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System.Runtime.InteropServices;
using System.IO;

namespace Org.BouncyCastle.Crypto.Generators
{
    public class ECKeyPairGenerator
		: IAsymmetricCipherKeyPairGenerator
    {
		private readonly string algorithm;

		private ECDomainParameters parameters;
		private DerObjectIdentifier publicKeyParamSet;
        private SecureRandom random;

		public ECKeyPairGenerator()
			: this("EC")
		{
		}

		public ECKeyPairGenerator(
			string algorithm)
		{
			if (algorithm == null)
				throw new ArgumentNullException("algorithm");

			this.algorithm = VerifyAlgorithmName(algorithm);
		}

        public void Init(KeyGenerationParameters parameters)
        {
            Init(parameters, null);
        }

		public void Init(KeyGenerationParameters parameters,string curveName)
        {
			if (parameters is ECKeyGenerationParameters)
			{
				ECKeyGenerationParameters ecP = (ECKeyGenerationParameters) parameters;

				this.publicKeyParamSet = ecP.PublicKeyParamSet;
				this.parameters = ecP.DomainParameters;
			}
			else
			{
                DerObjectIdentifier oid = null;
                X9ECParameters ecps = null;
                if (string.IsNullOrEmpty(curveName))
                {
                    switch (parameters.Strength)
                    {
                        case 112:
                            oid = SecObjectIdentifiers.SecP112r2;
                            break;
                        case 128:
                            oid = SecObjectIdentifiers.SecP128r2;
                            break;
                        case 160:
                            oid = SecObjectIdentifiers.SecP160k1;
                            break;
                        case 192:
                            oid = X9ObjectIdentifiers.Prime192v1;
                            break;
                        case 224:
                            oid = SecObjectIdentifiers.SecP224r1;
                            break;
                        case 239:
                            oid = X9ObjectIdentifiers.Prime239v1;
                            break;
                        case 256:
                            oid = X9ObjectIdentifiers.Prime256v1;
                            break;
                        case 384:
                            oid = SecObjectIdentifiers.SecP384r1;
                            break;
                        case 521:
                            oid = SecObjectIdentifiers.SecP521r1;
                            break;
                        default:
                            throw new InvalidParameterException("unknown key size.");
                    }
                    ecps = FindECCurveByOid(oid);
                }
                else
                {
                    oid = FindOIDByCurveName(curveName);
                    ecps = FindECCurveByName(curveName);
                }

				this.parameters = new ECDomainParameters(
					ecps.Curve, ecps.G, ecps.N, ecps.H, ecps.GetSeed());

                publicKeyParamSet = new DerObjectIdentifier(oid.ToString());
			}

			this.random = parameters.Random;
		}

		/**
         * Given the domain parameters this routine Generates an EC key
         * pair in accordance with X9.62 section 5.2.1 pages 26, 27.
         */
        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            BigInteger d = null;
            ECPoint q = null;

            BigInteger n = parameters.N;
            do
            {
                d = new BigInteger(n.BitLength, random);
            }
            while (d.SignValue == 0 || (d.CompareTo(n) >= 0));

            q = parameters.G.Multiply(d);

            if (publicKeyParamSet != null)
            {
                return new AsymmetricCipherKeyPair(
                    new ECPublicKeyParameters(algorithm, q, publicKeyParamSet),
                    new ECPrivateKeyParameters(algorithm, d, publicKeyParamSet));
            }
			return new AsymmetricCipherKeyPair(
				new ECPublicKeyParameters(algorithm, q, parameters),
				new ECPrivateKeyParameters(algorithm, d, parameters));
		}

        public AsymmetricCipherKeyPair GenerateKeyPairExternAlgorithm()
        {
            BigInteger privateKey = null;
            ECPoint publicKey = null;

            string N = ((FpCurve)parameters.curve).Q.ToString();
            string A = parameters.curve.A.ToBigInteger().ToString();
            string B = parameters.curve.B.ToBigInteger().ToString();
            string xG = parameters.g.x.ToBigInteger().ToString();
            string yG = parameters.g.y.ToBigInteger().ToString();
            string nG = parameters.n.ToString();
            string h = parameters.h.ToString();

            var privateKeyStr=getKeys(N, A, B, xG, yG, nG, h);
            var publicKeyXStr=getPublicX();
            var publicKeyYStr = getPublicY();
            var error = getError();

            BigInteger pX = new BigInteger(publicKeyXStr);
            BigInteger pY = new BigInteger(publicKeyYStr);
            
            publicKey = new FpPoint(parameters.curve, new FpFieldElement(((FpCurve)parameters.curve).Q, pX),
                                                   new FpFieldElement(((FpCurve)parameters.curve).Q, pY));
            privateKey = new BigInteger(privateKeyStr);
            
            if (publicKeyParamSet != null)
            {
                return new AsymmetricCipherKeyPair(
                    new ECPublicKeyParameters(algorithm, publicKey, publicKeyParamSet),
                    new ECPrivateKeyParameters(algorithm, privateKey, publicKeyParamSet));
            }
            return new AsymmetricCipherKeyPair(
                new ECPublicKeyParameters(algorithm, publicKey, parameters),
                new ECPrivateKeyParameters(algorithm, privateKey, parameters));
        }

        [DllImport("genCertificados.dll", EntryPoint = "getKeys", CallingConvention = CallingConvention.StdCall)]
        public static extern string getKeys(string N, string A, string B, string xG, string yG, string nG, string h);

        [DllImport("genCertificados.dll", EntryPoint = "getPublicX", CallingConvention = CallingConvention.StdCall)]
        public static extern string getPublicX();

        [DllImport("genCertificados.dll", EntryPoint = "getPublicY", CallingConvention = CallingConvention.StdCall)]
        public static extern string getPublicY();

        [DllImport("genCertificados.dll", EntryPoint = "getError", CallingConvention = CallingConvention.StdCall)]
        public static extern string getError();

		private string VerifyAlgorithmName(string algorithm)
		{
			string upper = algorithm.ToUpper(CultureInfo.InvariantCulture);

			switch (upper)
			{
				case "EC":
				case "ECDSA":
				case "ECDH":
				case "ECDHC":
				case "ECGOST3410":
				case "ECMQV":
					break;
				default:
					throw new ArgumentException("unrecognised algorithm: " + algorithm, "algorithm");
			}

			return upper;
		}

		internal static X9ECParameters FindECCurveByOid(DerObjectIdentifier oid)
		{
			// TODO ECGost3410NamedCurves support (returns ECDomainParameters though)

			X9ECParameters ecP = X962NamedCurves.GetByOid(oid);

			if (ecP == null)
			{
				ecP = SecNamedCurves.GetByOid(oid);

				if (ecP == null)
				{
					ecP = NistNamedCurves.GetByOid(oid);

					if (ecP == null)
					{
						ecP = TeleTrusTNamedCurves.GetByOid(oid);
					}
				}
			}

			return ecP;
		}

        public static X9ECParameters FindECCurveByName(string curveName)
        {
            X9ECParameters ecP = X962NamedCurves.GetByName(curveName);

            if (ecP == null)
            {
                ecP = SecNamedCurves.GetByName(curveName);

                if (ecP == null)
                {
                    ecP = NistNamedCurves.GetByName(curveName);

                    if (ecP == null)
                    {
                        ecP = TeleTrusTNamedCurves.GetByName(curveName);
                    }
                }
            }

            return ecP;
        }

        internal static DerObjectIdentifier FindOIDByCurveName(string curveName)
        {
            DerObjectIdentifier oid = X962NamedCurves.GetOid(curveName);

            if (oid == null)
            {
                oid = SecNamedCurves.GetOid(curveName);

                if (oid == null)
                {
                    oid = NistNamedCurves.GetOid(curveName);

                    if (oid == null)
                    {
                        oid = TeleTrusTNamedCurves.GetOid(curveName);
                    }
                }
            }

            return oid;
        }

		internal static ECPublicKeyParameters GetCorrespondingPublicKey(
			ECPrivateKeyParameters privKey)
		{
			ECDomainParameters parameters = privKey.Parameters;
			ECPoint q = parameters.G.Multiply(privKey.D);

			if (privKey.PublicKeyParamSet != null)
			{
				return new ECPublicKeyParameters(privKey.AlgorithmName, q, privKey.PublicKeyParamSet);
			}

			return new ECPublicKeyParameters(privKey.AlgorithmName, q, parameters);
		}
	}
}
