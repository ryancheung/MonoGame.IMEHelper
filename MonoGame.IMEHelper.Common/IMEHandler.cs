using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGame.IMEHelper
{
    public abstract class IMEHandler
    {
        private static IMEHandler _ImplInstance;
        public static IMEHandler ImplInstance
        {
            get
            {
                if (_ImplInstance == null)
                    throw new NotImplementedException("Muset set `IMEHandler.ImplInstance` to an implementation instance.");

                return _ImplInstance;
            }
            set
            {
                if (_ImplInstance != null)
                    return;

                _ImplInstance = value;
            }
        }

        /// <summary>
        /// Game Instance
        /// </summary>
        public Game GameInstance { get; private set; }
        public bool ShowDefaultIMEWindow { get; private set; }

        protected IMEHandler(Game game, bool showDefaultIMEWindow = false)
        {
            this.GameInstance = game;
            this.ShowDefaultIMEWindow = showDefaultIMEWindow;

            PlatformInitialize();
        }

        /// <summary>
        /// Platform specific initialization.
        /// </summary>
        public abstract void PlatformInitialize();

        /// <summary>
        /// Check if text composition is enabled currently.
        /// </summary>
        public abstract bool Enabled { get; protected set; }

        /// <summary>
        /// Enable the system IMM service to support composited character input.
        /// This should be called when you expect text input from a user and you support languages
        /// that require an IME (Input Method Editor).
        /// </summary>
        /// <seealso cref="EndTextComposition" />
        /// <seealso cref="TextComposition" />
        public abstract void StartTextComposition();

        /// <summary>
        /// Stop the system IMM service.
        /// </summary>
        /// <seealso cref="StartTextComposition" />
        /// <seealso cref="TextComposition" />
        public abstract void StopTextComposition();

        /// <summary>
        /// Use this function to set the rectangle used to type Unicode text inputs if IME supported.
        /// In SDL2, this method call gives the OS a hint for where to show the candidate text list,
        /// since the OS doesn't know where you want to draw the text you received via SDL_TEXTEDITING event.
        /// </summary>
        public virtual void SetTextInputRect(ref Rectangle rect) { }

        /// <summary>
        /// Array of the candidates
        /// </summary>
        public virtual string[] Candidates { get { return null; } }

        /// <summary>
        /// How many candidates should display per page
        /// </summary>
        public virtual uint CandidatesPageSize { get { return 0; } }

        /// <summary>
        /// First candidate index of current page
        /// </summary>
        public virtual uint CandidatesPageStart { get { return 0; } }

        /// <summary>
        /// The selected canddiate index
        /// </summary>
        public virtual uint CandidatesSelection { get { return 0; } }

        /// <summary>
        /// Composition String
        /// </summary>
        public virtual string Composition { get { return string.Empty; } }

        /// <summary>
        /// Composition Clause
        /// </summary>
        public virtual string CompositionClause { get { return string.Empty; } }

        /// <summary>
        /// Composition Reading String
        /// </summary>
        public virtual string CompositionRead { get { return string.Empty; } }

        /// <summary>
        /// Composition Reading Clause
        /// </summary>
        public virtual string CompositionReadClause { get { return string.Empty; } }

        /// <summary>
        /// Caret position of the composition
        /// </summary>
        public virtual int CompositionCursorPos { get { return 0; } }

        /// <summary>
        /// Result String
        /// </summary>
        public virtual string Result { get { return string.Empty; } }

        /// <summary>
        /// Result Clause
        /// </summary>
        public virtual string ResultClause { get { return string.Empty; } }

        /// <summary>
        /// Result Reading String
        /// </summary>
        public virtual string ResultRead { get { return string.Empty; } }

        /// <summary>
        /// Result Reading Clause
        /// </summary>
        public virtual string ResultReadClause { get { return string.Empty; } }

        /// <summary>
        /// Position Y of virtual keyboard, for mobile platforms has virtual keyboard.
        /// </summary>
        public virtual int VirtualKeyboardHeight { get { return 0; } }

        /// <summary>
        /// Get the composition attribute at character index.
        /// </summary>
        /// <param name="index">Character Index</param>
        /// <returns>Composition Attribute</returns>
        public virtual CompositionAttributes GetCompositionAttr(int charIndex)
        {
            return CompositionAttributes.Input;
        }

        /// <summary>
        /// Get the composition read attribute at character index.
        /// </summary>
        /// <param name="index">Character Index</param>
        /// <returns>Composition Attribute</returns>
        public virtual CompositionAttributes GetCompositionReadAttr(int charIndex)
        {
            return CompositionAttributes.Input;
        }

        /// <summary>
        /// Invoked when the IMM service is enabled and a character composition is changed.
        /// </summary>
        /// <seealso cref="StartTextComposition" />
        /// <seealso cref="EndTextComposition" />
        public event EventHandler<TextCompositionEventArgs> TextComposition;

        /// <summary>
        /// Trigger a text composition event.
        /// </summary>
        /// <param name="compString"></param>
        /// <param name="cursorPosition"></param>
        /// <param name="candidateList"></param>
        public virtual void OnTextComposition(string compString, int cursorPosition, CandidateList? candidateList = null)
        {
            if (TextComposition != null)
                TextComposition(this, new TextCompositionEventArgs(compString, cursorPosition, candidateList));
        }

        /// <summary>
        /// Invoked when the IMM service emit character input event.
        /// </summary>
        /// <seealso cref="StartTextComposition" />
        /// <seealso cref="EndTextComposition" />
        public event EventHandler<TextInputEventArgs> TextInput;

        /// <summary>
        /// Trigger a text input event.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="key"></param>
        public virtual void OnTextInput(char character, Keys key = Keys.None)
        {
            if (TextInput != null)
                TextInput(this, new TextInputEventArgs(character, key));
        }

        /// <summary>
        /// Redirect a sdl text input event.
        /// </summary>
        public virtual void OnTextInput(TextInputEventArgs args)
        {
            if (TextInput != null)
                TextInput(this, args);
        }

        public virtual void Update(GameTime gameTime)
        {

        }
    }
}
