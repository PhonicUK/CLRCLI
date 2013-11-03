using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class HorizontalBarGraph : Widget
    {
        internal HorizontalBarGraph() {
            Entries = new List<int>();
        }
        public HorizontalBarGraph(Widget parent)
            : base(parent)
        {
            Entries = new List<int>();
            Foreground = ConsoleColor.DarkGreen;
            Background = Parent.Background;
            Max = 100;
        }

        [XmlAttribute]
        public int Max { get; set; }

        private List<int> Entries;

        public void AddPoint(int Value)
        {
            Entries.Add(Value);
            if (Entries.Count > Width) { Entries.RemoveAt(0); }
            Draw();
        }

        internal override void Render()
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                var val = Entries[i];
                var scaledVal = (int)(((double)val / (double)Max) * Height);

                for (int j = 0; j < Height; j++)
                {
                    var ch = j < scaledVal ? "│" : " ";
                    if (j == scaledVal) { ch = "┐"; }
                    var top = DisplayTop + (Height - j);
                    ConsoleHelper.DrawText(DisplayLeft + i, top, Foreground, Background, "{0}", ch);
                }
            }
        }
    }
}
