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
namespace WorldISCorrupted.UtilsObjects
{
    public class AnimationController
    {
        bool Enabled = false;
        bool AutoEnable = false;
        int Value = 0;
        int AddCurve = 0;
        int Speed = 0;
        bool ResetAnimationCurve = false;
        public bool Ended = false;
        int State = 0;
        int MaxValue = 0;
        int MinValue = 0;

        /// <summary>
        /// Creates an Animation Controller Object
        /// </summary>
        /// <param name="pMaxValue">Maximun Animation Value</param>
        /// <param name="pMinValue">Minimun Animation Value</param>
        /// <param name="pAnimationSpeed">Animation Speed</param>
        /// <param name="pResetAnimationCurve">If set to <c>true</c> Reset the animation curve</param>
        /// <param name="pAutoEnable">If set to <c>true</c>Re-Enabled animation when ending.</param>
        /// <param name="InitialState">Initial state</param>
        public AnimationController(int pMaxValue, int pMinValue, int pAnimationSpeed,
            bool pResetAnimationCurve, bool pAutoEnable = false, int InitialState = 0)
        {
            Enabled = true;
            State = InitialState;
            MaxValue = pMaxValue;
            MinValue = pMinValue;
            AutoEnable = pAutoEnable;
            ResetAnimationCurve = pResetAnimationCurve;
            Speed = pAnimationSpeed;
            Value = MinValue;

        }

        public void SetMaxValue(int Value)
        {
            MaxValue = Value;
        }

        public void SetMinValue(int Value)
        {
            MinValue = Value;
        }

        public void SetEnabled(bool Value)
        {
            Enabled = Value;
        }

        public void ForceState(int StateID)
        {
            Enabled = true;
            State = StateID;
        }

        public int GetValue()
        {
            return Value;
        }

        public int GetState()
        {
            return State;
        }

        public void Update()
        {
            if (!Enabled) { return; }

            switch (State)
            {
                case 0:
                    AddCurve += Speed;
                    Value += AddCurve;


                    if (Value >= MaxValue)
                    {
                        Value = MaxValue;
                        State += 1;
                        Ended = false;

                        if (!AutoEnable) { Enabled = false; }
                        if (!ResetAnimationCurve) { AddCurve = 0; }

                    }
                    break;

                case 1:
                    AddCurve += Speed;
                    Value -= AddCurve;

                    if (Value <= MinValue)
                    {
                        Value = MinValue;
                        State -= 1;
                        Ended = true;

                        if (!AutoEnable) { Enabled = false; }
                        if (!ResetAnimationCurve) { AddCurve = 0; }

                    }
                    break;


            }

        }

    }
}
