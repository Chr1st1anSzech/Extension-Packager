// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Text;
using System.Text.RegularExpressions;
using Windows.UI;

namespace Extension_Packager_Library.src.Formatter
{
    public class JSONFormatter : Formatter
    {
        public Color KeyColor { get; set; } = Color.FromArgb(255, 140, 220, 255);
        public Color BoolNullColor { get; set; } = Color.FromArgb(255, 55, 140, 215);
        public Color StringColor { get; set; } = Color.FromArgb(255, 195, 145, 120);
        public Color NumberColor { get; set; } = Color.FromArgb(255, 180, 205, 170);
        public Color HighlightColor { get; set; } = Color.FromArgb(255, 40, 80, 120);

        public JSONFormatter()
        {

        }

        public JSONFormatter(Color keyColor, Color boolNullColor, Color stringColor, Color numberColor, Color highlightColor)
        {
            KeyColor = keyColor;
            BoolNullColor = boolNullColor;
            StringColor = stringColor;
            NumberColor = numberColor;
            HighlightColor = highlightColor;
        }

        

        public override void SyntaxHighlight(RichEditTextDocument document)
        {
            document.GetText(TextGetOptions.None, out string original);

            SetNormaleColor(document, 0, original.Length - 1);

            MatchCollection matches = Regex.Matches(original, @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)".Replace('¤', '"'));
            foreach (Match match in matches)
            {
                Color color = NumberColor;
                int endPosition = match.Index + match.Length;
                if (Regex.IsMatch(match.Value, "^\""))
                {
                    if (Regex.IsMatch(match.Value, ":$"))
                    {
                        color = KeyColor;
                    }
                    else
                    {
                        color = StringColor;
                    }
                }
                else if (Regex.IsMatch(match.Value, "true|false|null"))
                {
                    color = BoolNullColor;
                }
                ChangeForeground(document, match.Index, endPosition, color);
            }

            Match match2 = Regex.Match(original, @"(¤update_url¤\s*:\s*¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤)".Replace('¤', '"'));
            if (match2.Success)
            {
                int endPosition2 = match2.Index + match2.Length;
                ChangeBackground(document, match2.Index, endPosition2, HighlightColor);
            }
        }
    }
}
