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
using WorldISCorrupted.Taiyou;
using WorldISCorrupted.GameLogic.Screens.Game;
using WorldISCorrupted.GameLogic;
namespace WorldISCorrupted.GameLogic.GameObjects
{
    public class ScriptTrigger : GameObject
    {
        string EventName = "";
        string EventScript = "";
        public int ID = 0;

        public ScriptTrigger(string pScriptName, string pEventName, bool pIsColideable, string pColideableSprite, string pID, bool pActivateActionKey, int pX, int pY, MapRenderer mapRenderer)
        {
            Console.WriteLine("Creating Script Trigger ; agrs received:");

            try
            {
                pEventName = pScriptName;
                EventScript = pEventName;
                bool Colideable = pIsColideable;
                ID = Convert.ToInt32(pID);
                string ColideableSprite = pColideableSprite;

                string InitScriptName = EventScript + "_init";
                string InitEventName = pEventName + "_init";

                Event.RegisterEvent(pEventName, EventScript);
                Event.RegisterEvent(InitEventName, InitScriptName);

                if (Colideable)
                {
                    Console.WriteLine("ScriptTrigger is colideable.");
                    Console.WriteLine("Using sprite [" + ColideableSprite + "]");
                    int posX = pX / Global.TileSize;
                    int posY = pY / Global.TileSize;

                    Console.WriteLine("Tile located at: " + posX + ";" + posY);
                    // 1;14;sprite:/grass_dark.png,colideable:False,obj_tile_id:0
                    // "sprite:" + ColideableSprite + ",obj_tile_id:2"
                    System.Console.WriteLine(pActivateActionKey);
                    MapTile NewlyCreatedMapTile = new MapTile(posX, posY, new string[] { "sprite:" + ColideableSprite, "obj_tile_id:2", "call_script_when_colide:" + pEventName, "action_key:" + pActivateActionKey }, mapRenderer.AdditionalColideableTiles.Count);

                    mapRenderer.AdditionalColideableTiles.Add(NewlyCreatedMapTile);

                    Console.WriteLine("Added tile to MapObj layer.");

                }


                Event.TriggerEvent(InitEventName);

            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Error while loading ScriptTile\nNo arguments has been provided or insuficient arguments has been provided.");
            }


        }

        public override void Update()
        {
            base.Update();


        }



    }
}
