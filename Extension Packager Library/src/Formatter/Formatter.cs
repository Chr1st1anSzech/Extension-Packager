// Copyright (c) Christian Szech
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Text;
using Windows.UI;

namespace Extension_Packager_Library.src.Formatter
{
    public abstract class Formatter
    {
        public Color NormalColor { get; set; } = Color.FromArgb(255, 128, 128, 128);

        internal void ChangeBackground(RichEditTextDocument document, int start, int endPosition, Color color)
        {
            var textRange = document.GetRange(start, endPosition);
            textRange.CharacterFormat.BackgroundColor = color;
        }

        internal void ChangeForeground(RichEditTextDocument document, int start, int endPosition, Color color)
        {
            var textRange = document.GetRange(start, endPosition);
            textRange.CharacterFormat.ForegroundColor = color;
        }

        internal void SetNormaleColor(RichEditTextDocument document, int start, int endPosition)
        {
            var textRange = document.GetRange(start, endPosition);
            textRange.CharacterFormat.ForegroundColor = NormalColor;
        }

        public abstract void SyntaxHighlight(RichEditTextDocument document);
    }
}
