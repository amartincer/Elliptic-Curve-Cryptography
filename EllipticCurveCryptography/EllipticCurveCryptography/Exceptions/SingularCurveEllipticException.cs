using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EllipticCurveCryptography.Exceptions
{
    public class SingularCurveEllipticException:Exception
    {
        public override string Message
        {
            get
            {
                return "The elliptic curve is singular";
            }
        }
    }
}
