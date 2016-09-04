using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFrame.UI.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoFrame.UI
{
    /// <summary>
    /// The GuiTextBox element is a traditional input text box structure
    /// You can extend this class for your Text Box controls and supply defaults for the 
    /// textures, or use as-is.
    /// </summary>
    public class GuiTextBox : GuiElement
    {
        public Texture2D BoxTexture { get; set; }
        public Color BoxMask { get; set; }
        public Texture2D BackgroundTexture { get; set; }
        public Color BackgroundMask { get; set; }
        public Texture2D CaratTexture { get; set; }
        public Color CaratMask { get; set; }

        public int CaratWidth { get; set; }
        public int CaratHeight { get; set; }

        public SpriteFont Font { get; set; }
        public Color FontColor { get; set; }

        public int MaxCharacters { get; set; }
        private string _text;
        private int _charIndex;

        public bool IsPassword { get; set; }
        public char PasswordChar { get; set; }

        private List<Keys> lastKeysPressed;
        private DateTime lastKeystroke;
        private double keyDelaySeconds;

        private int stringXOffset;
        private Rectangle caratLocation;

        public event EventHandler<InputEventArgs> TextChanged;

        public GuiTextBox(MainGame inGame)
            : base(inGame)
        {
            _text = "";
            MaxCharacters = 5000;
            lastKeysPressed = new List<Keys>();

            PasswordChar = '*';
            _charIndex = 0;
            keyDelaySeconds = 0.5d;

            BoxMask = Color.White;
            BackgroundMask = Color.White;
            CaratMask = Color.Black;
        }

        private int CharIndex
        {
            get
            {
                return _charIndex;
            }
            set
            {
                _charIndex = value;
                if (_charIndex > Text.Length) _charIndex = Text.Length;
                if (_charIndex < 0) _charIndex = 0;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                // fire "TextChanged" event
                InputState.Update(Keyboard.GetState(), Mouse.GetState());
                InputEventArgs args = new InputEventArgs();
                args.InputState = InputState;
                OnInputEvent(args, TextChanged);
            }
        }

        public override void Update(GameTime time)
        {
            if (IsVisible && IsEnabled)
            {
                base.Update(time);

                if (IsActive)
                {
                     // capture keyboard input, update the text string
                     Keys[] keys = Keyboard.GetState().GetPressedKeys();

                    List<char> chars = new List<char>();
                    bool capsDown = Console.CapsLock; // ugh...  replace this with better call
                    bool shiftDown = keys.Contains(Keys.RightShift) || keys.Contains(Keys.LeftShift);
                    bool caps = (!shiftDown && !capsDown) || (shiftDown && capsDown) ? false : true;

                    foreach (Keys key in keys)
                    {
                        // test for held Backspace button
                        if((key == Keys.Back || key == Keys.Delete || key == Keys.Left || key == Keys.Right) && lastKeysPressed.Contains(key))
                        {
                            // backspace button has been held down over multiple updates
                            // we want to allow for this... but not too fast
                            double secondsSinceKeystroke = (DateTime.Now - lastKeystroke).TotalSeconds;

                            if(secondsSinceKeystroke > keyDelaySeconds)
                            {
                                if (key == Keys.Back && Text.Length > 0 && _charIndex > 0)
                                {
                                    Text = Text.Remove(_charIndex - 1, 1);
                                    CharIndex--;
                                }
                                else if (key == Keys.Delete && Text.Length > 0 && _charIndex >= 0 && _charIndex != Text.Length)
                                {
                                    Text = Text.Remove(_charIndex, 1);
                                }
                                else if (key == Keys.Left)
                                {
                                    CharIndex--;
                                }
                                else if (key == Keys.Right)
                                {
                                    CharIndex++;
                                }

                                lastKeystroke = DateTime.Now;
                                keyDelaySeconds -= 0.1d;
                                if (keyDelaySeconds < 0.1d) keyDelaySeconds = 0.05d;
                            }
                        }
                        else if (lastKeysPressed.Count == 0 || !lastKeysPressed.Contains(key))
                        {
                            keyDelaySeconds = 0.5d;

                            if (key == Keys.Back)
                            {
                                if (Text.Length > 0 && _charIndex > 0)
                                {
                                    Text = Text.Remove(_charIndex - 1, 1);
                                    CharIndex--;
                                }
                            }
                            else if (key == Keys.Delete)
                            {
                                if (Text.Length > 0 && _charIndex >= 0 && _charIndex != Text.Length)
                                {
                                    Text = Text.Remove(_charIndex, 1);
                                }
                            }
                            else if (key == Keys.Enter) IsActive = false;
                            else if (key == Keys.Space && Text.Length < MaxCharacters) chars.Add(' ');
                            else if (key == Keys.OemPeriod && Text.Length < MaxCharacters) chars.Add(shiftDown ? '>' : '.');
                            else if (key == Keys.OemComma && Text.Length < MaxCharacters) chars.Add(shiftDown ? '<' : ',');
                            else if (key == Keys.OemQuestion && Text.Length < MaxCharacters) chars.Add(shiftDown ? '?' : '/');
                            else if (key == Keys.OemTilde && Text.Length < MaxCharacters) chars.Add(shiftDown ? '~' : '`');
                            else if (key == Keys.OemSemicolon && Text.Length < MaxCharacters) chars.Add(shiftDown ? ':' : ';');
                            else if (key == Keys.OemQuotes && Text.Length < MaxCharacters) chars.Add(shiftDown ? '\'' : '"');
                            else if (key == Keys.OemPlus && Text.Length < MaxCharacters) chars.Add(shiftDown ? '+' : '=');
                            else if (key == Keys.OemMinus && Text.Length < MaxCharacters) chars.Add(shiftDown ? '_' : '-');
                            else if (key == Keys.OemPipe && Text.Length < MaxCharacters) chars.Add(shiftDown ? '|' : '\\');
                            else if (key == Keys.OemOpenBrackets && Text.Length < MaxCharacters) chars.Add(shiftDown ? '{' : '[');
                            else if (key == Keys.OemCloseBrackets && Text.Length < MaxCharacters) chars.Add(shiftDown ? '}' : ']');
                            else if (key == Keys.D1 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '!' : '1');
                            else if (key == Keys.D2 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '@' : '2');
                            else if (key == Keys.D3 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '#' : '3');
                            else if (key == Keys.D4 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '$' : '4');
                            else if (key == Keys.D5 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '%' : '5');
                            else if (key == Keys.D6 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '^' : '6');
                            else if (key == Keys.D7 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '&' : '7');
                            else if (key == Keys.D8 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '*' : '8');
                            else if (key == Keys.D9 && Text.Length < MaxCharacters) chars.Add(shiftDown ? '(' : '9');
                            else if (key == Keys.D0 && Text.Length < MaxCharacters) chars.Add(shiftDown ? ')' : '0');
                            else if (key == Keys.NumPad0 && Text.Length < MaxCharacters) chars.Add('0');
                            else if (key == Keys.NumPad1 && Text.Length < MaxCharacters) chars.Add('1');
                            else if (key == Keys.NumPad2 && Text.Length < MaxCharacters) chars.Add('2');
                            else if (key == Keys.NumPad3 && Text.Length < MaxCharacters) chars.Add('3');
                            else if (key == Keys.NumPad4 && Text.Length < MaxCharacters) chars.Add('4');
                            else if (key == Keys.NumPad5 && Text.Length < MaxCharacters) chars.Add('5');
                            else if (key == Keys.NumPad6 && Text.Length < MaxCharacters) chars.Add('6');
                            else if (key == Keys.NumPad7 && Text.Length < MaxCharacters) chars.Add('7');
                            else if (key == Keys.NumPad8 && Text.Length < MaxCharacters) chars.Add('8');
                            else if (key == Keys.NumPad9 && Text.Length < MaxCharacters) chars.Add('9');
                            else if (key == Keys.Multiply && Text.Length < MaxCharacters) chars.Add('*');
                            else if (key == Keys.Divide && Text.Length < MaxCharacters) chars.Add('/');
                            else if (key == Keys.Subtract && Text.Length < MaxCharacters) chars.Add('-');
                            else if (key == Keys.Add && Text.Length < MaxCharacters) chars.Add('+');
                            else if (key == Keys.Home) CharIndex = 0;
                            else if (key == Keys.End) CharIndex = Text.Length;
                            else if (key == Keys.Left)
                            {
                                CharIndex--;

                                Vector2 stringLength = Font.MeasureString(Text);

                                if (CharIndex > 0 && stringLength.X > Width)
                                {

                                    Vector2 stringToCaratLength = Font.MeasureString(Text.Substring(0, CharIndex));
                                    Vector2 stringAfterCaratLength = stringLength - stringToCaratLength;
                                    Vector2 charAtCaratWidth = Text.Length > 0 && CharIndex > 0 ? Font.MeasureString(Text.Substring(CharIndex - 1, 1)) : new Vector2(0, 0);

                                    caratLocation.X -= (int)charAtCaratWidth.X;

                                    if (caratLocation.X - charAtCaratWidth.X < RelativeX)
                                    {
                                        stringXOffset -= (int)charAtCaratWidth.X;
                                    }
                                }
                            }
                            else if (key == Keys.Right && _charIndex < Text.Length)
                            {
                                CharIndex++;

                                Vector2 stringLength = Font.MeasureString(Text);

                                if (CharIndex < Text.Length && stringLength.X > Width)
                                {

                                    Vector2 stringToCaratLength = Font.MeasureString(Text.Substring(0, CharIndex));
                                    Vector2 stringAfterCaratLength = stringLength - stringToCaratLength;
                                    Vector2 charAtCaratWidth = Text.Length > 0 && CharIndex > 0 ? Font.MeasureString(Text.Substring(CharIndex - 1, 1)) : new Vector2(0, 0);

                                    caratLocation.X += (int)charAtCaratWidth.X;

                                    if (caratLocation.X + charAtCaratWidth.X > Width)
                                    {
                                        stringXOffset += (int)charAtCaratWidth.X;
                                    }
                                }
                            }
                            else if (key.ToString().Length == 1) // single character keys only. ignore functions
                            {
                                chars.Add(key.ToString()[0]);
                            }

                            lastKeystroke = DateTime.Now;
                        }
                    }

                    lastKeysPressed = keys.ToList();
                    lastKeysPressed.Remove(Keys.LeftShift);
                    lastKeysPressed.Remove(Keys.RightShift);

                    //Should realistically only be one character here. Need to filter out extra chars
                    if (Text.Length < MaxCharacters)
                    {
                        foreach (char keyChar in chars)
                        {
                            string keyString = caps ? keyChar.ToString().ToUpper() : keyChar.ToString().ToLower();

                            if (Text.Length == 0) Text += keyString;
                            else
                            {
                                string p1 = Text.Substring(0, _charIndex);
                                string p2 = "";
                                if (_charIndex < Text.Length) p2 = Text.Substring(_charIndex);

                                Text = p1 + keyString + p2;
                            }

                            CharIndex++;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime time)
        {
            if (IsVisible)
            {
                //set a scissor rasterizer so we can clip the text to paint so it doesn't spill outside of the box width
                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.ScissorTestEnable = true;

                //get the current graphics device scissor rect.
                Rectangle previousRect = MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle;

                //set the scissor rect to the box
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = HitBox;

                MainGame.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, rasterizerState, null, null);
                
                // draw panel background
                if (BackgroundTexture != null)
                    MainGame.SpriteBatch.Draw(BackgroundTexture, HitBox, BackgroundMask);

                if (BoxTexture != null)
                    MainGame.SpriteBatch.Draw(BoxTexture, HitBox, BoxMask);

                string finalText = "";

                if (IsPassword)
                {
                    foreach (char c in Text)
                        finalText += '*';
                }
                else finalText = Text;
                
                Vector2 stringLength = Font.MeasureString(finalText);

                Vector2 caratPlacement = Vector2.Zero;

                if (stringLength.X > HitBox.Width)
                {
                    // our string is bigger than the hitbox

                    // place string where the carat is. We don't want to hide the carat

                    if (CharIndex == Text.Length)
                    {
                        stringXOffset = (int)(stringLength.X - HitBox.Width);
                        caratPlacement = new Vector2(RelativeX + HitBox.Width, RelativeY);
                    }
                    else if (CharIndex <= 0)
                    {
                        stringXOffset = 0;

                        caratPlacement = new Vector2(5, 0);
                    }
                    else
                    {
                        Vector2 stringToCaratLength = Font.MeasureString(finalText.Substring(0, CharIndex));
                        Vector2 stringAfterCaratLength = stringLength - stringToCaratLength;
                        Vector2 charAtCaratWidth = finalText.Length > 0 && CharIndex > 0 ? Font.MeasureString(finalText.Substring(CharIndex - 1, 1)) : new Vector2(0, 0);

                        caratPlacement = new Vector2(caratLocation.X , RelativeY);
                        // at this point, the carat isn't at the front or end, so we want to paint the text
                        // wherever it last was, and just move the carat to a new place. The string should
                        // only really move if the carat is moved left or right at either end of the box.
                    }
                }
                else
                {
                    // our string is smaller than the hitbox
                    stringXOffset = 0;
                    // location of carat
                    Vector2 stringToCaratLength = Font.MeasureString(finalText.Substring(0, CharIndex));
                    caratPlacement = new Vector2(RelativeX + 6 + stringToCaratLength.X, RelativeY);
                }

                // move the vector as the string increases in length, or the carat location moves
                MainGame.SpriteBatch.DrawString(Font, finalText, new Vector2((RelativeX + 5) - stringXOffset, RelativeY), FontColor);

                caratLocation = new Rectangle((int)caratPlacement.X, (int)caratPlacement.Y + 3, CaratWidth, CaratHeight - 6);

                // paint the carat at the location of the charIndex (string length, or box width if greater than length)
                // also, only paint every even second, so it blinks.
                if (CaratTexture != null && DateTime.Now.Second % 2 == 0 && IsActive)
                    MainGame.SpriteBatch.Draw(CaratTexture, caratLocation, CaratMask);
                
                MainGame.SpriteBatch.End();

                //reset the scissor rect
                MainGame.SpriteBatch.GraphicsDevice.ScissorRectangle = previousRect;

                base.Draw(time);
            }
        }
    }
}
