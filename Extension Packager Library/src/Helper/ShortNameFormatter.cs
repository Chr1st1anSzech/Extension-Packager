// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.RegularExpressions;

namespace Extension_Packager_Library.src.Helper
{
    public interface IShortNameFormatter
    {
        public string Format(string input);
    }

    public class ShortNameFormatter : IShortNameFormatter
    {
        public string Format(string input)
        {
            return Regex.Replace(input, "[^A-Za-z_\\-0-9]", "");
        }
    }
}
