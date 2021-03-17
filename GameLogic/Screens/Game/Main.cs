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
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WorldISCorrupted.GameLogic.GameObjects;
using Microsoft.Xna.Framework;
using WorldISCorrupted.Taiyou;

namespace WorldISCorrupted.GameLogic.Screens.Game
{
    public class Main : GameScreen
    {
        public List<GameObject> MapObjects = new List<GameObject>();
        List<string> MapProperties = new List<string>();
        List<string> MapPropertiesValues = new List<string>();
        List<LightPoint> LightPoints = new List<LightPoint>();
        int WorldIlumination = 255;
        Player CurrentPlayer;
        RenderTarget2D darkness = null;
        Matrix CameraTransform;

        public MapRenderer MapView;
        public int MapWidth;
        public int MapHeight;
        public bool Reload;
        public int CameraX;
        public int CameraY;
        public int CameraCenterX;
        public int CameraCenterY;
        public int CameraCenterWidth;
        public int CameraCenterHeight;
        public bool CameraCenter;
        public float DebugTestVar;
        private KeyboardState oldState;
        public bool MapLoadingError = false;
        private int ReloadMapDelay = 0;
        public bool Initialized;
        public int GravityEffectness = 3;

        public bool CameraBoundary_Enabled;


        public override void Update()
        {
            base.Update();

            KeyboardState newState = Keyboard.GetState();

            // Reload Map Key
            ReloadMapKey(newState);

            // Change to Map Editor Screen
            if (Utils.CheckKeyDown(oldState, newState, Keys.F5))
            {
                MapObjects.Clear();
                Taiyou.Global.Reload();
                ScreenSelector.SetCurrentScreen(1);

            }

            if (Utils.CheckKeyDown(oldState, newState, Keys.H))
            {
                DebugTestVar += 0.01f;
                Console.WriteLine("DEBUG VARIABLE VALUE:" + DebugTestVar);

            }
            if (Utils.CheckKeyUp(oldState, newState, Keys.N))
            {
                DebugTestVar += 0.01f;
                Console.WriteLine("DEBUG VARIABLE VALUE:" + DebugTestVar);

            }
            if (Utils.CheckKeyDown(oldState, newState, Keys.J))
            {
                DebugTestVar -= 0.01f;
                Console.WriteLine("DEBUG VARIABLE VALUE:" + DebugTestVar);

            }
            if (Utils.CheckKeyUp(oldState, newState, Keys.M))
            {
                DebugTestVar -= 0.05f;
                Console.WriteLine("DEBUG VARIABLE VALUE:" + DebugTestVar);

            }
            if (Utils.CheckKeyUp(oldState, newState, Keys.K))
            {
                DebugTestVar = 1.0f;
                Console.WriteLine("DEBUG VARIABLE VALUE:" + DebugTestVar);

            }
            if (Utils.CheckKeyUp(oldState, newState, Keys.L))
            {
                DebugTestVar = 0;
                Console.WriteLine("DEBUG VARIABLE VALUE:" + DebugTestVar);

            }


            // If map failed to load, return
            if (MapLoadingError)
            {
                oldState = newState;
                return;
            }

            // Set this game instance
            TaiyouGlobal.CurrentGameInstance = this;

            // If map needs to be reloaded
            if (Global.ForceReloadMap)
            {
                Initialize();
                return;
            }

            UpdateCamera();

            foreach (var obj in MapObjects)
            {
                obj.CameraX = CameraX;
                obj.CameraY = CameraY;
                obj.CurrentMapRenderer = MapView;


                obj.Update();
            }

            MapView.CameraX = CameraX;
            MapView.CameraY = CameraY;

            oldState = newState;

            // Update Player
            if (!Initialized) { return; }
            CurrentPlayer.Update();

        }

        private void UpdateCamera()
        {
            // Set the Camera to the MapViewCamera
            CameraX = MapView.CameraX;
            CameraY = MapView.CameraY;

            // Set Camera to the Center of Object if requested
            if (CameraCenter)
            {
                CameraX = Global.WindowWidth / 2 - CameraCenterWidth / 2 - CameraCenterX;
                CameraY = Global.WindowHeight / 2 - CameraCenterHeight / 2 - CameraCenterY;

            }

            // Get Map Size in Pixels
            int CorrectMapWidth = -MapWidth * Global.TileSize;
            int CorrectMapHeight = -MapHeight * Global.TileSize;

            // Get Viewport Size
            Vector2 ViewportSize = new Vector2(Game1.Reference.GraphicsDevice.Viewport.Width, Game1.Reference.GraphicsDevice.Viewport.Height);

            // Check if map has width to Camera Width Clipping
            if (Math.Abs(CorrectMapWidth) > ViewportSize.X)
            {
                // Check if camera is not ouside the map
                if (CameraX > 0)
                {
                    CameraX = 0;
                }

                if (CameraX - ViewportSize.X < CorrectMapWidth)
                {
                    CameraX = CorrectMapWidth + (int)ViewportSize.X;
                }

            }

            // Check if map was height to Camera Height Clipping
            if (Math.Abs(CorrectMapHeight) > ViewportSize.Y)
            {
                // Check if camera is not ouside the map
                if (CameraY > 0)
                {
                    CameraY = 0;
                }

                if (CameraY - ViewportSize.Y < CorrectMapHeight)
                {
                    CameraY = CorrectMapHeight + (int)ViewportSize.Y;
                }

            }


        }

        private void ReloadMapKey(KeyboardState newState)
        {
            ReloadMapDelay += 1;

            if (ReloadMapDelay > 25)
            {
                ReloadMapDelay = 25;
            }

            if (newState.IsKeyDown(Keys.LeftShift) && oldState.IsKeyDown(Keys.R))
            {
                if (ReloadMapDelay == 25)
                {
                    ReloadMapDelay = 0;
                    Global.ForceReloadMap = true;

                }

            }

        }

        public void SetCameraToCenter(int X, int Y, int Width, int Height)
        {
            CameraCenter = true;

            CameraCenterX = X;
            CameraCenterY = Y;
            CameraCenterWidth = Width;
            CameraCenterHeight = Height;


        }

        public void ReloadAll()
        {
            Taiyou.Global.Reload();
            Initialize();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Global.ForceReloadMap && !Initialized) { return; }

            if (MapLoadingError)
            {
                spriteBatch.Begin();

                spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), "Error while loading map.", new Vector2(5, 5), Color.DarkRed);

                spriteBatch.End();

                base.Draw(spriteBatch);

                return;
            }

            // Set camera transformation matrix
            CameraTransform = Matrix.CreateTranslation(new Vector3(CameraX, CameraY, 0));


            // Create Render Target when it was not created
            if (darkness == null)
            {
                darkness = new RenderTarget2D(spriteBatch.GraphicsDevice, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height);
            }
            // Set Up Lightning
            if (Global.Setting_LightningEnabled)
            {
                // Set Render Target and Clear Screen
                spriteBatch.GraphicsDevice.SetRenderTarget(darkness);
                spriteBatch.GraphicsDevice.Clear(new Color(0, 0, 0, WorldIlumination));

                // Render Light Points
                spriteBatch.Begin(blendState: Global.BlendState_LightningBlendState, transformMatrix: CameraTransform);
                MapView.RenderLightMap(spriteBatch, LightPoints, CameraTransform);
                spriteBatch.End();

                // Set RenderTarget back to Null
                spriteBatch.GraphicsDevice.SetRenderTarget(null);

            }

            // Render Map Layers
            spriteBatch.Begin(transformMatrix: CameraTransform);
            MapView.RenderBackgroundLayer(spriteBatch);
            MapView.RenderBackgroundObjsLayer(spriteBatch);
            spriteBatch.End();

            // Render Objects
            spriteBatch.Begin();
            foreach (var obj in MapObjects)
            {
                obj.Draw(spriteBatch, CameraTransform);
            }
            spriteBatch.End();

            // Render Player Sprite
            spriteBatch.Begin(transformMatrix: CameraTransform);
            CurrentPlayer.Draw(spriteBatch, CameraTransform);
            spriteBatch.End();

            // Render Light Layer 
            if (Global.Setting_LightningEnabled)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(darkness, Vector2.Zero, Color.White);
                spriteBatch.End();

            }

            // Render Player HUD
            spriteBatch.Begin();
            CurrentPlayer.DrawHUD(spriteBatch, CameraTransform);
            spriteBatch.End();


            base.Draw(spriteBatch);

        }


        public override void Initialize()
        {
            string pMapFileToLoad;
            Taiyou.Global.Reload();
            CurrentPlayer = null;

            if (!Global.ForceReloadMap)
            {
                pMapFileToLoad = Registry.ReadKeyValue("/defaultMap");

            }
            else
            {
                pMapFileToLoad = Global.CurrentMapName;

            }

            // Check if MapFile Exists
            if (!File.Exists(pMapFileToLoad)) { MapLoadingError = true; Console.WriteLine("Cannot find Map file {" + pMapFileToLoad + "}."); return; }
            MapLoadingError = false;


            try
            {
                string[] MapFileRead = File.ReadAllText(pMapFileToLoad).Split('|');
                string[] MapBackgroundFile = MapFileRead[0].Split(new[] { '\r', '\n' });
                string[] MapDataFile = MapFileRead[1].Split(new[] { '\r', '\n' });
                string[] MapObjFile = MapFileRead[2].Split(new[] { '\r', '\n' });
                string[] MapMetadataFile = MapFileRead[3].Split(';');

                foreach (var MetaLine in MapMetadataFile)
                {
                    try
                    {
                        string[] SplitedArgs = MetaLine.Split(':');

                        if (SplitedArgs[0] == "width")
                        {
                            MapWidth = Convert.ToInt32(SplitedArgs[1]);
                        }

                        if (SplitedArgs[0] == "height")
                        {
                            MapHeight = Convert.ToInt32(SplitedArgs[1]);
                        }

                        if (SplitedArgs[0] == "world_ilumination")
                        {
                            WorldIlumination = Convert.ToInt32(SplitedArgs[1]);
                        }


                        if (SplitedArgs[0] == "end")
                        {
                            break;
                        }

                        MapProperties.Add(SplitedArgs[0]);
                        MapPropertiesValues.Add(SplitedArgs[1]);

                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Invalid data in Metadata");
                    }

                }


                MapView = new MapRenderer(MapDataFile, MapBackgroundFile);
                ReloadObjMap(MapObjFile);


                Global.ForceReloadMap = false;

            }
            catch (IndexOutOfRangeException)
            {
                MapLoadingError = true;
                Console.WriteLine("Error while loading MapFile [" + pMapFileToLoad + "].\nMap is invalid.");
            }

            Global.CurrentMapName = pMapFileToLoad;

            Initialized = true;
            base.Initialize();
        }

        public void ReloadObjMap(string[] pMapObjFile)
        {
            MapObjects.Clear();
            LightPoints.Clear();

            foreach (var obj in pMapObjFile)
            {
                if (obj.Length < 2) { continue; }

                string[] LineArgs = obj.Split(';');
                int TileX = Convert.ToInt32(LineArgs[0]) * Global.TileSize;
                int TileY = Convert.ToInt32(LineArgs[1]) * Global.TileSize;
                string[] TileProperties = LineArgs[2].Split(',');

                MapTile ThisTile = new MapTile(TileX, TileY, TileProperties, MapObjects.Count);

                if (ThisTile.ObjectTileIDType != -1)
                {
                    if (ThisTile.ObjectTileIDType == 1)
                    {
                        CurrentPlayer = new Player(TileX, TileY, this);

                    }
                    if (ThisTile.ObjectTileIDType == 2)
                    {
                        //ScriptTrigger Stta = new ScriptTrigger(ThisTile.TileArgs, TileX, TileY, MapView);
                        string ParsedScriptName = "";
                        string ParsedEventName = "";
                        string ParsedID = "";
                        bool ParsedColideable = false;
                        bool ActivateViaActionKey = false;
                        string ParsedColideableSprite = "";

                        for (int i = 0; i < TileProperties.Length; i++)
                        {
                            string[] Parameter = TileProperties[i].Split(':');

                            switch (Parameter[0])
                            {
                                case "script_name":
                                    ParsedScriptName = Parameter[1];
                                    break;

                                case "event_name":
                                    ParsedEventName = Parameter[1];
                                    break;

                                case "script_id":
                                    ParsedID = Parameter[1];
                                    break;

                                case "script_colideable":
                                    ParsedColideable = true;
                                    break;

                                case "colideable_sprite":
                                    ParsedColideableSprite = Parameter[1];
                                    break;

                                case "action_key":
                                    ActivateViaActionKey = true;
                                    break;

                            }
                        }

                        ScriptTrigger Stta = new ScriptTrigger(ParsedScriptName, ParsedEventName, ParsedColideable, ParsedColideableSprite, ParsedID, ActivateViaActionKey, TileX, TileY, MapView);


                        MapObjects.Add(Stta);

                    }
                    if (ThisTile.ObjectTileIDType == 4)
                    {
                        LightPoint LightPointToAdd = new LightPoint(this, TileX, TileY, 10, "default");
                        int ParsedLightRadius = 100;
                        string ParsedLightShape = "default";

                        for (int i = 0; i < TileProperties.Length; i++)
                        {
                            string[] Parameter = TileProperties[i].Split(':');

                            switch (Parameter[0])
                            {
                                case "light_radius":
                                    ParsedLightRadius = Convert.ToInt32(Parameter[1]);
                                    break;

                                case "light_shape":
                                    ParsedLightShape = Parameter[1];
                                    break;

                            }
                        }
                        LightPointToAdd.Radius = ParsedLightRadius;
                        LightPointToAdd.SetLightShape(ParsedLightShape);

                        LightPoints.Add(LightPointToAdd);

                    }

                }
            }

        }

    }

}
