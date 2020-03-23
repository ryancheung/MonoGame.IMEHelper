using System;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.IMEHelper
{
    public class TextInputEventArgs : EventArgs
    {
        public TextInputEventArgs(char character, Keys key = Keys.None)
        {
            Character = character;
            Key = key;
        }

        public readonly char Character;
        public readonly Keys Key;
    }

}
