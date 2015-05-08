using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRCLI.Widgets
{
    public class VerticalLine : Widget
    {
        internal VerticalLine() { }
        public VerticalLine(Widget parent)
            : base(parent)
        {
            Background = parent.Background;
            Foreground = ConsoleColor.Black;
            Border = BorderStyle.Thin;
        }

        internal override void Render()
        {
            if (Border != BorderStyle.None)
            {
                string b = (Border == BorderStyle.Thick) ? "║" : "│";

                for (var i = 0; i < Height; i++)
                {
                    ConsoleHelper.DrawText(DisplayLeft, DisplayTop + i, Foreground, Background, b);
                }
            }
        }
    }
}
