using Cos.Engine.Sprite.Enemy._Superclass;
using Cos.Engine.Sprite.Enemy.Peon;
using Cos.Library.Mathematics;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static Cos.Library.CosConstants;

namespace Cos.Engine.Manager
{
    /// <summary>
    /// Handles keyboard input and keeps track of key-press states.
    /// </summary>
    public class InputManager
    {
        private readonly EngineCore _engine;
        private readonly Dictionary<CosPlayerKey, float> _playerKeyStates = new();
        private readonly Dictionary<Key, bool> _allKeyStates = new();

        public DirectInput DxInput { get; private set; }
        public Keyboard DxKeyboard { get; private set; }

        /// <summary>
        /// Contains a list of keys that have been pressed and then released (cycled).
        /// Must enable via a call to CollectDetailedKeyInformation().
        /// </summary>
        public List<Key> CycledKeys { get; private set; } = new();

        /// <summary>
        /// Contains a list of all keys that are currently pressed down.
        /// Must enable via a call to CollectDetailedKeyInformation().
        /// </summary>
        public List<Key> DepressedKeys { get; private set; } = new();

        //Controller controller;
        //Gamepad gamepad;

        public InputManager(EngineCore engine)
        {
            _engine = engine;

            DxInput = new();
            DxKeyboard = new Keyboard(DxInput);
            DxKeyboard.Acquire();
        }

        ~InputManager()
        {
            DxInput.Dispose();
            DxKeyboard.Dispose();
        }

        public bool IsModifierKey(Key key)
        {
            return key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.LeftControl || key == Key.RightControl ||
                   key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LeftWindowsKey || key == Key.RightWindowsKey;
        }

        public void Snapshot()
        {
            if (_engine.Display.IsDrawingSurfaceFocused == false)
            {
                //We do this so that I can have more than one instance open on the same computer 
                //  at a time without the keyboard commands to one affecting the other.
                //_playerKeyStates.Clear();
                //return;
            }

            var keyboardState = DxKeyboard.GetCurrentState();

            _engine.Input.KeyStateChangedHard(CosPlayerKey.StrafeLeft, keyboardState.IsPressed(Key.Left));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.StrafeRight, keyboardState.IsPressed(Key.Right));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.Forward, keyboardState.IsPressed(Key.W));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.Reverse, keyboardState.IsPressed(Key.S));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.RotateCounterClockwise, keyboardState.IsPressed(Key.A));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.RotateClockwise, keyboardState.IsPressed(Key.D));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.SpeedBoost, keyboardState.IsPressed(Key.LeftShift));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.SwitchWeaponLeft, keyboardState.IsPressed(Key.Q));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.SwitchWeaponRight, keyboardState.IsPressed(Key.E));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.PrimaryFire, keyboardState.IsPressed(Key.Space));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.SecondaryFire, keyboardState.IsPressed(Key.RightControl));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.Left, keyboardState.IsPressed(Key.Left));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.Right, keyboardState.IsPressed(Key.Right));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.Up, keyboardState.IsPressed(Key.Up));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.Down, keyboardState.IsPressed(Key.Down));

            _engine.Input.KeyStateChangedHard(CosPlayerKey.Enter, keyboardState.IsPressed(Key.Return));
            _engine.Input.KeyStateChangedHard(CosPlayerKey.Escape, keyboardState.IsPressed(Key.Escape));
        }

        /// <summary>
        /// Returns the percentage of a key that is pressed. This is for gamepad analog and triggers.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetAnalogValue(CosPlayerKey key)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                return _playerKeyStates[key];
            }

            return 0;
        }

        /// <summary>
        /// Returns true or false depending on whether the applied key amount is zero or non-zero.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public bool IsKeyPressed(CosPlayerKey key)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                return (_playerKeyStates[key] != 0);
            }

            return false;
        }

        /// <summary>
        /// Allows the containing window to tell the engine about key press events.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void KeyStateChangedHard(CosPlayerKey key, bool state)
        {
            if (_playerKeyStates.ContainsKey(key))
            {
                _playerKeyStates[key] = state ? 1 : 0;
            }
            else
            {
                _playerKeyStates.Add(key, state ? 1 : 0);
            }
        }

        public void HandleSingleKeyPress(Keys key)
        {
            if (key == Keys.P)
            {
                _engine.TogglePause();
            }
            else if (key == Keys.F1)
            {
                if (_engine.Sprites.OfType<SpriteEnemyBase>().Count() > 0)
                {
                    _engine.Sprites.OfType<SpriteEnemyBase>()[0].Explode();
                }
            }
            else if (key == Keys.F2)
            {
            }
            else if (key == Keys.F5)
            {
                _engine.Sprites.CreateFragmentsOf(_engine.Player.Sprite);
            }
            else if (key == Keys.F6)
            {
                _engine.Sprites.Particles.ParticleCloud(500, _engine.Player.Sprite);
            }
            else if (key == Keys.F7)
            {
                var enemy = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyScav>();
                enemy.Orientation = CosVector.FromDegrees(-90);
                enemy.Location = new CosVector(1000, 1000);
            }
        }
    }
}
