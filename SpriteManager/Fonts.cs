using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace WorldISCorrupted
{

    public static class Fonts
    {
		static List<SpriteFont> CachedFonts = new List<SpriteFont>();
		static List<string> CachedFonts_Key = new List<string>();

        public static void LoadFontsDescriptors()
        {

		}

		public static SpriteFont GetFont(string FontPath, int FontSize)
        {
			int FontIndex = CachedFonts_Key.IndexOf(FontPath + ":" + FontSize);

			if (FontSize < 1) { FontSize = 1; }

			// Font was not found in cache
			if (FontIndex == -1)
            {
				var fontBakeResult = TtfFontBaker.Bake(File.ReadAllBytes(FontPath),
										 FontSize,
										 1024,
										 1024,
										 new[]
										 {
														CharacterRange.BasicLatin,
														CharacterRange.Latin1Supplement,
														CharacterRange.LatinExtendedA,
														CharacterRange.Cyrillic
										 }
									 );

				CachedFonts.Add(fontBakeResult.CreateSpriteFont(Game1.Reference.GraphicsDevice));
				CachedFonts_Key.Add(FontPath + ":" + FontSize);
                Console.WriteLine("Added font to cache");

				FontIndex = CachedFonts_Key.IndexOf(FontPath + ":" + FontSize);
			}

			return CachedFonts[FontIndex];

        }

		public static void RenderTest(SpriteBatch spriteBatch)
        {
			int SizeC = 0;
			for (int x = 0; x < 10; x++)
            {
				for(int y = 0; y < 10; y++)
                {
					SizeC += 1;
					spriteBatch.DrawString(GetFont("C:\\Windows\\Fonts\\arial.ttf", SizeC), "Ceira", new Vector2(x * (55 * SizeC / 50), y * 25), Color.Red);
					
				}
            }
        }

    }
}
