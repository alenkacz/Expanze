using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Expanze.Utils
{
    static class TextWrapping
    {
        public static void DrawStringIntoRectangle(String text, SpriteFont font, Color color, float x, float y, float width)
        {
            String textCopy = text;
            String row = "";
            String word;
            float rowY = y;
            float rowWidth = 0;

            while (textCopy.Length != 0)
            {
                int spaceIndex = textCopy.IndexOf(' ');
                if (spaceIndex < 0)
                {
                    word = textCopy;
                    textCopy = "";
                }
                else
                    word = textCopy.Substring(0, spaceIndex);

                bool drawRow = true;
                if (rowWidth + font.MeasureString(word + " ").X <= width)
                {
                    row += word + " ";
                    rowWidth += font.MeasureString(word + " ").X;
                    if(textCopy.Length > 0)
                        drawRow = false;
                }
                
                if(drawRow)
                {
                    GameState.spriteBatch.DrawString(font, row, new Vector2(x, rowY), color);
                    row = word + " ";
                    rowWidth = font.MeasureString(word + " ").X;
                    rowY += font.LineSpacing;
                }
                textCopy = textCopy.Substring(spaceIndex + 1);
            }
        }
    }
}
