using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace EllipticCurveCryptography
{
    /// <summary>
    /// Represent an elliptic curve key pair consisting in a public and a secret keys
    /// </summary>
    [Serializable]
    public class EllipticCurveKeyPair
    {
        EllipticCurveDomainParameters domainParameters;
        EllipticCurvePublicKey publicKey;
        EllipticCurvePrivateKey privateKey;
        
        Random random=new Random(Environment.TickCount);

        /// <summary>
        /// Initialize a new EllipticCurveKeyPair given a domain parameters
        /// </summary>
        /// <param name="domainParameters">The domain parameters</param>
        public EllipticCurveKeyPair(EllipticCurveDomainParameters domainParameters)
        {
            this.domainParameters = domainParameters;
            if (domainParameters == null)
                throw new NullReferenceException();
            GenerateKeys();
        }

        /// <summary>
        /// Method for generate the public and secret keys
        /// </summary>
        public void GenerateKeys()
        {
            privateKey = new EllipticCurvePrivateKey(random.Next(1, domainParameters.OrderBasePoint - 1));
            publicKey = new EllipticCurvePublicKey(domainParameters.EllipticCurve.ScalarMultiplicate(domainParameters.BasePoint, privateKey.SecretKey));
        }

        /// <summary>
        /// Gets the BigInteger secret key
        /// </summary>
        public EllipticCurvePrivateKey SecretKey
        {
            get { return privateKey; }
        }

        /// <summary>
        /// Gets the Big2DPoint public key
        /// </summary>
        public EllipticCurvePublicKey PublicKey
        {
            get { return publicKey; }
        }
    }
}
