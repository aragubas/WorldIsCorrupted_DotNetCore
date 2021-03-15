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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WorldISCorrupted.GameLogic
{
    public class MapTile
    {
        public int x;
        public int y;
        public int index;
        public string TileSprite;
        public string TileArgs = "NULL";
        public bool Colideable;
        public string[] atributes;
        public int ObjectTileIDType;
        Color ColorBlend;
        public float OpacityAnimValue = 0;
        public bool OpacityAnimHappened;
        bool CallEventWhenColide = false;
        string EventToCallWhenColide;
        public bool TileColisionActionToggle = false;
        public bool TileScriptActionKey = false;

        public MapTile(int X, int Y, string[] TileData, int pIndex)
        {
            x = X;
            y = Y;
            ColorBlend = Color.White;
            index = pIndex;

            atributes = TileData;

            UpdateTileAtributes();

        }

        public void ExecuteFunction()
        {
            if (Taiyou.TaiyouGlobal.PlayerObj.PlayerIsDead) { return; }
            if (CallEventWhenColide)
            {
                if (!TileColisionActionToggle && !TileScriptActionKey)
                {
                    TileColisionActionToggle = true;
                    Taiyou.Event.TriggerEvent(EventToCallWhenColide);
                }

                if (TileScriptActionKey)
                {
                    if (GameInput.GetInputState("ACTION_KEY", ShowInViewer: true))
                    {
                        Taiyou.Event.TriggerEvent(EventToCallWhenColide);

                    }
                }
            }
        }

        public void UpdateTileAtributes()
        {
            for (int i = 0; i < atributes.Length; i++)
            {
                string[] Parameter = atributes[i].Split(':');

                switch (Parameter[0])
                {
                    // If sprite, grab the next parameter
                    case "sprite":
                        TileSprite = Parameter[1];
                        break;

                    case "colideable":
                        Colideable = Convert.ToBoolean(Parameter[1]);
                        break;

                    case "color":
                        string[] ColorArguments = Parameter[1].Split('|');

                        ColorBlend = Color.FromNonPremultiplied(Convert.ToInt32(ColorArguments[0]), Convert.ToInt32(ColorArguments[1]), Convert.ToInt32(ColorArguments[2]), Convert.ToInt32(ColorArguments[3]));
                        break;

                    case "obj_tile_id":
                        ObjectTileIDType = Convert.ToInt32(Parameter[1]);
                        break;


                    case "args":
                        TileArgs = Parameter[1];
                        break;


                    case "call_script_when_colide":
                        CallEventWhenColide = true;
                        EventToCallWhenColide = Parameter[1];
                        break;

                    case "action_key":
                        TileScriptActionKey = Convert.ToBoolean(Parameter[1]);
                        break;

                }

            }

        }

        private void ResetOpacity()
        {
            OpacityAnimHappened = false;
            OpacityAnimValue = 0;
        }

        private Color UpdateOpacity()
        {
            if (!OpacityAnimHappened)
            {
                OpacityAnimValue += Math.Max(10, index / 64);

                if (OpacityAnimValue >= 255)
                {
                    OpacityAnimHappened = true;
                }

                return Color.FromNonPremultiplied(ColorBlend.R, ColorBlend.G, ColorBlend.B, (int)OpacityAnimValue);
            }

            return ColorBlend;
        }

        public bool TileIsOnScreen(int CameraX, int CameraY)
        {
            int SpriteRelativeX = CameraX + x * Global.TileSize;
            int SpriteRelativeY = CameraY + y * Global.TileSize;

            if (SpriteRelativeX >= Global.WindowWidth) { ResetOpacity(); return false; }
            if (SpriteRelativeX <= -Global.TileSize) { ResetOpacity(); return false; }
            if (SpriteRelativeY >= Global.WindowHeight) { ResetOpacity(); return false; }
            if (SpriteRelativeY <= -Global.TileSize) { ResetOpacity(); return false; }

            return true;
        }

        public bool Render(SpriteBatch spriteBatch, int CameraX, int CameraY)
        {
            int SpriteRelativeX = CameraX + x * Global.TileSize;
            int SpriteRelativeY = CameraY + y * Global.TileSize;

            if (!TileIsOnScreen(CameraX, CameraY)) { return false; }
            if (TileSprite == "") { return true; }

            Color spriteColor = UpdateOpacity();

            spriteBatch.Draw(Sprites.GetSprite(TileSprite), new Rectangle(x * Global.TileSize, y * Global.TileSize, Global.TileSize, Global.TileSize), spriteColor);
            return true;
        }

    }
}
