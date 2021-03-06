/*
   ####### BEGIN APACHE 2.0 LICENSE #######
   Copyright 2020 Aragubas

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

   ####### END APACHE 2.0 LICENSE #######




   ####### BEGIN MONOGAME LICENSE #######
   THIS GAME-ENGINE WAS CREATED USING THE MONOGAME FRAMEWORK
   Github: https://github.com/MonoGame/MonoGame#license 

   MONOGAME WAS CREATED BY MONOGAME TEAM

   THE MONOGAME LICENSE IS IN THE MONOGAME_License.txt file on the root folder. 

   ####### END MONOGAME LICENSE ####### 


*/

using System;
using Microsoft.Xna.Framework.Graphics;


namespace WorldISCorrupted
{
    public static class Global
    {
        // Os Name
        public static string OSName = Environment.OSVersion.Platform.ToString();

        // Directories
        public static string ContentFolder = "WorldIsCorrupted_data\\";
        public static string TaiyouDirectory = ContentFolder + "taiyou\\";
        public static string TaiyouScriptsDirectory = ContentFolder + "taiyou\\scripts\\";
        public static string SourceDirectory = ContentFolder + "source\\";
        public static string BGM_SourceFolder = SourceDirectory + "bgm";
        public static string FONT_SourceFolder = SourceDirectory + "font";
        public static string IMAGE_SourceFolder = SourceDirectory + "image";
        public static string SOUND_SourceFolder = SourceDirectory + "sound";
        public static string REGISTRY_SourceFolder = SourceDirectory + "reg";
        public static string MAP_SourceFolder = SourceDirectory + "map";
        public static string BIN_RootSourceFolder = SourceDirectory + "bin\\";
        public static string BIN_SourceFolder = BIN_RootSourceFolder + OSName;
        public static string FSCheck_Folder = ContentFolder + "fs_check\\";
        public static string System_Folder = ContentFolder + "system\\";
        public static string SystemLog_Folder = System_Folder + "logs";


        // Version Match
        public static string MatchVersion_CurrentVersionString = "WIC_2021_FEV_2021";


        // Map Rendering
        public static int TileSize = 32;
        public static bool IsOnEditorMode;

        // Screen
        public static int WindowWidth;
        public static int WindowHeight;


        // Global variables for map rendering wax
        public static bool ForceReloadMap = false;
        public static string CurrentMapName = "";

        // Global Variables for Map Editor
        public static string MapEditor_CurrentMapName = "untitled_map";
        public static bool MapEditor_ReloadMap = false;

        // Global Game Settings
        public static bool Setting_LightningEnabled = true;

        public static BlendState BlendState_LightningBlendState = new BlendState()
        {
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
        };

        public static void LoadValues()
        {
            System.Console.WriteLine("Loading Settings Values");
            Setting_LightningEnabled = Convert.ToBoolean(Registry.ReadKeyValue("/options/lightning"));

        }


    }
}