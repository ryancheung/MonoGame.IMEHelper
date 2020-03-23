using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MonoGame.IMEHelper.WindowsDX.Test
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        DynamicSpriteFont font1;

        IMEHandler imeHandler;
        KeyboardState lastState;
        Texture2D whitePixel;
        string inputContent = string.Empty;

        const int UnicodeSimplifiedChineseMin = 0x4E00;
        const int UnicodeSimplifiedChineseMax = 0x9FA5;
        const string DefaultChar = "?";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            imeHandler = new WinFormsIMEHandler(this, false);

            imeHandler.TextInput += (s, e) =>
            {
                switch ((int)e.Character)
                {
                    case 8:
                        if (inputContent.Length > 0)
                            inputContent = inputContent.Remove(inputContent.Length - 1, 1);
                        break;
                    case 27:
                    case 13:
                        inputContent = "";
                        break;
                    default:
                        if (e.Character > UnicodeSimplifiedChineseMax)
                            inputContent += DefaultChar;
                        else
                            inputContent += e.Character;
                        break;
                }

                inputContent = inputContent.Trim();
            };

            imeHandler.TextComposition += (o, e) =>
            {
                var rect = new Rectangle(10, 50, 0, 0);
                imeHandler.SetTextInputRect(ref rect);
            };

            IsMouseVisible = true;

            base.Initialize();
        }

        public static byte[] GetManifestResourceStream(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resNames = assembly.GetManifestResourceNames();

            var actualResourceName = resNames.First(r => r.EndsWith(resourceName));

            var stream = assembly.GetManifestResourceStream(actualResourceName);
            byte[] ret = new byte[stream.Length];
            stream.Read(ret, 0, (int)stream.Length);

            return ret;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font1 = DynamicSpriteFont.FromTtf(GetManifestResourceStream("simsun.ttf"), 30);

            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.F1) && lastState.IsKeyUp(Keys.F1))
            {
                if (imeHandler.Enabled)
                    imeHandler.StopTextComposition();
                else
                    imeHandler.StartTextComposition();
            }

            lastState = ks;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            Vector2 len = font1.MeasureString(inputContent);

            spriteBatch.DrawString(font1, "按下 F1 启用 / 停用 IME", new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(font1, inputContent, new Vector2(10, 50), Color.White);

            Vector2 drawPos = new Vector2(15 + len.X, 50);
            Vector2 measStr = new Vector2(0, font1.MeasureString("|").Y);
            Color compColor = Color.White;

            if (imeHandler.CompositionCursorPos == 0)
                spriteBatch.Draw(whitePixel, new Rectangle((int)drawPos.X, (int)drawPos.Y, 1, (int)measStr.Y), Color.White);

            for (int i = 0; i < imeHandler.Composition.Length; i++)
            {
                string val = imeHandler.Composition[i].ToString();
                switch (imeHandler.GetCompositionAttr(i))
                {
                    case CompositionAttributes.Converted: compColor = Color.LightGreen; break;
                    case CompositionAttributes.FixedConverted: compColor = Color.Gray; break;
                    case CompositionAttributes.Input: compColor = Color.Orange; break;
                    case CompositionAttributes.InputError: compColor = Color.Red; break;
                    case CompositionAttributes.TargetConverted: compColor = Color.Yellow; break;
                    case CompositionAttributes.TargetNotConverted: compColor = Color.SkyBlue; break;
                }

                if (val[0] > UnicodeSimplifiedChineseMax)
                    val = DefaultChar;

                spriteBatch.DrawString(font1, val, drawPos, compColor);

                measStr = font1.MeasureString(val);
                drawPos += new Vector2(measStr.X, 0);

                if ((i + 1) == imeHandler.CompositionCursorPos)
                    spriteBatch.Draw(whitePixel, new Rectangle((int)drawPos.X, (int)drawPos.Y, 1, (int)measStr.Y), Color.White);
            }

            for (uint i = imeHandler.CandidatesPageStart;
                i < Math.Min(imeHandler.CandidatesPageStart + imeHandler.CandidatesPageSize, imeHandler.Candidates.Length);
                i++)
            {
                if (imeHandler.Candidates[i][0] > UnicodeSimplifiedChineseMax)
                    imeHandler.Candidates[i] = DefaultChar;

                try
                {
                    spriteBatch.DrawString(font1,
                        String.Format("{0}.{1}", i + 1 - imeHandler.CandidatesPageStart, imeHandler.Candidates[i]),
                        new Vector2(15 + len.X, 25 + 50 + (i - imeHandler.CandidatesPageStart) * 20),
                        i == imeHandler.CandidatesSelection ? Color.Yellow : Color.White);
                }
                catch
                {
                    Trace.WriteLine($"Candidate string {imeHandler.Candidates[i]} has invalid codepoint in current font.");
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
