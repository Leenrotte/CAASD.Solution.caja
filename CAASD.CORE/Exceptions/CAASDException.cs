using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAASD.Core.Exceptions
{
    public class CAASDException : Exception
    {
        public CAASDException(string message) : base(message)
        {
        }

        public CAASDException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}