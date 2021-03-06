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
using System.Text.RegularExpressions;
using System.Threading;

namespace WorldISCorrupted.Taiyou
{
    public class TaiyouScript
    {

        private string scriptName;
        private List<TaiyouLine> Code;
        public bool HaltExecution = false;

        public TaiyouScript(string ScriptName, bool DirectCode = false, List<TaiyouLine> taiyouLines = null)
        {
            scriptName = ScriptName;

            // Find the script on Script List
            if (DirectCode)
            {
                SetCode(taiyouLines);

                return;
            }
            int ScriptIndex = Global.LoadedTaiyouScripts.IndexOf(ScriptName);
            if (ScriptIndex == -1) { throw new EntryPointNotFoundException("the Taiyou Script (" + ScriptName + ") does not exist."); }

            SetCode(Global.LoadedTaiyouScripts_Data[ScriptIndex]);

        }

        private void SetCode(List<TaiyouLine> Lines)
        {
            Code = Lines;

            foreach (var tiy in Code)
            {
                tiy.ParentScript = this;
            }

        }

        public int Interpret()
        {
            if (HaltExecution) { HaltExecution = false; }


            foreach (var line in Code)
            {
                if (HaltExecution)
                {
                    return 2;
                }

                int ReturnCode = line.call();

                if (ReturnCode != 0) { return ReturnCode; }
            }

            return 0;
        }



    }
}