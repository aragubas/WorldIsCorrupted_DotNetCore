using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WorldISCorrupted
{
    public class InputKeyArgument
    {
        public Keys KeyObj;
        public Buttons ThisGamePadButton;
        public string GamepadEq;

        public string ActionName = "";

        public InputKeyArgument(string pActionName, string pKeyboardEq, string pGamepadEq)
        {
            ActionName = pActionName;

            // Set KeyboardEq
            SetKeyboardEq(pKeyboardEq);

            // Set GamepadEq    
            SetGamepadEq(pGamepadEq);

            GamepadEq = pGamepadEq;

        }

        public void SetGamepadEq(string pGamepadEq)
        {
            pGamepadEq = pGamepadEq.ToUpper();

            switch (pGamepadEq)
            {
                case "A":
                    ThisGamePadButton = Buttons.A;
                    break;

                case "B":
                    ThisGamePadButton = Buttons.B;
                    break;

                case "X":
                    ThisGamePadButton = Buttons.X;
                    break;

                case "Y":
                    ThisGamePadButton = Buttons.Y;
                    break;

                case "DPAD_UP":
                    ThisGamePadButton = Buttons.DPadUp;
                    break;

                case "DPAD_DOWN":
                    ThisGamePadButton = Buttons.DPadDown;
                    break;

                case "DPAD_LEFT":
                    ThisGamePadButton = Buttons.DPadLeft;
                    break;

                case "DPAD_RIGHT":
                    ThisGamePadButton = Buttons.DPadRight;
                    break;

                case "BACK":
                    ThisGamePadButton = Buttons.Back;
                    break;

                case "HOME":
                    ThisGamePadButton = Buttons.BigButton;
                    break;

                case "LEFT_SHOULDER":
                    ThisGamePadButton = Buttons.LeftShoulder;
                    break;

                case "RIGHT_SHOULDER":
                    ThisGamePadButton = Buttons.RightShoulder;
                    break;

                case "LEFT_STICK_PRESS":
                    ThisGamePadButton = Buttons.LeftStick;
                    break;

                case "RIGHT_STICK_PRESS":
                    ThisGamePadButton = Buttons.RightStick;
                    break;

                case "START":
                    ThisGamePadButton = Buttons.Start;
                    break;

                case "LEFT_THUMB_UP":
                    ThisGamePadButton = Buttons.LeftThumbstickUp;
                    break;

                case "LEFT_THUMB_DOWN":
                    ThisGamePadButton = Buttons.LeftThumbstickDown;
                    break;

                case "LEFT_THUMB_LEFT":
                    ThisGamePadButton = Buttons.LeftThumbstickLeft;
                    break;

                case "LEFT_THUMB_RIGHT":
                    ThisGamePadButton = Buttons.LeftThumbstickRight;
                    break;

                case "RIGHT_THUMB_UP":
                    ThisGamePadButton = Buttons.RightThumbstickUp;
                    break;

                case "RIGHT_THUMB_DOWN":
                    ThisGamePadButton = Buttons.RightThumbstickDown;
                    break;

                case "RIGHT_THUMB_LEFT":
                    ThisGamePadButton = Buttons.RightThumbstickLeft;
                    break;

                case "RIGHT_THUMB_RIGHT":
                    ThisGamePadButton = Buttons.RightThumbstickRight;
                    break;

                default:
                    throw new Exception("Invalid GamepadKeyCode {" + pGamepadEq + "}");

            }

        }

        public void SetKeyboardEq(string pKeyboardEq)
        {
            pKeyboardEq = pKeyboardEq.ToUpper();

            switch (pKeyboardEq)
            {
                case "A":
                    KeyObj = Keys.A;
                    break;

                case "B":
                    KeyObj = Keys.B;
                    break;

                case "C":
                    KeyObj = Keys.C;
                    break;

                case "D":
                    KeyObj = Keys.D;
                    break;

                case "E":
                    KeyObj = Keys.E;
                    break;

                case "F":
                    KeyObj = Keys.F;
                    break;

                case "G":
                    KeyObj = Keys.G;
                    break;

                case "H":
                    KeyObj = Keys.H;
                    break;

                case "I":
                    KeyObj = Keys.I;
                    break;
                case "J":
                    KeyObj = Keys.J;
                    break;

                case "K":
                    KeyObj = Keys.K;
                    break;

                case "L":
                    KeyObj = Keys.L;
                    break;

                case "M":
                    KeyObj = Keys.M;
                    break;

                case "N":
                    KeyObj = Keys.N;
                    break;

                case "O":
                    KeyObj = Keys.O;
                    break;

                case "P":
                    KeyObj = Keys.P;
                    break;

                case "Q":
                    KeyObj = Keys.Q;
                    break;

                case "R":
                    KeyObj = Keys.R;
                    break;

                case "S":
                    KeyObj = Keys.S;
                    break;

                case "T":
                    KeyObj = Keys.T;
                    break;

                case "U":
                    KeyObj = Keys.U;
                    break;

                case "V":
                    KeyObj = Keys.V;
                    break;

                case "W":
                    KeyObj = Keys.W;
                    break;

                case "X":
                    KeyObj = Keys.X;
                    break;

                case "Y":
                    KeyObj = Keys.Y;
                    break;

                case "Z":
                    KeyObj = Keys.Z;
                    break;

                case "SPACE":
                    KeyObj = Keys.Space;
                    break;

                case "ENTER":
                    KeyObj = Keys.Enter;
                    break;

                case "L_SHIFT":
                    KeyObj = Keys.LeftShift;
                    break;

                case "R_SHIFT":
                    KeyObj = Keys.RightShift;
                    break;

                case "ESC":
                    KeyObj = Keys.Escape;
                    break;

                default:
                    throw new Exception("Invalid KeyCode {" + pKeyboardEq + "}");


            }

        }
    }


    public class GameInput
    {
        public static int CurrentInputMode = 0;
        public static KeyboardState oldState;
        public static GamePadState oldPadState;
        public static GamePadCapabilities newCapabilities;
        private static int LastInputMode = -1;
        public static List<InputKeyArgument> InputKeyArguments = new List<InputKeyArgument>();
        public static List<string> InputKeyArguments_key = new List<string>();
        public static List<string> Waxer = new List<string>();
        public static List<string> PressedWaxer = new List<string>();

        // State Animation Change Variables
        #region StateChange Variables
        private static int StateChangeStartDelay = 0;
        private static int StateChangeStartDelayMax = 60;
        private static bool StateChangeAnimation = false;
        private static bool StateChangeAnimMode = false;
        private static bool StateChangeAnimDelayEnd = false;
        private static int StateChangeAnimDelayEndValue = 0;
        private static int StateChangeAnimDelayEndValueMax = 50;
        private static int StateChangeAnimVal = 0;
        private static int StateChangeAnimValAdder = 0;
        private static int StateChangeAnimValMax = 150;
        #endregion

        // InputViwerAnim Variables
        #region InputViwer Changer
        public static bool InputViwer_AnimationEnabled;
        private static int InputViwer_AnimationVal = 34;
        private static int InputViwer_AnimationValAdder;
        private static int InputViwer_AnimationValMax = 34;
        private static int InputViwer_AnimationMode;


        #endregion

        public static void Initialize()
        {
            System.Console.WriteLine("Loading Input contexts...");
            string InputContextFile = Registry.ReadKeyValue("/input_context");

            foreach (var Line in InputContextFile.Split(";"))
            {
                if (Line.Length < 3 || Line.StartsWith("#"))
                {
                    System.Console.WriteLine("Skipped blank line");
                    continue;

                }

                string[] LineSplit = Line.Split(",");

                string ActionName = LineSplit[0];
                string ActionKeyboard = LineSplit[1];
                string ActionJoypad = LineSplit[2].TrimEnd();

                System.Console.WriteLine("Loaded input context {" + ActionName + "}");

                InputKeyArguments_key.Add(ActionName);
                InputKeyArgument NewWax = new InputKeyArgument(ActionName, ActionKeyboard, ActionJoypad);
                InputKeyArguments.Add(NewWax);

            }



        }

        public static void InputViwer_Define(string InputArg)
        {
            int WaxIndex = Waxer.IndexOf(InputArg);
            if (WaxIndex == -1)
            {
                Waxer.Add(InputArg);
            }
        }
        private static string LastInputContextFindError = "";
        public static InputKeyArgument GetInputKeyArg(string InputContext)
        {
            int InpContxtIndex = InputKeyArguments_key.IndexOf(InputContext);

            if (InpContxtIndex == -1)
            {
                if (LastInputContextFindError != InputContext)
                {
                    LastInputContextFindError = InputContext;
                    System.Console.WriteLine("Cannot find InputContext : {" + InputContext + "}");
                }
                return null;
            }

            return InputKeyArguments[InpContxtIndex];
        }

        public static void SetInputMode(int NewInputMode)
        {
            if (NewInputMode > 1)
            {
                throw new Exception("Invalid input mode.");
            }

            CurrentInputMode = NewInputMode;
            if (LastInputMode != NewInputMode)
            {
                LastInputMode = NewInputMode;
                StateChangeAnimation = true;
                Sound.PlaySound("hud/im_change");
            }


        }

        public static bool GetInputState(string KeyactionArg, bool KeyDown = false, bool ShowInViewer = false)
        {
            if (ShowInViewer) { InputViwer_Define(KeyactionArg); };

            InputKeyArgument Wax = GetInputKeyArg(KeyactionArg);

            if (Wax == null)
            {
                return false;
            }

            // Check for Keyboard
            switch (CurrentInputMode)
            {
                case 0:
                    if (KeyDown)
                    {
                        return Utils.CheckKeyDown(oldState, Keyboard.GetState(), Wax.KeyObj);
                    }
                    return Utils.CheckKeyUp(oldState, Keyboard.GetState(), Wax.KeyObj);

                case 1:
                    if (KeyDown)
                    {
                        if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Wax.ThisGamePadButton) && oldPadState.IsButtonDown(Wax.ThisGamePadButton))
                        {
                            PressedWaxer.Add(KeyactionArg);
                            return true;
                        }
                        return false;
                    }
                    if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Wax.ThisGamePadButton) && oldPadState.IsButtonUp(Wax.ThisGamePadButton))
                    {
                        PressedWaxer.Add(KeyactionArg);
                        return true;
                    };
                    return false;

            }

            return false;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            if (StateChangeAnimation || StateChangeAnimDelayEnd)
            {
                spriteBatch.Draw(Sprites.GetSprite("/input_mode/" + CurrentInputMode + ".png"), new Vector2(spriteBatch.GraphicsDevice.Viewport.Width - StateChangeAnimVal, spriteBatch.GraphicsDevice.Viewport.Height - 170), color: Color.FromNonPremultiplied(255, 255, 255, 255 - -StateChangeAnimVal));

            }

            if (CurrentInputMode != 1) { spriteBatch.End(); return; }

            spriteBatch.End();

            DrawIconViewer(spriteBatch);

        }

        private static void DrawIconViewer(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            //string AllWax = "";
            int WaxWax = -1;
            Vector2 Position = new Vector2(0, 0);

            foreach (string Wax in Waxer)
            {
                WaxWax += 1;

                int ShowerIndex = InputKeyArguments_key.IndexOf(Wax);
                if (ShowerIndex == -1) { WaxWax -= 1; continue; }
                InputKeyArgument Equivalent = InputKeyArguments[ShowerIndex];
                Position.X = 5 + WaxWax * 32;
                Position.Y = spriteBatch.GraphicsDevice.Viewport.Height - InputViwer_AnimationVal;
                int Opacity = 150;
                int TextOffset = 10;

                if (PressedWaxer.IndexOf(Wax) != -1) { Opacity = 255; TextOffset = 7; }

                spriteBatch.Draw(Sprites.GetSprite("/input_mode/gamepad_indicator/" + Equivalent.GamepadEq + ".png"), Position, color: Color.FromNonPremultiplied(255, 255, 255, Opacity));

                if (Registry.KeyExists("/gamepad_actions/" + Wax))
                {
                    string ActionWax = Registry.ReadKeyValue("/gamepad_actions/" + Wax);

                    spriteBatch.DrawString(Game1.Reference.Content.Load<SpriteFont>("default"), ActionWax, new Vector2(Position.X, Position.Y - TextOffset), Color.FromNonPremultiplied(230 - TextOffset, Opacity, Opacity, Opacity + 50), -0.7f, Vector2.Zero, 1f, SpriteEffects.None, default);

                }

            }
            spriteBatch.End();

            Waxer.Clear();
            PressedWaxer.Clear();
        }
        public static void UpdateStateChangeAnim()
        {
            if (StateChangeAnimation)
            {
                if (!StateChangeAnimMode)
                {
                    // Add delay to start of animation
                    if (StateChangeStartDelay <= StateChangeStartDelayMax)
                    {
                        StateChangeStartDelay += 1;

                    }
                    else
                    {
                        StateChangeAnimValAdder += 1;
                        StateChangeAnimVal += StateChangeAnimValAdder;

                    }

                    if (StateChangeAnimVal >= StateChangeAnimValMax)
                    {
                        StateChangeAnimVal = StateChangeAnimValMax;
                        StateChangeAnimMode = true;
                        StateChangeAnimation = false;
                        StateChangeAnimDelayEnd = true;
                    }
                }
                else
                {
                    StateChangeAnimValAdder += 1;
                    StateChangeAnimVal -= StateChangeAnimValAdder;

                    if (StateChangeAnimVal <= 0)
                    {
                        StateChangeAnimVal = 0;
                        StateChangeAnimMode = false;
                        StateChangeAnimation = false;
                        StateChangeAnimValAdder = 0;
                        StateChangeStartDelay = 0;

                    }

                }


            }

            // DelayEnd animation
            if (StateChangeAnimDelayEnd)
            {
                StateChangeAnimDelayEndValue += 1;

                if (StateChangeAnimDelayEndValue >= StateChangeAnimDelayEndValueMax)
                {
                    StateChangeAnimation = true;
                    StateChangeAnimDelayEnd = false;
                    StateChangeAnimDelayEndValue = 0;
                }
            }
        }

        public static void UpdateInputViwerAnim()
        {
            if (InputViwer_AnimationEnabled)
            {
                if (InputViwer_AnimationMode == 0)
                {
                    InputViwer_AnimationValAdder += 1;
                    InputViwer_AnimationVal += InputViwer_AnimationValAdder;

                    if (InputViwer_AnimationVal >= InputViwer_AnimationValMax)
                    {
                        InputViwer_AnimationVal = InputViwer_AnimationValMax;
                        InputViwer_AnimationEnabled = false;
                        InputViwer_AnimationMode += 1;
                    }

                }
                else if (InputViwer_AnimationMode == 1)
                {
                    InputViwer_AnimationValAdder += 1;
                    InputViwer_AnimationVal -= InputViwer_AnimationValAdder;

                    if (InputViwer_AnimationVal <= -InputViwer_AnimationValMax * 2)
                    {
                        InputViwer_AnimationVal = -InputViwer_AnimationValMax * 2;
                        InputViwer_AnimationEnabled = false;
                        InputViwer_AnimationMode -= 1;
                        InputViwer_AnimationValAdder = 0;
                    }

                }

            }
        }
        public static void Update()
        {
            UpdateStateChangeAnim();
            UpdateInputViwerAnim();

            // Get GamePad Capabilities
            newCapabilities = GamePad.GetCapabilities(PlayerIndex.One);

            // Check if any controller is connected
            if (newCapabilities.IsConnected)
            {
                SetInputMode(1);
                oldPadState = GamePad.GetState(PlayerIndex.One);
                return;
            }

            if (CurrentInputMode == 0)
            {
                oldState = Keyboard.GetState();
                return;
            }

            // Check if controller has been disconnected
            if (!newCapabilities.IsConnected)
            {
                SetInputMode(0);
                return;
            }


        }

    }
}