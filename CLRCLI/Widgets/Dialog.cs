using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRCLI.Widgets
{
    public class Dialog : Widget
    {
        internal Dialog() { }
        public Dialog(Widget parent)
            : base(parent)
        {
            Top = 0;
            Left = 0;
            Width = 30;
            Height = 12;
            Background = ConsoleColor.Gray;
            Foreground = ConsoleColor.Black;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
        }

        internal override void Render()
        {
            if (Border == BorderStyle.None)
            {
                ConsoleHelper.DrawRectShade(DisplayLeft + 1, DisplayTop + 1, Width, Height, Parent.Background, ConsoleColor.Black, '▒');
            }
            else
            {
                ConsoleHelper.DrawRectShade(DisplayLeft, DisplayTop, Width + 2, Height + 2, Parent.Background, ConsoleColor.Black, '▓');
            }

            DrawBackground();

            BorderDrawMethod(DisplayLeft - 1, DisplayTop - 1, Width + 1, Height + 2, Foreground); 
            DrawLabel(); 
        }

        private void DrawLabel()
        {
            ConsoleHelper.DrawText(DisplayLeft + 1, DisplayTop - 1, Background, Foreground, " {0} ", Text);
        }

        public new void Show()
        {
            base.Show();
            this.FocusableChildren.FirstOrDefault().SetFocus();
        }
    }
}
