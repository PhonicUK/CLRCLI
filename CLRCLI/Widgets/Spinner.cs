using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class Spinner : Widget
    {
        internal Spinner()
        {
            timer = new Timer(100);
            timer.Elapsed += timer_Elapsed;
        }

        public Spinner(Widget parent) : base(parent) 
        {
            Background = Parent.Background;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            timer = new Timer(100);
            timer.Elapsed += timer_Elapsed;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Draw();
        }

        private string RotateString(string Input, int Offset)
        {
            return Input.Substring(Offset) + Input.Substring(0, Offset);
        }

        private int Cycle = 8;
        private string Chars = "    ░▒▓█";
        private Timer timer;

        internal override void Render()
        {
            Cycle = (Cycle - 1);
            if (Cycle == 0) { Cycle = 8; }
            var DrawStr = RotateString(Chars, Cycle);
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, DrawStr.Substring(0, 3));
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop + 1, Foreground, Background, "{0} {1}", DrawStr[7], DrawStr[3]);
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop + 2, Foreground, Background, "{0}{1}{2}", DrawStr[6], DrawStr[5], DrawStr[4]);
        }

        private bool _spinning = false;
        [XmlAttribute]
        public bool Spinning
        {
            get
            {
                return _spinning;
            }
            set
            {
                _spinning = value;
                if (value) { timer.Start(); } else { timer.Stop(); }
            }
        }
    }
}
