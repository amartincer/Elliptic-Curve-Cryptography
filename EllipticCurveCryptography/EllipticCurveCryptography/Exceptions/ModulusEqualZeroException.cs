using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EllipticCurveCryptography.Exceptions
{
    public class ModulusEqualZeroException:Exception
    {
        public override string Message
        {
            get
            {
                return "The value of the modulus don't be equal zero";
            }
        }
    }
}
