using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace EllipticCurveCryptography
{
    public class EllipticCurvePrivateKey
    {
        BigInteger secretKey = BigInteger.Zero;

        public EllipticCurvePrivateKey(BigInteger secretKey)
        {
            this.secretKey = secretKey;
        }

        public BigInteger SecretKey
        {
            get { return secretKey; }
            set { secretKey = value; }
        }
    }
}
