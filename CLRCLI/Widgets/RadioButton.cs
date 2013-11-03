using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class RadioButton : Widget, IFocusable
    {
        internal RadioButton() {
            this.Clicked += OptionBox_Clicked;
        }
        public RadioButton(Widget parent)
            : base(parent)
        {
            Foreground = ConsoleColor.White;
            Background = Parent.Background;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            Text = "OptionBox";
            this.Clicked += OptionBox_Clicked;
        }

        void OptionBox_Clicked(object sender, EventArgs e)
        {
            Checked = true;
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
                    if (value)
                    {
                        foreach (var r in Siblings.OfType<RadioButton>())
                        {
                            r.Checked = false;
                        }
                    }
                    _checked = value;
                    Draw();
                    if (ValueChanged != null) { ValueChanged(this, EventArgs.Empty); }
                }
            }
        }

        internal override void Render()
        {
            char c = (Checked) ? 'O' : ' ';

            if (HasFocus)
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, ActiveBackground, "({0})", c);
                ConsoleHelper.DrawText(DisplayLeft + 4, DisplayTop, Foreground, Background, Text);
            }
            else
            {
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, "({0}) {1}", c, Text);
            }
        }
    }
}
