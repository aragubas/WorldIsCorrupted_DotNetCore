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
using WorldISCorrupted.GameLogic.GameObjects;
namespace WorldISCorrupted.Taiyou.Command
{
    public class SetPlayerState : TaiyouCommand
    {
        public SetPlayerState(string[] pArguments, string pScriptCaller, TaiyouLine pRootTaiyouLine)
        {
            OriginalArguments = pArguments;

            ScriptCaller = pScriptCaller;
            Title = "SetPlayerState";
            RootTaiyouLine = pRootTaiyouLine;

        }

        bool FunctionInited = false;
        string NewPlayerState;

        public override int Call()
        {
            if (!FunctionInited)
            {
                FunctionInited = true;

                string[] Arguments = ReplaceVarLiterals();

                NewPlayerState = GetArgument(Arguments, 0);

            }

            switch (NewPlayerState)
            {
                case "DEAD":
                    TaiyouGlobal.PlayerObj.Health = 0;
                    TaiyouGlobal.PlayerObj.PlayerIsDead = true;
                    break;

                case "DAMAGE":
                    TaiyouGlobal.PlayerObj.Health -= Convert.ToInt32(GetArgument(ReplaceVarLiterals(), 1));
                    TaiyouGlobal.PlayerObj.PlayerDamage();
                    break;

                case "DISABLE_CONTROL":
                    TaiyouGlobal.PlayerObj.MovmentEnabled = false;
                    break;

                case "ENABLE_CONTROL":
                    TaiyouGlobal.PlayerObj.MovmentEnabled = true;
                    break;

            }

            return 0;
        }

    }
}
