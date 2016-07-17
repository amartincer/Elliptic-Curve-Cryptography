using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EllipticCurveCryptography.Exceptions
{
    public class InvalidDomainParametersException:Exception
    {
        public override string Message
        {
            get
            {
                return "Invalid domain parameters";
            }
        }
    }
}
