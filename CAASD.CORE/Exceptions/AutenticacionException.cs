using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Exceptions
{
    public class AutenticacionException : CAASDException
    {
        public AutenticacionException(string message) : base(message)
        {
        }
    }
}
