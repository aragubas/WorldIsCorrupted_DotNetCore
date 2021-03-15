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

namespace WorldISCorrupted.GameLogic.GameObjects
{
    public class LightPoint : GameObject
    {
        Screens.Game.Main RootScreen;
        public int PosX;
        public int PosY;
        public int Radius;
        private string LightShape;
        public Color LightColor;

        public LightPoint(Screens.Game.Main pRootScreen, int pLightPosX, int pLightPosY, int pLightRadius,
                            string pLightShape)
        {
            RootScreen = pRootScreen;
            PosX = pLightPosX;
            PosY = pLightPosY;
            Radius = pLightRadius;
            SetLightShape(pLightShape);
            LightColor = Color.Black;

            System.Console.WriteLine("New LightPoint at:");
            System.Console.WriteLine(pLightPosX + " , " + pLightPosY);

        }

        public void SetLightShape(string pLightShape)
        {
            LightShape = Registry.ReadKeyValue("/light_shape/" + pLightShape);

        }


        public override void Draw(SpriteBatch spriteBatch, Matrix CameraMatrix)
        {
            base.Draw(spriteBatch, CameraMatrix);


            spriteBatch.Draw(Sprites.GetSprite(LightShape), new Rectangle(PosX - Radius, PosY - Radius, Radius * 2, Radius * 2), LightColor);

        }


    }


}