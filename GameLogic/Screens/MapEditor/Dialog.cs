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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WorldISCorrupted;
using WorldISCorrupted.GameLogic.TextBox;
using Microsoft.Xna.Framework.Input;
using WorldISCorrupted.UtilsObjects;

namespace WorldISCorrupted.GameLogic.Screens.MapEditor
{
    public class Dialog
    {
        int DialogType = 0;
        bool FirstUpdateCycle = false;
        List<string> Arguments = new List<string>();

        Vector2 ViewportSize;

        TextBox.TextBox DialogTextBox;
        Rectangle DialogRectangle;
        public string DialogOutput;
        public bool DialogFinished;
        int DialogOpacity = 0;
        bool DialogRectangleSet = false;

        // Dialog Animation Behavoiur
        int RectDesiredX = 0;
        int RectDesiredY = 0;
        int RectDesiredWidth = 0;
        int RectDesiredHeight = 0;

        List<AnimationController> animationControllers = new List<AnimationController>();
        AnimationController DesiredXActuator;
        AnimationController DesiredYActuator;
        AnimationController DesiredWidthActuator;
        AnimationController DesiredHeightActuator;
        AnimationController DesiredOpacityActuator;

        bool DialogExitTrigger = false;
        bool AnimationControllersEnd = false;
        int EndedCount = 0;


        KeyboardState oldState;

        public Dialog(int pDialogType, List<string> pArguments)
        {
            DialogType = pDialogType;
            Arguments = pArguments;
        }

        public void Update()
        {
            UpdateRectangle();

            switch (DialogType)
            {
                case 0:
                    UpdateDialogModeZero();
                    break;

                case 1:
                    UpdateDialogModeOne();
                    break;


            }


            FirstUpdateCycle = true;
        }

        private void UpdateRectangle()
        {
            // Update Mode Zero Rectangle

            if (!DialogRectangleSet)
            {
                DialogRectangleSet = true;

                Vector2 FontSize = Game1.Reference.Content.Load<SpriteFont>("default").MeasureString(Arguments[0]);
                ViewportSize = new Vector2(Game1.Reference.GraphicsDevice.Viewport.Width, Game1.Reference.GraphicsDevice.Viewport.Height);
                Vector2 DialogSize = new Vector2(FontSize.X + 10, FontSize.Y + 10);

                RectDesiredX = (int)ViewportSize.X / 2 - (int)DialogSize.X / 2;
                RectDesiredY = (int)ViewportSize.Y / 2 - (int)DialogSize.Y / 2;
                RectDesiredWidth = (int)DialogSize.X;
                RectDesiredHeight = (int)DialogSize.Y;

                int AnimationSpeed = 15;

                DesiredXActuator = new AnimationController(RectDesiredX, RectDesiredX / 2, AnimationSpeed, false);
                DesiredYActuator = new AnimationController(RectDesiredY, 0, AnimationSpeed, false);
                DesiredWidthActuator = new AnimationController(RectDesiredWidth, 0, AnimationSpeed, false);
                DesiredHeightActuator = new AnimationController(RectDesiredHeight, 0, AnimationSpeed, false);
                DesiredOpacityActuator = new AnimationController(255, 0, AnimationSpeed, false);

                animationControllers.Add(DesiredYActuator);
                animationControllers.Add(DesiredWidthActuator);
                animationControllers.Add(DesiredHeightActuator);
                animationControllers.Add(DesiredOpacityActuator);


            }

            foreach (var Controller in animationControllers)
            {
                Controller.Update();
            }

            DialogRectangle = new Rectangle(RectDesiredX, DesiredYActuator.GetValue(), DesiredWidthActuator.GetValue(), DesiredHeightActuator.GetValue());
            DialogOpacity = DesiredOpacityActuator.GetValue();

            if (DialogExitTrigger)
            {
                if (!AnimationControllersEnd)
                {
                    foreach (var Controller in animationControllers)
                    {
                        if (Controller.Ended)
                        {
                            EndedCount += 1;
                            Controller.SetEnabled(false);
                            break;
                        }
                        else
                        {
                            Controller.ForceState(1);
                        }
                    }

                    if (EndedCount >= animationControllers.Count)
                    {
                        AnimationControllersEnd = true;
                    }

                }
                else
                {
                    DialogFinished = true;
                }



            }

        }

        private void UpdateDialogModeZero()
        {

            // Check for Keypress
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape) && !DialogExitTrigger)
            {
                DialogExitTrigger = true;

            }

            oldState = newState;
        }

        private void UpdateDialogModeOne()
        {

            if (!FirstUpdateCycle)
            {
                string Text = Arguments[1];

                int Width = (int)ViewportSize.X - 100;
                DialogTextBox = new TextBox.TextBox(new Rectangle((int)ViewportSize.X / 2 - Width / 2, (int)ViewportSize.Y / 2 - 16, Width, 24), 128, Text, Game1.Reference.GraphicsDevice, Game1.Reference.Content.Load<SpriteFont>("arial_12"), Color.White, Color.FromNonPremultiplied(255, 5, 5, 50), 20);

            }

            DialogTextBox.Update();

            DialogTextBox.Renderer.Color = Color.FromNonPremultiplied(255, 255, 255, DialogOpacity);

            Rectangle CursorCoision = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);

            DialogTextBox.Active = CursorCoision.Intersects(DialogTextBox.Area);

            DialogTextBox.Area = new Rectangle(DialogTextBox.Area.X, DesiredYActuator.GetValue() + 15, DialogTextBox.Area.Width, DialogTextBox.Area.Height);

            // Check for Keypress
            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape) && !DialogExitTrigger)
            {
                DialogExitTrigger = true;
                DialogOutput = "NONE";
            }

            if (newState.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter) && !DialogExitTrigger)
            {
                DialogExitTrigger = true;
                DialogOutput = DialogTextBox.Text.String;
            }


            oldState = newState;
        }


        private void DrawDialogBackground(SpriteBatch spriteBatch)
        {
            // Draw Background
            spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(0, 0, (int)ViewportSize.X, (int)ViewportSize.Y), Color.FromNonPremultiplied(5, 5, 5, DialogOpacity - 55));

        }

        private void DrawDialogModeZero(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), Arguments[0], new Vector2(DialogRectangle.X + 5, DialogRectangle.Y + 5), Color.FromNonPremultiplied(255, 255, 255, DialogOpacity));
        }

        private void DrawDialogModeOne(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), Arguments[0], new Vector2(DialogRectangle.X + 5, DialogRectangle.Y - 20), Color.FromNonPremultiplied(255, 255, 255, DialogOpacity));

            // DialogTextBox.Area
            spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), DialogTextBox.Area, Color.FromNonPremultiplied(5, 5, 5, DialogOpacity - 100));

            DialogTextBox.Draw(spriteBatch);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!FirstUpdateCycle) { return; }
            spriteBatch.Begin();
            DrawDialogBackground(spriteBatch);

            switch (DialogType)
            {
                case 0:
                    DrawDialogModeZero(spriteBatch);
                    break;

                case 1:
                    DrawDialogModeOne(spriteBatch);
                    break;


            }


            spriteBatch.End();
        }

    }
}
