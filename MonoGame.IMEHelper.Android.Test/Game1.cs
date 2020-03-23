using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpriteFontPlus;
using System.Reflection;
using Microsoft.Xna.Framework.Input.Touch;
using System.Linq;

namespace MonoGame.IMEHelper.Android.Test
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        DynamicSpriteFont _font;

        IMEHandler imeHandler;
        Texture2D whitePixel;
        string inputContent = string.Empty;

        const int UnicodeSimplifiedChineseMin = 0x4E00;
        const int UnicodeSimplifiedChineseMax = 0x9FA5;
        const string DefaultChar = "?";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            imeHandler = new AndroidIMEHandler(this, true);

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

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = DynamicSpriteFont.FromTtf(GetManifestResourceStream("simsun.ttf"), 50);

            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new Color[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (TouchLocationState.Pressed == touchLocation.State)
                {
                    if (imeHandler.Enabled)
                        imeHandler.StopTextComposition();
                    else
                        imeHandler.StartTextComposition();
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            Vector2 len = _font.MeasureString(inputContent.Trim());

            _spriteBatch.DrawString(_font, "点击屏幕 启用 / 停用 IME", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, inputContent, new Vector2(10, 10+50+5), Color.White);

            Vector2 drawPos = new Vector2(5 + len.X, 10+50+5);

            _spriteBatch.DrawString(_font, "|", drawPos, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
