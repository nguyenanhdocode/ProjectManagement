using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public enum AppTaskStatus
    {
        Unfulfilled,
        Doing,
        Suspend,
        Done,
        Redo
    }
}
