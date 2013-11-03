using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class SingleLineTextbox : Widget, IFocusable, IAcceptInput
    {
        [XmlAttribute]
        [DefaultValue("")]
        public String PasswordChar { get; set; }

        private Timer ToggleCursorTimer = new Timer(500);

        internal SingleLineTextbox()
        {
            ToggleCursorTimer.Elapsed += ToggleCursorTimer_Elapsed;
            ToggleCursorTimer.Start();
        }

        public SingleLineTextbox(Widget parent) : base(parent)
        {
            Background = ConsoleColor.DarkGray;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            ToggleCursorTimer.Elapsed += ToggleCursorTimer_Elapsed;
            ToggleCursorTimer.Start();
        }

        void ToggleCursorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CursorVisible = !CursorVisible;
            Draw();
        }

        private int _CursorPosition = 1;
        [XmlIgnore]
        public int CursorPosition
        {
            get { return _CursorPosition; }
            private set
            {
                if (value != _CursorPosition)
                {
                    _CursorPosition = value;
                    CursorVisible = true;
                }
            }
        }
        private bool CursorVisible = false;
        private char CursorChar = '_';

        internal override void Render()
        {
            var drawBG = (HasFocus) ? ActiveBackground : Background;
            var drawText = Text;

            if (!String.IsNullOrEmpty(PasswordChar)) { drawText = new String(PasswordChar[0], drawText.Length); }

            if (HasFocus)
            {
                if (CursorPosition > drawText.Length)
                {
                    drawText += (CursorVisible) ? CursorChar : ' ';
                }
                else if (CursorVisible)
                {
                    StringBuilder sb = new StringBuilder(drawText);
                    sb[CursorPosition - 1] = CursorChar;
                    drawText = sb.ToString();
                }
            }

            if (drawText.Length > Width)
            {
                if (HasFocus)
                {
                    var startPos = drawText.Length - Width;
                    if (startPos > (CursorPosition - 1))
                    {
                        startPos = CursorPosition - 1;
                        drawText = drawText.Substring(startPos, Width);
                    }
                    else
                    {
                        drawText = drawText.Substring(startPos);
                    }
                    
                }
                else
                {
                    drawText = drawText.Substring(0, Width);
                }
            }
            else if (drawText.Length < Width)
            {
                drawText = drawText + new String(' ', Width - drawText.Length);
            }

            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, drawBG, drawText);
        }

        public bool Keypress(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                //Keys we don't specifically want to handle, just return true.
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.Enter:
                case ConsoleKey.Escape:
                    return true;
                case ConsoleKey.Backspace:
                    RemoveBehind();
                    break;
                case ConsoleKey.RightArrow:
                    CursorPosition++;
                    CursorPosition = Math.Min(CursorPosition, Text.Length + 1);
                    Draw();
                    break;
                case ConsoleKey.LeftArrow:
                    CursorPosition--;
                    CursorPosition = Math.Max(CursorPosition, 1);
                    Draw();
                    break;
                case ConsoleKey.Delete:
                    DeleteKey();
                    break;
                case ConsoleKey.Home:
                    CursorPosition = 1;
                    Draw();
                    break;
                case ConsoleKey.End:
                    CursorPosition = Text.Length + 1;
                    Draw();
                    break;
                default:
                    if (!Char.IsControl(key.KeyChar))
                    {
                        AddCharacter(key);
                    }
                    else
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        private void RemoveBehind()
        {
            if (CursorPosition > 1)
            {
                _Text = _Text.Remove(CursorPosition - 2, 1);
                CursorPosition--;
                FireTextChanged();
                Draw();
            }
        }

        private void DeleteKey()
        {
            if (CursorPosition < Text.Length + 1)
            {
                Text = Text.Remove(CursorPosition - 1, 1);
            }
        }

        private void AddCharacter(ConsoleKeyInfo key)
        {
            //Modifies the private text member to prevent an early redraw.
            if (CursorPosition <= Text.Length)
            {
                _Text = _Text.Insert(CursorPosition - 1, "" + key.KeyChar);
            }
            else
            {
                _Text += key.KeyChar;
            }
            CursorPosition++;
            FireTextChanged();
            Draw();
        }
    }
}
