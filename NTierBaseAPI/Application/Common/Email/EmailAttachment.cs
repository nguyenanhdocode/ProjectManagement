using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Email
{
    public class EmailAttachment
    {
        public byte[] Value { get; private set; }

        public string Name { get; private set; }

        public EmailAttachment(byte[] value, string name)
        {
            Value = value;
            Name = name;
        }
    }
}
