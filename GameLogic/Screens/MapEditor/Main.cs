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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WorldISCorrupted.GameLogic.Screens;
using WorldISCorrupted.GameLogic.Screens.MapEditor;

namespace WorldISCorrupted.GameLogic.Screens.MapEditor
{
    public class Main : GameScreen
    {

        KeyboardState oldState;

        List<MapTile> MapBackground = new List<MapTile>();
        List<MapTile> MapTiles = new List<MapTile>();
        List<MapTile> MapObjects = new List<MapTile>();
        List<MapTile> VisibleTiles = new List<MapTile>();

        float CameraX;
        float CameraY;
        int CameraSpeed = 5;

        int CursorX = 0;
        int CursorY = 0;
        Rectangle MouseColision;
        int Cursor_TileSelected = -1;
        int HUD_SelectedTileIndex = 0;
        string HUD_SelectedTileTag = "none";
        string HUD_SelectedTileTagPrevious = "none";
        string HUD_SelectedTileTagNext = "none";

        string HUD_SelectedTileAtributesString = "null";
        bool HUD_SelectedTileAtribute_Colideable = false;
        int HUD_SelectedTileAtribute_ObjectTileInterationID;
        int HUD_SelectedLayer = 0;
        int HUD_X = 0;
        int HUD_Width = 150;
        bool HUD_IsHiddenToggle = false;
        bool HUD_IsHidden = false;
        bool HUD_IsHiddenAnimation_Enabled = false;
        float HUD_IsHiddenAnimation_Value;
        float HUD_IsHiddenAnimation_ValueMultiplier;
        int HUD_IsHiddenAnimationMode;

        // Dialog
        bool DialogEnabled = false;
        bool MapHasBeenLoaded;
        int DialogRequestedOperation = -1;
        Dialog MessageDialog;

        // Map Data
        int MapWidth = 32;
        int MapHeight = 32;
        string CustomMapProperty = "";

        List<int> MapTilesByID_Indexes = new List<int>();
        List<string> MapTilesByID_String = new List<string>();

        private void UpdateCamera(KeyboardState newState)
        {
            Global.IsOnEditorMode = true;

            if (Utils.CheckKeyDown(oldState, newState, Keys.W))
            {
                CameraY += CameraSpeed;
            }
            if (Utils.CheckKeyDown(oldState, newState, Keys.S))
            {
                CameraY -= CameraSpeed;
            }
            if (Utils.CheckKeyDown(oldState, newState, Keys.A))
            {
                CameraX += CameraSpeed;
            }
            if (Utils.CheckKeyDown(oldState, newState, Keys.D))
            {
                CameraX -= CameraSpeed;
            }

            if (Utils.CheckKeyDown(oldState, newState, Keys.Z))
            {
                Global.TileSize -= 1;

                if (Global.TileSize <= 8)
                {
                    Global.TileSize = 8;
                }

            }
            if (Utils.CheckKeyDown(oldState, newState, Keys.X))
            {
                Global.TileSize += 1;

            }

        }

        public void UpdateHUD(KeyboardState newState)
        {
            // Update X cordinate
            HUD_X = Convert.ToInt32(HUD_IsHiddenAnimation_Value);


            // Update IsHidden Animation
            if (HUD_IsHiddenToggle && !HUD_IsHiddenAnimation_Enabled)
            {
                HUD_IsHiddenToggle = false;
                HUD_IsHiddenAnimation_Enabled = true;

            }

            if (HUD_IsHiddenAnimation_Enabled)
            {
                HUD_IsHiddenAnimation_ValueMultiplier += 1f;

                if (HUD_IsHiddenAnimationMode == 0)
                {
                    HUD_IsHiddenAnimation_Value -= HUD_IsHiddenAnimation_ValueMultiplier;

                    if (HUD_IsHiddenAnimation_Value <= -HUD_Width + 10)
                    {
                        HUD_IsHiddenAnimation_Value = -HUD_Width + 10;
                        HUD_IsHiddenAnimation_Enabled = false;
                        HUD_IsHiddenAnimationMode = 1;
                        HUD_IsHiddenAnimation_ValueMultiplier = 0.5f;
                        HUD_IsHidden = true;

                    }

                }
                else if (HUD_IsHiddenAnimationMode == 1)
                {
                    HUD_IsHiddenAnimation_Value += HUD_IsHiddenAnimation_ValueMultiplier;

                    if (HUD_IsHiddenAnimation_Value >= 0)
                    {
                        HUD_IsHiddenAnimation_Value = 0;
                        HUD_IsHiddenAnimation_Enabled = false;
                        HUD_IsHiddenAnimationMode = 0;
                        HUD_IsHiddenAnimation_ValueMultiplier = 0.5f;
                        HUD_IsHidden = false;
                    }

                }

            }

            // Hides/Unhides HUD
            if (Utils.CheckKeyUp(oldState, newState, Keys.J))
            {
                HUD_IsHiddenToggle = true;
            }


            if (HUD_IsHidden) { return; }

            // Next TileSprite
            if (Utils.CheckKeyUp(oldState, newState, Keys.Up))
            {
                HUD_SelectedTileIndex += 1;

                UpdateTileIndex();
            }

            // Previuos Map Tile
            if (Utils.CheckKeyUp(oldState, newState, Keys.Down))
            {
                HUD_SelectedTileIndex -= 1;

                if (HUD_SelectedTileIndex <= -2)
                {
                    HUD_SelectedTileIndex = 0;
                }

                UpdateTileIndex();
            }

            // Next Map Layer
            if (Utils.CheckKeyUp(oldState, newState, Keys.F10))
            {
                if (HUD_SelectedLayer == Convert.ToInt32(Registry.ReadKeyValue("/editor/map_max_layers")))
                {
                    HUD_SelectedTileIndex = Convert.ToInt32(Registry.ReadKeyValue("/editor/map_max_layers"));
                }
                else
                {
                    HUD_SelectedLayer += 1;
                }


            }

            // Previus Map Layer
            if (Utils.CheckKeyUp(oldState, newState, Keys.F9))
            {
                HUD_SelectedLayer -= 1;

                if (HUD_SelectedLayer <= -1)
                {
                    HUD_SelectedLayer = -1;
                }
            }

            // Clear custom atributes
            if (Utils.CheckKeyUp(oldState, newState, Keys.F12))
            {
                UpdateTileAtribute();

            }

            // Call dialog for setting custom atributes
            if (Utils.CheckKeyUp(oldState, newState, Keys.F11))
            {
                if (HUD_SelectedLayer == 1)
                {
                    DialogAction(4);

                }
                else
                {
                    DialogMessage("Re");
                }
            }


            // Toggle Help
            if (Utils.CheckKeyUp(oldState, newState, Keys.H))
            {
                DialogMessage(Registry.ReadKeyValue("/editor/help_screen"));
            }

            // Set IsColideable Parameter
            if (Utils.CheckKeyUp(oldState, newState, Keys.F5))
            {
                if (HUD_SelectedLayer != 0)
                {
                    DialogMessage(Registry.ReadKeyValue("/editor/editor_layer_not_supports_colision_flag"));

                }
                else
                {
                    if (!HUD_SelectedTileAtribute_Colideable) { HUD_SelectedTileAtribute_Colideable = true; } else { HUD_SelectedTileAtribute_Colideable = false; }

                }
            }

            // Decrease InterationID
            if (Utils.CheckKeyUp(oldState, newState, Keys.F6))
            {
                if (HUD_SelectedLayer != 1)
                {
                    DialogMessage(Registry.ReadKeyValue("/editor/editor_layer_not_supports_action_id"));

                }
                else
                {
                    HUD_SelectedTileAtribute_ObjectTileInterationID -= 1;

                    if (HUD_SelectedTileAtribute_ObjectTileInterationID <= -1)
                    {
                        HUD_SelectedTileAtribute_ObjectTileInterationID = 0;
                    }

                }


            }

            // Increase InteractionID
            if (Utils.CheckKeyUp(oldState, newState, Keys.F7))
            {
                if (HUD_SelectedLayer != 1)
                {
                    DialogMessage(Registry.ReadKeyValue("/editor/editor_layer_not_supports_action_id"));

                }
                else
                {
                    HUD_SelectedTileAtribute_ObjectTileInterationID += 1;

                }


            }

            // Run Test the Map
            if (Utils.CheckKeyUp(oldState, newState, Keys.F4))
            {
                SaveCurrentMap();

                Global.CurrentMapName = Global.MAP_SourceFolder + "/" + Global.MapEditor_CurrentMapName + ".map";
                Global.ForceReloadMap = true;


                ScreenSelector.SetCurrentScreen(0);
            }

            // Save Dialog
            if (Utils.CheckKeyUp(oldState, newState, Keys.F1))
            {
                if (!MapHasBeenLoaded)
                {
                    DialogMessage(Registry.ReadKeyValue("/editor/cannot_save_without_map"));

                }
                else
                {
                    DialogAction(1);

                }

            }

            // Load Dialog
            if (Utils.CheckKeyUp(oldState, newState, Keys.F2))
            {
                DialogAction(2);

            }

            // NewMap Dialog
            if (Utils.CheckKeyUp(oldState, newState, Keys.F3))
            {
                DialogAction(3);

            }

            // Map Properties Dialog
            if (Utils.CheckKeyUp(oldState, newState, Keys.P))
            {
                DialogAction(5);

            }


            // Update selected tile
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                UpdateSelectedTile();
            }
        }

        private void UpdateSelectedTile()
        {
            try
            {
                switch (HUD_SelectedLayer)
                {
                    case -1:
                        MapBackground[Cursor_TileSelected].atributes = HUD_SelectedTileAtributesString.Split(',');
                        MapBackground[Cursor_TileSelected].UpdateTileAtributes();
                        break;

                    case 0:
                        MapTiles[Cursor_TileSelected].atributes = HUD_SelectedTileAtributesString.Split(',');
                        MapTiles[Cursor_TileSelected].UpdateTileAtributes();
                        break;

                    case 1:
                        MapObjects[Cursor_TileSelected].atributes = HUD_SelectedTileAtributesString.Split(',');
                        MapObjects[Cursor_TileSelected].UpdateTileAtributes();
                        break;

                }

            }
            catch (Exception)
            {
                Console.WriteLine("Can't change tile that is outside the bounds of map.\nLayer: " + HUD_SelectedLayer + "\nTileSelected: " + Cursor_TileSelected);

            }

        }

        private void ProcessDialogActions()
        {
            string DialogOutput = MessageDialog.DialogOutput;

            if (DialogOutput == "NONE")
            {
                Console.WriteLine("Dialog returned NONE\nOperation canceled");
                return;
            }

            switch (DialogRequestedOperation)
            {
                case 1:
                    // MessageDialog.DialogOutput
                    Global.MapEditor_CurrentMapName = DialogOutput;
                    SaveCurrentMap();

                    break;

                case 2:
                    Global.MapEditor_CurrentMapName = DialogOutput;
                    LoadMap();
                    break;

                case 3:
                    ReloadMap(true, DialogOutput);
                    break;


                case 4:
                    HUD_SelectedTileAtributesString = DialogOutput;
                    //UpdateTileAtribute();
                    break;

                case 5:
                    CustomMapProperty = DialogOutput;
                    break;


            }

        }

        public override void Update()
        {
            base.Update();
            KeyboardState newState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();

            CursorX = newMouseState.X;
            CursorY = newMouseState.Y;
            MouseColision = new Rectangle(CursorX, CursorY, 1, 1);

            if (DialogEnabled)
            {
                MessageDialog.Update();

                if (MessageDialog.DialogFinished)
                {
                    DialogEnabled = false;

                    if (DialogRequestedOperation != -1)
                    {
                        string DialogOutput = MessageDialog.DialogOutput;
                        ProcessDialogActions();

                    }

                }

                return;
            }

            UpdateCamera(newState);
            UpdateHUD(newState);

            // Set Tile Atribute ID to 0 if not Object Map
            if (HUD_SelectedLayer != 1)
            {
                HUD_SelectedTileAtribute_ObjectTileInterationID = 0;

            }

            // Set Tile Colideable Flag to False if not Background Objects
            if (HUD_SelectedLayer != 0)
            {
                HUD_SelectedTileAtribute_Colideable = false;

            }

            // Update the Atribute String 
            if (HUD_SelectedLayer < 1)
            {
                UpdateTileAtribute();

            }


            oldState = newState;
        }

        private void UpdateTileAtribute()
        {
            HUD_SelectedTileAtributesString = "sprite:" + HUD_SelectedTileTag + ",colideable:" + HUD_SelectedTileAtribute_Colideable + ",obj_tile_id:" + HUD_SelectedTileAtribute_ObjectTileInterationID;

        }

        private void SaveCurrentMap()
        {
            if (Global.MapEditor_CurrentMapName == "")
            {
                return;
            }
            Console.Clear();
            Console.WriteLine("SaveMap() -- WIC Map Engine\nFileName: " + Global.MapEditor_CurrentMapName + "\nCurrent Operation:");

            // Save BGMAP
            string MapTilesData = "";
            string MapObjectsData = "";
            string MapBackgroundData = "";
            string MapMetaData = "";

            if (!MapHasBeenLoaded)
            {
                DialogMessage(Registry.ReadKeyValue("/editor/cannot_save_without_map"));
                Console.WriteLine("No map data has been loaded, map saving aborted");
                return;
            }

            // Map Background Processing
            foreach (var tile in MapBackground)
            {
                string all_atribute_to_string_list = "";

                int index = -1;
                foreach (var atr in tile.atributes)
                {
                    index += 1;

                    if (index != tile.atributes.Length - 1) // Start
                    {
                        all_atribute_to_string_list += atr + ",";

                    }
                    else // Middle
                    {
                        all_atribute_to_string_list += atr;
                    }

                }

                string tileString = tile.x + ";" + tile.y + ";" + all_atribute_to_string_list;
                MapBackgroundData += "\n" + tileString;

            }

            // Map objects
            foreach (var tile in MapTiles)
            {
                string all_atribute_to_string_list = "";

                int index = -1;

                foreach (var atr in tile.atributes)
                {
                    index += 1;

                    if (index != tile.atributes.Length - 1) // Start
                    {
                        all_atribute_to_string_list += atr + ",";

                    }
                    else // Middle
                    {
                        all_atribute_to_string_list += atr;
                    }


                }

                string tileString = tile.x + ";" + tile.y + ";" + all_atribute_to_string_list;
                MapTilesData += "\n" + tileString;


            }

            // Map Objects
            foreach (var tile in MapObjects)
            {
                string all_atribute_to_string_list = "";

                int index = -1;
                foreach (var atr in tile.atributes)
                {
                    index += 1;

                    if (index != tile.atributes.Length - 1) // Start
                    {
                        all_atribute_to_string_list += atr + ",";

                    }
                    else // Middle
                    {
                        all_atribute_to_string_list += atr;
                    }

                }

                string tileString = tile.x + ";" + tile.y + ";" + all_atribute_to_string_list;
                MapObjectsData += "\n" + tileString;

            }


            MapMetaData = "width:" + MapWidth + ";" +
                          "height:" + MapHeight + ";" +
                          CustomMapProperty + ";" +
                          "end\n\n\n###### This string has been generated using WIC Map Editor ######\nVersion: " + Global.MatchVersion_CurrentVersionString + "\nMapCreationDate:" + DateTime.Now.ToString() + "\n\n###### Metadata String End ######";

            // Write Map File
            string MapFilePath = Global.MAP_SourceFolder + "/" + Global.MapEditor_CurrentMapName + ".map";


            string MapFile = MapBackgroundData + "|" +
                             MapTilesData + "|" +
                             MapObjectsData + "|" +
                             MapMetaData;

            File.WriteAllText(MapFilePath, MapFile);

        }

        private void LoadMap()
        {
            Console.Clear();
            Console.WriteLine("LoadMap() -- WIC Map Engine\nFileName: " + Global.MapEditor_CurrentMapName + "\nCurrent Operation:");

            MapTiles.Clear();
            MapObjects.Clear();
            MapBackground.Clear();

            try
            {
                string[] MapFileRead = File.ReadAllText(Global.MAP_SourceFolder + "/" + Global.MapEditor_CurrentMapName + ".map").Split('|');
                string[] MapBGData = MapFileRead[0].Split(new[] { '\r', '\n' });
                string[] MapTileData = MapFileRead[1].Split(new[] { '\r', '\n' });
                string[] MapObjData = MapFileRead[2].Split(new[] { '\r', '\n' });
                string[] MapMetaData = MapFileRead[3].Split(';');


                foreach (var tile in MapBGData)
                {
                    if (tile.Length < 2) { continue; }


                    // Split Line arguments
                    string[] LineArgs = tile.Split(';');
                    int TileX = Convert.ToInt32(LineArgs[0]);
                    int TileY = Convert.ToInt32(LineArgs[1]);
                    string[] TileProperties = LineArgs[2].Split(',');

                    MapTile ThisTile = new MapTile(TileX, TileY, TileProperties, MapBackground.Count);

                    MapBackground.Add(ThisTile);
                }

                foreach (var tile in MapTileData)
                {
                    if (tile.Length < 2) { continue; }
                    // Split Line arguments
                    string[] LineArgs = tile.Split(';');
                    int TileX = Convert.ToInt32(LineArgs[0]);
                    int TileY = Convert.ToInt32(LineArgs[1]);
                    string[] TileProperties = LineArgs[2].Split(',');

                    MapTile ThisTile = new MapTile(TileX, TileY, TileProperties, MapTiles.Count);

                    MapTiles.Add(ThisTile);
                }

                foreach (var tile in MapObjData)
                {
                    if (tile.Length < 2) { continue; }

                    // Split Line arguments
                    string[] LineArgs = tile.Split(';');
                    int TileX = Convert.ToInt32(LineArgs[0]);
                    int TileY = Convert.ToInt32(LineArgs[1]);
                    string[] TileProperties = LineArgs[2].Split(',');

                    MapTile ThisTile = new MapTile(TileX, TileY, TileProperties, MapObjects.Count);

                    MapObjects.Add(ThisTile);
                }

                List<string> LinesWitoutIt = new List<string>();
                CustomMapProperty = "";
                foreach (var MetaLine in MapMetaData)
                {
                    string[] SplitedArgs = MetaLine.Split(':');

                    switch (SplitedArgs[0])
                    {

                        case "width":
                            MapWidth = Convert.ToInt32(SplitedArgs[1]);
                            break;

                        case "height":
                            MapHeight = Convert.ToInt32(SplitedArgs[1]);
                            break;

                        default:
                            try
                            {
                                LinesWitoutIt.Add(SplitedArgs[0] + ":" + SplitedArgs[1]);
                            }
                            catch (IndexOutOfRangeException)
                            {

                            }
                            break;

                    }
                }
                foreach (var CustomProp in LinesWitoutIt)
                {
                    if (CustomProp.Contains("end"))
                    {
                        break;
                    }
                    CustomMapProperty += CustomProp + ";";
                }

                System.Console.WriteLine(CustomMapProperty);
                Console.WriteLine("Operation Completed.");

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Error while loading Mapfile: [" + Global.MapEditor_CurrentMapName + "]\nInvalid map file.");

                DialogMessage(Registry.ReadKeyValue("/editor/map_load_error_1"));

            }

            catch (FileNotFoundException)
            {
                Console.WriteLine("Error while loading Mapfile: [" + Global.MapEditor_CurrentMapName + "]\nCannot find map file.");

                DialogMessage(Registry.ReadKeyValue("/editor/map_load_error_2"));

            }

            MapHasBeenLoaded = true;

        }

        private void DialogMessage(string Message)
        {
            MessageDialog = new Dialog(0, new List<string>() { Message });
            DialogEnabled = true;
            DialogRequestedOperation = -1;

        }

        private void DialogAction(int DialogActionID)
        {
            string Message = "ACTION_ID_" + DialogActionID.ToString();
            string TextboxDefaultText = "ACTION_DEFAULT_TEXT_ID_" + DialogActionID.ToString();

            switch (DialogActionID)
            {
                case 1:
                    Message = Registry.ReadKeyValue("/editor/save_map_dialog_title");
                    TextboxDefaultText = Global.MapEditor_CurrentMapName;

                    break;

                case 2:
                    Message = Registry.ReadKeyValue("/editor/load_map_dialog_title");
                    TextboxDefaultText = Global.MapEditor_CurrentMapName;

                    break;

                case 3:
                    Message = Registry.ReadKeyValue("/editor/new_map_title");
                    TextboxDefaultText = MapWidth + "x" + MapHeight;

                    break;


                case 4:
                    Message = Registry.ReadKeyValue("/editor/custom_atributes_title");
                    UpdateTileAtribute();
                    TextboxDefaultText = HUD_SelectedTileAtributesString;

                    break;

                case 5:
                    Message = "Map Properties";
                    TextboxDefaultText = CustomMapProperty;
                    break;



            }

            MessageDialog = new Dialog(1, new List<string>() { Message, TextboxDefaultText });
            DialogRequestedOperation = DialogActionID;
            DialogEnabled = true;

        }

        private void UpdateTileIndex()
        {
            int pSelectedTileIndex = MapTilesByID_Indexes.IndexOf(HUD_SelectedTileIndex);

            if (pSelectedTileIndex == -1)
            {
                return;
            }

            HUD_SelectedTileTag = MapTilesByID_String[pSelectedTileIndex];

            if (pSelectedTileIndex + 1 >= MapTilesByID_String.Count)
            {
                HUD_SelectedTileTagNext = Registry.ReadKeyValue("/missing_texture");

            }
            else
            {
                HUD_SelectedTileTagNext = MapTilesByID_String[pSelectedTileIndex + 1];

            }

            if (pSelectedTileIndex - 1 == -1)
            {
                HUD_SelectedTileTagPrevious = Registry.ReadKeyValue("/missing_texture");

            }
            else
            {
                HUD_SelectedTileTagPrevious = MapTilesByID_String[pSelectedTileIndex - 1];

            }

        }

        public void RenderTileCursor(SpriteBatch spriteBatch)
        {
            foreach (var tile in VisibleTiles)
            {
                Rectangle TileColRect = new Rectangle((int)CameraX + tile.x * Global.TileSize, (int)CameraY + tile.y * Global.TileSize, Global.TileSize, Global.TileSize);
                Rectangle CursorCol = new Rectangle(CursorX, CursorY, Global.TileSize, Global.TileSize);
                if (CursorCol.Intersects(TileColRect))
                {
                    Color CursorColor = Color.FromNonPremultiplied(15, 15, 15, 100);

                    // Switch the CursorColor
                    switch (HUD_SelectedLayer)
                    {
                        case -1:
                            CursorColor.R = 50;
                            CursorColor.G = 65;
                            CursorColor.B = 82;
                            break;

                        case 0:
                            CursorColor.R = 120;
                            CursorColor.G = 135;
                            CursorColor.B = 112;
                            break;

                        case 1:
                            CursorColor.R = 10;
                            CursorColor.G = 25;
                            CursorColor.B = 52;
                            break;

                    }

                    spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(tile.x * Global.TileSize, tile.y * Global.TileSize, Global.TileSize, Global.TileSize), CursorColor);


                    // Update Map Cursor
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        Cursor_TileSelected = tile.index;
                    }

                    break;
                }

            }

        }

        private void RenderMap(SpriteBatch spriteBatch)
        {
            Matrix Camera = Matrix.CreateTranslation(new Vector3(CameraX, CameraY, 0));
            spriteBatch.Begin(transformMatrix: Camera);

            // Render Background Map
            if (MapBackground.Count != 0)
            {
                spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(0, 0, MapWidth * Global.TileSize, MapHeight * Global.TileSize), Color.DimGray);
            }
            else
            {
                spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(0, 0, MapWidth * Global.TileSize, MapHeight * Global.TileSize), Color.FromNonPremultiplied(15, 15, 15, 255));

            }

            VisibleTiles.Clear();

            // Render Map Background
            foreach (var tile in MapBackground)
            {
                if (MapBackground.Count == 0) { return; }

                // If tile has been rendered, add to the Visible Tiles list
                if (tile.Render(spriteBatch, (int)CameraX, (int)CameraY)) { if (HUD_SelectedLayer == -1) { VisibleTiles.Add(tile); } }

                // If tile is the selected tile, do something special
                if (HUD_SelectedLayer != -1) { continue; }

                if (tile.index == Cursor_TileSelected)
                {
                    spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(tile.x * Global.TileSize, tile.y * Global.TileSize, Global.TileSize, Global.TileSize), Color.FromNonPremultiplied(0, 0, 0, 155));
                }

            }

            if (HUD_SelectedLayer == -1) { RenderTileCursor(spriteBatch); }


            // Render Map Obj Layer
            foreach (var tile in MapTiles)
            {
                if (MapTiles.Count == 0) { return; }

                // If tile has been rendered, add to the Visible Tiles list
                if (tile.Render(spriteBatch, (int)CameraX, (int)CameraY)) { if (HUD_SelectedLayer == 0) { VisibleTiles.Add(tile); } }

                // Render a little C letter if tile is an Colideable Tile
                if (tile.Colideable)
                {
                    spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), "C", new Vector2(tile.x * Global.TileSize, tile.y * Global.TileSize), Color.Red);
                }

                // If tile is the selected tile, do something special
                if (HUD_SelectedLayer != 0) { continue; }

                if (tile.index == Cursor_TileSelected)
                {
                    spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(tile.x * Global.TileSize, tile.y * Global.TileSize, Global.TileSize, Global.TileSize), Color.FromNonPremultiplied(0, 0, 0, 155));
                }

            }

            if (HUD_SelectedLayer == 0) { RenderTileCursor(spriteBatch); }

            // Render MapObjects
            foreach (var objTile in MapObjects)
            {
                if (MapObjects.Count == 0) { return; }

                // If tile has been rendered, add to the Visible Tiles list
                if (objTile.Render(spriteBatch, (int)CameraX, (int)CameraY)) { if (HUD_SelectedLayer == 1) { VisibleTiles.Add(objTile); } }


                // If tile is the selected tile, do something special
                if (HUD_SelectedLayer != 1) { continue; }

                if (objTile.index == Cursor_TileSelected)
                {
                    spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(objTile.x * Global.TileSize, objTile.y * Global.TileSize, Global.TileSize, Global.TileSize), Color.FromNonPremultiplied(0, 0, 0, 155));
                }


            }
            if (HUD_SelectedLayer == 1) { RenderTileCursor(spriteBatch); }

            spriteBatch.End();

        }

        private void RenderHUD(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            string InfosText = "...";

            // Draw Debug Test 1
            try
            {
                InfosText = "Visible Tiles: " + VisibleTiles.Count + "\n" +
                            "SelectedTile x:" + MapTiles[Cursor_TileSelected].x + "; y:" + MapTiles[Cursor_TileSelected].y + "; index:" + Cursor_TileSelected;

            }
            catch (ArgumentOutOfRangeException) { }

            spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), InfosText, new Vector2(HUD_X + 155, 15), Color.BlueViolet);

            // Render HUD Background
            spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(HUD_X, 0, HUD_Width, Global.WindowHeight), Color.FromNonPremultiplied(15, 12, 7, 155));

            // Render SelectedTile HUD
            spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), "Selected Tile: " + HUD_SelectedTileIndex, new Vector2(HUD_X + 20, 15), Color.White);
            spriteBatch.Draw(Sprites.GetSprite(HUD_SelectedTileTagPrevious), new Rectangle(HUD_X + 37, 45, 16, 16), Color.White);
            spriteBatch.Draw(Sprites.GetSprite(HUD_SelectedTileTag), new Rectangle(HUD_X + HUD_Width / 2 - 32 / 2, 30, 32, 32), Color.White);
            spriteBatch.Draw(Sprites.GetSprite(HUD_SelectedTileTagNext), new Rectangle(HUD_X + 97, 45, 16, 16), Color.White);

            // Render Tile Atributes AND EditorInfo HUD
            string TileObjIDDescription;

            try
            {
                TileObjIDDescription = Registry.ReadKeyValue("/editor/interation_id_description/" + HUD_SelectedTileAtribute_ObjectTileInterationID);
            }
            catch (Exception) { TileObjIDDescription = "null"; }

            string TileAtributesText = "Tile Atributes:\nColideable: " + HUD_SelectedTileAtribute_Colideable +
                                       "\nTileObjID: " + HUD_SelectedTileAtribute_ObjectTileInterationID + "\n" + TileObjIDDescription + "\n" +
                                       "\nSelectedMapLayer:\n" + Registry.ReadKeyValue("/editor/map_layer_" + HUD_SelectedLayer) + "\n\n" +
                                       "Tile Atributes:\n" + Utils.SplitIntoLines(HUD_SelectedTileAtributesString, ',');

            spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), TileAtributesText, new Vector2(HUD_X + 5, 70), Color.White);

            RenderDialog(spriteBatch);

            spriteBatch.End();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            RenderMap(spriteBatch);

            RenderHUD(spriteBatch);

            // Render Dialog
            if (DialogEnabled)
            {
                MessageDialog.Draw(spriteBatch);
            }


            spriteBatch.Begin();

            // Render Cursor
            spriteBatch.Draw(Sprites.GetSprite("/cursor.png"), new Rectangle(CursorX, CursorY, 17, 23), Color.White);

            spriteBatch.End();

        }

        Vector2 RenderDialog_DialogTitleFontSize = new Vector2(0, 0);

        public void RenderDialog(SpriteBatch spriteBatch)
        {
            if (!DialogEnabled) { return; }


        }

        public override void Initialize()
        {
            base.Initialize();

            ReloadTilesIndexes();

            DialogAction(2);

            UpdateTileIndex();

        }

        private void ReloadTilesIndexes()
        {
            Console.WriteLine("Reloading Tile Indexes...");
            MapTilesByID_String.Clear();
            MapTilesByID_Indexes.Clear();

            string[] MapTilesIndexes = Registry.ReadKeyValue("/maptile_by_index").Split(';');

            foreach (var item in MapTilesIndexes)
            {
                if (item.Length < 2)
                {
                    continue;
                }

                string[] MapTilesIndexesAguments = item.Split(',');

                MapTilesByID_String.Add(MapTilesIndexesAguments[1]);
                MapTilesByID_Indexes.Add(Convert.ToInt32(MapTilesIndexesAguments[0]));

                Console.WriteLine("TileIndex: string{" + MapTilesIndexesAguments[1] + "}, index{" + MapTilesIndexesAguments[0] + "}.");

            }

            Console.WriteLine("Operation Completed.");

        }

        //                 DefaultText = Registry.ReadKeyValue("editor/default_text_" + Dialog_Mode);


        public void ReloadMap(bool ReloadFromDialog = false, string DialogInput = "")
        {
            Console.WriteLine("Reloading map data...");
            MapTiles.Clear();
            MapObjects.Clear();
            MapBackground.Clear();
            VisibleTiles.Clear();

            if (ReloadFromDialog)
            {
                try
                {
                    string[] SplitedSize = DialogInput.Split('x');

                    MapWidth = Convert.ToInt32(SplitedSize[0]);
                    MapHeight = Convert.ToInt32(SplitedSize[1]);

                    if (MapWidth <= 16)
                    {
                        Console.WriteLine("Map Width can't be less than 16");
                        MapWidth = 16;
                    }

                    if (MapHeight <= 16)
                    {
                        Console.WriteLine("Map Height can't be less than 16");
                        MapHeight = 16;
                    }

                }
                catch (IndexOutOfRangeException)
                {
                    DialogMessage(Registry.ReadKeyValue("/editor/invalid_format"));
                    return;
                }

                catch (FormatException)
                {
                    DialogMessage(Registry.ReadKeyValue("/editor/invalid_format"));
                    return;
                }



            }
            else
            {
                MapWidth = Convert.ToInt32(Registry.ReadKeyValue("default/editor/map_width"));
                MapHeight = Convert.ToInt32(Registry.ReadKeyValue("default/editor/map_height"));

                Console.WriteLine("MapSize has been set to Default Size");
            }

            Global.TileSize = 32;
            CameraX = 50;
            CameraY = 50;

            // Report Map Size
            Console.WriteLine("MapSize is:\n" + "W:" + MapWidth + ", H:" + MapHeight);


            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    MapTiles.Add(new MapTile(x, y, new string[] { "sprite:" }, MapTiles.Count));
                    MapObjects.Add(new MapTile(x, y, new string[] { "sprite:" }, MapObjects.Count));
                    MapBackground.Add(new MapTile(x, y, new string[] { "sprite:" }, MapBackground.Count));

                }
            }

            MapHasBeenLoaded = true;
            Console.WriteLine("Done!");
        }




    }
}
