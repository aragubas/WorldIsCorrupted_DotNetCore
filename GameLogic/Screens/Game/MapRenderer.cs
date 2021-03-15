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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WorldISCorrupted.Taiyou;
using WorldISCorrupted.GameLogic.GameObjects;

namespace WorldISCorrupted.GameLogic.Screens.Game
{
    public class MapRenderer
    {
        public List<MapTile> MapObjsLayer = new List<MapTile>();
        public List<MapTile> MapBackgroundLayer = new List<MapTile>();
        public List<MapTile> ColideableTiles = new List<MapTile>();
        public List<MapTile> AdditionalColideableTiles = new List<MapTile>();
        public List<MapTile> LightTiles = new List<MapTile>();


        // Camera Position
        public int CameraX = 0;
        public int CameraY = 0;

        public int MapWidth = 0;
        public int MapHeight = 0;


        public MapRenderer(string[] pMapData, string[] pMapBGData)
        {
            // Map File
            foreach (var line in pMapData)
            {
                if (line.Length < 2) { continue; }
                // Split Line arguments
                string[] LineArgs = line.Split(';');
                int TileX = Convert.ToInt32(LineArgs[0]);
                int TileY = Convert.ToInt32(LineArgs[1]);
                string[] TileProperties = LineArgs[2].Split(',');

                MapTile ThisTile = new MapTile(TileX, TileY, TileProperties, MapObjsLayer.Count);

                MapObjsLayer.Add(ThisTile);

            }

            // Map Background
            foreach (var line in pMapBGData)
            {
                if (line.Length < 2) { continue; }
                // Split Line arguments
                string[] LineArgs = line.Split(';');
                int TileX = Convert.ToInt32(LineArgs[0]);
                int TileY = Convert.ToInt32(LineArgs[1]);
                string[] TileProperties = LineArgs[2].Split(',');

                MapTile ThisTile = new MapTile(TileX, TileY, TileProperties, MapObjsLayer.Count);

                MapBackgroundLayer.Add(ThisTile);

            }


        }

        public void SetCameraPos(int pX, int pY)
        {
            CameraX = pX;
            CameraY = pY;
        }

        public void RenderBackgroundLayer(SpriteBatch spriteBatch)
        {
            // Render MapBackground
            foreach (var tile in MapBackgroundLayer)
            {
                // If tile has been rendered, add to the Visible Tiles list
                tile.Render(spriteBatch, CameraX, CameraY);

            }

        }

        public void RenderBackgroundObjsLayer(SpriteBatch spriteBatch)
        {
            // Render MapBackgroundobjs
            ColideableTiles.Clear();

            foreach (var tile in MapObjsLayer)
            {
                if (tile.Render(spriteBatch, CameraX, CameraY)) { if (tile.Colideable) { ColideableTiles.Add(tile); } }

            }

            foreach (var tile in AdditionalColideableTiles)
            {
                if (tile.Render(spriteBatch, CameraX, CameraY)) { ColideableTiles.Add(tile); }
            }

        }

        private MapTile GetTileAt(List<MapTile> TileMap, int[] Position)
        {
            foreach (var tile in TileMap)
            {
                if (tile.x == Position[0] && tile.y == Position[1])
                {
                    return tile;
                }
            }

            throw new Exception("Tile not found.");
        }

        public void RenderLightMap(SpriteBatch spriteBatch, List<LightPoint> LightPoints, Matrix CameraTransMatrix)
        {

            foreach (var point in LightPoints)
            {
                point.Draw(spriteBatch, CameraTransMatrix);
            }


        }

    }
}
