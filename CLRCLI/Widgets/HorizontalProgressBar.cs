using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class HorizontalProgressBar : Widget
    {
        private int _value;
        [XmlAttribute]
        public int Value
        {
            get
            { 
                return _value; 
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (_value < 0) { _value = 0; }
                    if (_value > MaxValue) { _value = MaxValue; }
                    Draw();
                }
            }
        }

        [XmlAttribute]
        public int MaxValue { get; set; }

        internal HorizontalProgressBar() {}
        public HorizontalProgressBar(Widget parent)
            : base(parent)
        {
            Background = ConsoleColor.DarkGray;
            Foreground = ConsoleColor.DarkGreen;
            Height = 1;
            MaxValue = 100;
            Value = 50;
        }

        internal override void Render()
        {
            var scaled = (int)(((Single)Value / MaxValue) * (Single)Width);
            var remainder = Width - scaled;

            if (scaled > 0)
            {
                ConsoleHelper.DrawRectSolid(DisplayLeft, DisplayTop, scaled, Height, Foreground);
            }

            if (remainder > 0)
            {
                ConsoleHelper.DrawRectSolid(DisplayLeft + scaled, DisplayTop, remainder, Height, Background);
            }
        }
    }
}
