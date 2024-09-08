using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class UnauthorizeException : Exception
    {
        public UnauthorizeException() : base() { }

        public UnauthorizeException(string message) : base(message) { }
    }
}
