using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class Checkbox : Widget, IFocusable
    {
        internal Checkbox() {
            this.Clicked += Checkbox_Clicked;
        }
        public Checkbox(Widget parent)
            : base(parent)
        {
            Foreground = ConsoleColor.White;
            Background = Parent.Background;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            Text = "Checkbox";
            this.Clicked += Checkbox_Clicked;
        }

        void Checkbox_Clicked(object sender, EventArgs e)
        {
            Checked = !Checked;
        }

        public event EventHandler ValueChanged;

        private bool _checked;
        [XmlAttribute]
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                if (value != _checked)
                {
                    _checked = value;
                    Draw();
                    if (ValueChanged != null) { ValueChanged(this, EventArgs.Empty); }
                }
            }
        }

        internal override void Render()
        {
            char c = (Checked) ? 'X' : ' ';

            if (HasFocus)
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, ActiveBackground, "[{0}]", c);
                ConsoleHelper.DrawText(DisplayLeft + 4, DisplayTop, Foreground, Background,  Text);
            }
            else
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, "[{0}] {1}", c, Text);
            }
        }
    }
}
