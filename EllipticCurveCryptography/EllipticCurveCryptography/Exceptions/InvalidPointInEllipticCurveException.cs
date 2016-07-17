using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EllipticCurveCryptography.Exceptions
{
    public class InvalidPointInEllipticCurveException:Exception
    {
        public override string Message
        {
            get
            {
                return "The point dont' belong to the specified elliptic curve";
            }
        }
    }
}
