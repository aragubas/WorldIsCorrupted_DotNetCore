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
using Microsoft.Xna.Framework.Input;
using WorldISCorrupted.GameLogic;
using WorldISCorrupted.GameLogic.GameObjects.Weapons;
using WorldISCorrupted.Taiyou;

namespace WorldISCorrupted.GameLogic.GameObjects
{
    public class Player : GameObject
    {
        // Public Values
        public int X;
        public int Y;
        public int Width = 16;
        public int Height = 24;
        public Screens.Game.Main rootScreen;
        public int Health;
        public Weapon_Object WeaponInHand;
        public bool PlayerIsDead = false;
        public bool MovmentEnabled = true;

        private int initialY;
        private int initialX;
        private int velocity = 0;
        private int velocity_max = 10;
        private int velVelocityAceleration = 1;
        private int LastMovSide = -1;
        private int DefaultHealth = 100;

        private int AsphyciatedTimer;
        private int AsphyciatedTimerActivate = 50;
        private int AsphyciatedPenalty = 1;

        private int velDefaultVelocityAceleration = 1;

        private bool Gravity = true;
        private int GravityForce = 0;
        private int GravityPull = 1;
        private string HUD_Icon_state = "/hud_icon_set/default";

        private bool DamageAnimationEnabled;
        private float DamageAnimationAmmount;


        private int JumpAcel;
        private int JumpHeight = 20;
        private int JumpTarget = -9999;
        private int JumpSourceY = -9999;
        private int JumpVector = -1;


        private bool JumpEnabled;
        private bool IsAfficiated;

        private Rectangle Colision;
        private KeyboardState oldState;
        private List<Rectangle> ColisorsRect = new List<Rectangle>();

        private Rectangle newPos = new Rectangle(0, 0, 0, 0);

        private bool HasRoomToGoRight = true;
        private bool HasRoomToGoLeft = true;
        private bool CanJump = true;


        List<MapTile> VisibleTiles;

        public Player(int pX, int pY, Screens.Game.Main pRootScreen)
        {
            X = pX;
            Y = pY;

            initialX = pX;
            initialY = pY;

            Health = DefaultHealth;

            rootScreen = pRootScreen;

            // Set the jump vector
            SetJumpVector(3);

            // Set Up Camera Boundary
            pRootScreen.CameraBoundary_Enabled = true;
            CurrentMapRenderer = rootScreen.MapView;

            TaiyouGlobal.PlayerObj = this;

        }

        public override void Update()
        {
            base.Update();
            if (!rootScreen.Initialized) { return; }

            KeyboardState newState = Keyboard.GetState();
            // Update player physics
            UpdatePhysics(CurrentMapRenderer.ColideableTiles);
            UpdateDamageAnimation();

            // Update Indicator Icon
            SetIndicatorIconState();

            if (Y >= rootScreen.MapHeight * Global.TileSize ||
                X <= -Width || X >= rootScreen.MapWidth * Global.TileSize)
            {
                Health = 0;
                PlayerIsDead = true;
            }
            UpdateGravity();

            if (!PlayerIsDead)
            {

                // Update camera position
                rootScreen.SetCameraToCenter(X, Y, Width, Height);

                UpdateAsphyciatedTimer();

                if (Health < 0)
                {
                    Health = 0;
                    PlayerIsDead = true;
                }

                // Player Jump & Moviment
                UpdatePlayerJump();
                Move(newState);


            }
            else // Player is Dead
            {

            }

            newPos = new Rectangle(X, Y, Width, Height);

            oldState = newState;
        }

        private void UpdatePhysics(List<MapTile> pVisibleTiles)
        {
            VisibleTiles = pVisibleTiles;

            HasRoomToGoRight = true;
            HasRoomToGoLeft = true;
            CanJump = true;

            // Bottom Coliding
            foreach (var tile in pVisibleTiles)
            {
                Rectangle colisor = new Rectangle(tile.x * Global.TileSize, tile.y * Global.TileSize, Global.TileSize, Global.TileSize);

                if (!tile.Colideable) { continue; }


                // Bottom Colision
                if (colisor.Top <= newPos.Bottom + GravityForce && colisor.Right >= newPos.Right + velocity && colisor.Left <= newPos.Right + velocity && colisor.Bottom >= newPos.Top - GravityForce)
                {
                    if (colisor.Intersects(newPos) && colisor.Top > newPos.Bottom)
                    {
                        newPos = new Rectangle(newPos.X, colisor.Top - newPos.Height - GravityForce, newPos.Width, newPos.Height);
                    }

                    Gravity = false;
                    break;
                }

                if (!JumpEnabled) { Gravity = true; }
            }

            // Activate Touch Func Tiles
            foreach (var tile in pVisibleTiles)
            {
                Rectangle colisor = new Rectangle(tile.x * Global.TileSize, tile.y * Global.TileSize, Global.TileSize, Global.TileSize);

                if (!tile.Colideable)
                {
                    if (tile.ObjectTileIDType == 2)
                    {
                        if (colisor.Intersects(newPos))
                        {
                            tile.ExecuteFunction();
                        }
                        else
                        {
                            tile.TileColisionActionToggle = false;
                        }

                    }

                    continue;
                }


                bool ColidedTop = false;

                if (colisor.Intersects(newPos))
                {
                    // Top Coliding
                    if (newPos.Top < colisor.Bottom && colisor.Right >= newPos.Right && colisor.Left <= newPos.Left && !ColidedTop)
                    {
                        Gravity = true;
                        CanJump = false;
                        JumpEnabled = false;
                        ColidedTop = true;

                        newPos = new Rectangle(newPos.X, colisor.Bottom, newPos.Width, newPos.Height);


                    }

                    if (colisor.Right - velocity <= newPos.Right)
                    {
                        HasRoomToGoLeft = false;


                        newPos = new Rectangle(colisor.Right - velocity, newPos.Y, newPos.Width, newPos.Height);

                    }

                    if (colisor.Left + velocity >= newPos.Left)
                    {
                        HasRoomToGoRight = false;

                        newPos = new Rectangle(colisor.Left + velocity - Width, newPos.Y, newPos.Width, newPos.Height);

                    }



                }

            }

            Colision = newPos;

        }

        private void UpdateGravity()
        {
            // Apply Gravity Force
            if (Gravity)
            {
                GravityForce += GravityPull;
                if (GravityForce >= Height)
                {
                    GravityForce = Height;

                }
                Y += GravityForce;


                JumpEnabled = false;


            }
            else { GravityForce = 0; }

        }


        private void SetJumpVector(int Value)
        {
            JumpVector = Global.TileSize + Height * Value;
        }

        private void UpdatePlayerJump()
        {
            if (!MovmentEnabled)
            {
                return;
            }

            if (JumpEnabled)
            {
                JumpAcel -= 1;
                //Jump += GravityForce + 1;

                if (JumpTarget == -9999)
                {
                    JumpTarget = Y - JumpVector;
                }

                if (JumpSourceY == -9999)
                {
                    JumpSourceY = Y;
                }


                //JumpAcel += 5;
                //Gravity = false;
                JumpHeight = GravityForce + Height / 2;
                Y -= JumpHeight;


                if (Y <= JumpTarget || Y >= JumpSourceY)
                {
                    JumpEnabled = false;
                    JumpTarget = -9999;
                    JumpAcel = 0;
                    JumpSourceY = -9999;

                }

            }

        }


        private void UpdateAsphyciatedTimer()
        {
            if (!IsAfficiated) { return; }

            AsphyciatedTimer += 1;

            if (AsphyciatedTimer >= AsphyciatedTimerActivate)
            {
                AsphyciatedTimer = 0;
                Health -= AsphyciatedPenalty;

                PlayerDamage();
            }


        }

        public void PlayerDamage()
        {
            DamageAnimationAmmount = 35;
            DamageAnimationEnabled = true;
            Sound.PlaySound("player/hit");

        }

        private void UpdateDamageAnimation()
        {
            if (!DamageAnimationEnabled) { return; }

            DamageAnimationAmmount -= 1;

            if (DamageAnimationAmmount <= 0)
            {
                DamageAnimationAmmount = 0;
                DamageAnimationEnabled = false;
            }

        }

        private void SetIndicatorIconState()
        {
            HUD_Icon_state = "/hud_icon_set/0";

            if (IsAfficiated)
            {
                HUD_Icon_state = "/hud_icon_set/1";
                return;
            }

            if (DamageAnimationEnabled)
            {
                HUD_Icon_state = "/hud_icon_set/2";

            }

            if (PlayerIsDead)
            {
                HUD_Icon_state = "/hud_icon_set/3";

            }


        }

        private void Move(KeyboardState newState)
        {
            if (!MovmentEnabled)
            {
                return;
            }

            // Show inputs on Input Viwer
            GameInput.InputViwer_Define("MOVE_D");
            GameInput.InputViwer_Define("MOVE_A");
            GameInput.InputViwer_Define("MOVE_JUMP");

            if (GameInput.GetInputState("SHOW_VIWER"))
            {
                GameInput.InputViwer_AnimationEnabled = true;
            }

            if (GameInput.GetInputState("MOVE_D", true) && HasRoomToGoRight)
            {
                if (LastMovSide != 0) { velVelocityAceleration = velDefaultVelocityAceleration; velocity = 0; }
                LastMovSide = 0;

                velocity += velVelocityAceleration;
                X += velocity;

            }

            else if (GameInput.GetInputState("MOVE_A", true) && HasRoomToGoLeft)
            {
                if (LastMovSide != 1) { velVelocityAceleration = velDefaultVelocityAceleration; velocity = 0; }
                LastMovSide = 1;

                velocity += velVelocityAceleration;
                X -= velocity;

            }
            else
            {
                velocity = -1;
                if (velocity < 0) { velocity = 0; }
                velVelocityAceleration = velDefaultVelocityAceleration;

            }


            if (newState.IsKeyDown(Keys.V) && oldState.IsKeyDown(Keys.V))
            {
                IsAfficiated = true;
            }

            if (newState.IsKeyDown(Keys.B) && oldState.IsKeyDown(Keys.B))
            {
                PlayerIsDead = true;
            }


            // Player jump key
            if (GameInput.GetInputState("MOVE_JUMP", false) && CanJump)
            {
                JumpEnabled = true;
            }


            // Limit Velocity
            if (velocity >= velocity_max)
            {
                velocity = velocity_max;

            }


        }

        public void DrawHUD(SpriteBatch spriteBatch, Matrix CameraTransform)
        {
            // Render State Icon
            spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue(HUD_Icon_state)), new Rectangle(5, 5 + Math.Min(5, (int)DamageAnimationAmmount / 2), 64, 64), Color.FromNonPremultiplied(255 - (int)DamageAnimationAmmount, 255, 255 - (int)DamageAnimationAmmount, 255));

            // Render Health Text
            string InfosText = "Health: " + Health;
            spriteBatch.DrawString(Fonts.GetFont("/PressStart2P.ttf", 13), InfosText, new Vector2(75, 6), Color.Black);
            spriteBatch.DrawString(Fonts.GetFont("/PressStart2P.ttf", 13), InfosText, new Vector2(74, 5), Color.White);

        }

        public override void Draw(SpriteBatch spriteBatch, Matrix CameraMatrix)
        {
            base.Draw(spriteBatch, CameraMatrix);
            if (!rootScreen.Initialized) { return; }

            // Begin sprite batch with matrix transformation
            spriteBatch.Draw(Sprites.GetSprite(Registry.ReadKeyValue("default/base_sprite")), new Rectangle(Colision.X, Colision.Y, Width, Height), Color.Red);

        }

    }
}
