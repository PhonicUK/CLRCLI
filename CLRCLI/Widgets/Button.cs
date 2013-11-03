using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRCLI.Widgets
{
    public class Button : Widget, IFocusable
    {
        internal Button() {}
        public Button(Widget parent)
            : base(parent)
        {
            ActiveBackground = ConsoleColor.DarkMagenta;
            SelectedBackground = ConsoleColor.Magenta;
            Background = ConsoleColor.DarkGray;
            Foreground = ConsoleColor.White;
            DrawShadow = true;
            Text = "Button";
            Width = 10;
            Height = 3;
        }

        internal override void Render()
        {
            if (HasFocus)
            {
                if (DrawShadow)
                {
                    ConsoleHelper.DrawRectShade(DisplayLeft + 1, DisplayTop + 1, Width, Height, Parent.Background, ConsoleColor.DarkGray, '▒');
                }
                ConsoleHelper.DrawRectSolid(DisplayLeft, DisplayTop, Width, Height, ActiveBackground);
            }
            else
            {
                ConsoleHelper.DrawRectShade(DisplayLeft + 1, DisplayTop + 1, Width, Height, Parent.Background, ConsoleColor.DarkGray, ' '); //Erase the shadow
                DrawBackground();
            }

            ConsoleHelper.DrawText(DisplayLeft + (Width / 2) - (Text.Length / 2), DisplayTop + (Height / 2), Foreground, Text);
        }
    }
}
