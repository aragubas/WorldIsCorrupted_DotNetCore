using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using WorldISCorrupted.WriteToLog;

namespace WorldISCorrupted
{

    public static class Fonts
    {
		static List<SpriteFont> CachedFonts = new List<SpriteFont>();
		static List<string> CachedFonts_Key = new List<string>();
		
		public static void LoadFontsDescriptors()
        {
			LogObject Log = new LogObject("FontsFontDescriptor");
			Log.Write("Procesing font cache descriptor...");

			string FontDescriptorsData = Registry.ReadKeyValue("/font_precaching");
			string[] DescriptorSplit = FontDescriptorsData.Replace(Environment.NewLine, "").Split(';');

			foreach(var descriptor in DescriptorSplit)
            {
				string processedInput = descriptor.TrimStart().TrimEnd();
				string[] SubSplit = processedInput.Split(':');
				Log.Write("Processing entry {" + processedInput + "}...");
				string FontPath;
				int FontSize;

                // Sample:
                // /Ubuntu-B.ttf,12;

                try
                {
					FontPath = SubSplit[0];
					FontSize = Convert.ToInt32(SubSplit[1]);


				}
				catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Error While loading font descriptor:\nInvalid data found while loading, resulting in IndexOutOfRange.");
					Log.Write("Error While loading font descriptor:\nInvalid data found while loading, resulting in IndexOutOfRange.");

					continue;
                }

				Log.Write("Font Info : Path{" + FontPath + "} Size{" + FontSize + "}");
				LoadFont(FontPath, FontSize);

			}

			Log.Write("Font loading complete.");
			Log.FinishLog(true);


		}

		public static void LoadFont(string FontPath, int FontSize)
        {
            Console.WriteLine("Font : Add font to cache");
			FontPath = FontPath.Replace("/", "\\");
			FontPath = Environment.CurrentDirectory + "\\" + Global.FONT_SourceFolder + "\\" + FontPath;
			FontPath = FontPath.Replace("\\\\", "\\");
            Console.WriteLine("Font path {" + FontPath + "}");
			string FontDescName = FontPath.Replace(Environment.CurrentDirectory + "\\" + Global.FONT_SourceFolder + "\\", "/") + ":" + FontSize;
            Console.WriteLine("Font name {" + FontDescName + "}");

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
			CachedFonts_Key.Add(FontDescName);

		}

		public static SpriteFont GetFont(string FontPath, int FontSize)
        {
			int FontIndex = CachedFonts_Key.IndexOf(FontPath + ":" + FontSize);

			if (FontSize < 1) { FontSize = 1; }

			// Font was not found in cache
			if (FontIndex == -1)
            {
                Console.WriteLine("A font is being added to Font Cache");
                Console.WriteLine("Please wait, the game has not frozen");

				LoadFont(FontPath, FontSize);

				Console.WriteLine("Sucefully added font to font cache");

				FontIndex = CachedFonts_Key.IndexOf(FontPath + ":" + FontSize);

				if (FontIndex == -1)
                {
					throw new NotImplementedException("A internal bug has occoured on the application.\nFont was added to cache, but its index could not be found.");
                }
			}

			return CachedFonts[FontIndex];

        }

	}
}
