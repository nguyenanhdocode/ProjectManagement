using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ConflictExeception : Exception
    {
        public ConflictExeception() : base() { }

        public ConflictExeception(string message) : base(message) { }
    }
}
