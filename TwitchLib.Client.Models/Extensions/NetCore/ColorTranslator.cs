using System.Drawing;
using System.Globalization;

namespace TwitchLib.Client.Models.Extensions.NetCore
{
    public static class ColorTranslator
    {
        public static Color FromHtml(string hexColor)
        {
            hexColor += 00;
            int argb = System.Int32.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }
    }
}