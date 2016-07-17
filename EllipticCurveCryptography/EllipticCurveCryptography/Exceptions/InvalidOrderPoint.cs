using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EllipticCurveCryptography.Exceptions
{
    public class InvalidOrderPointException:Exception
    {
        public override string Message
        {
            get
            {
                return "The order of the specified point is invalid";
            }
        }
    }
}
