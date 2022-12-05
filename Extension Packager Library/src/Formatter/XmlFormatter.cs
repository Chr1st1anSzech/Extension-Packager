// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Text;
using System.Text.RegularExpressions;
using Windows.UI;

namespace Extension_Packager_Library.src.Formatter
{
    public class XmlFormatter : Formatter
    {
        public Color AttributeColor { get; set; } = Color.FromArgb(255, 140, 220, 255);
        public Color ElementNameColor { get; set; } = Color.FromArgb(255, 55, 140, 215);
        public Color ValueColor { get; set; } = Color.FromArgb(255, 195, 145, 120);
        public Color HighlightColor { get; set; } = Color.FromArgb(255, 40, 80, 120);

        public XmlFormatter()
        {

        }

        public XmlFormatter(Color attributeColor, Color elementNameColor, Color valueColor, Color highlightColor)
        {
            AttributeColor = attributeColor;
            ElementNameColor = elementNameColor;
            ValueColor = valueColor;
            HighlightColor = highlightColor;
        }


        public override void SyntaxHighlight(RichEditTextDocument document)
        {
            document.GetText(TextGetOptions.None, out string original);

            SetNormaleColor(document, 0, original.Length - 1);

            MatchCollection matches = Regex.Matches(original, "(?<=<(\\??|\\/))[a-zA-Z_:]+");
            foreach (Match match in matches)
            {
                int endPosition = match.Index + match.Length;
                ChangeForeground(document, match.Index, endPosition, ElementNameColor);
            }

            matches = Regex.Matches(original, "[a-zA-Z_:]+(?==\".*\")");
            foreach (Match match in matches)
            {
                int endPosition = match.Index + match.Length;
                ChangeForeground(document, match.Index, endPosition, AttributeColor);
            }

            matches = Regex.Matches(original, "(?<=[a-zA-Z_:]+=)(\"|')[^\"]*(\"|')");
            foreach (Match match in matches)
            {
                int endPosition = match.Index + match.Length;
                ChangeForeground(document, match.Index, endPosition, ValueColor);
            }
        }
        
    }
}
