using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Asset
{
    public class AssetValidatorConfiguration
    {
        public static readonly List<string> AllowFormats = new List<string>()
        {
            "jpg", "png", "jpeg", "bmp"
        };

        public static readonly double MaxSizeInMb = 5;
    }
}
