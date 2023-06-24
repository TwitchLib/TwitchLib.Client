﻿using System.Drawing;
using System.Globalization;

namespace TwitchLib.Client.Models.Extensions.NetCore
{
    public static class ColorTranslator
    {
        public static Color FromHtml(string hexColor)
        {
            var argb = int.Parse(hexColor.Substring(1), NumberStyles.HexNumber);
            return Color.FromArgb(argb);
        }
    }
}