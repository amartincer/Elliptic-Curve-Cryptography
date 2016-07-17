using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EllipticCurveCryptography
{
    public class EllipticCurvePublicKey
    {
        Big2DPoint publicKey = Big2DPoint.InfinitePoint;

        public EllipticCurvePublicKey(Big2DPoint publicKey)
        {
            this.publicKey = publicKey;                
        }

        public Big2DPoint PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }
    }
}
