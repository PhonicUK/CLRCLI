using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class TinySpinner : Widget
    {
        internal TinySpinner()
        {
            timer = new Timer(100);
            timer.Elapsed += timer_Elapsed;
        }

        public TinySpinner(Widget parent)
            : base(parent) 
        {
            Background = Parent.Background;
            Foreground = ConsoleColor.White;
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

        private int Cycle = 3;
        private string Chars = @"|/-\";
        private Timer timer;

        internal override void Render()
        {
            Cycle = (Cycle + 1) % 4;
            var c = Chars[Cycle];
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, "{0}", c);
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
