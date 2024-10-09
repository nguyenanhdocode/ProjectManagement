using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class DayHelper
    {
        public static int DaysRounded(TimeSpan input, MidpointRounding rounding = MidpointRounding.AwayFromZero)
        {
            int roundupHour = rounding == MidpointRounding.AwayFromZero ? 12 : 13;
            if (input.Hours >= roundupHour)
                return input.Days + 1;
            else
                return input.Days;
        }

    }
}
