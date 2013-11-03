using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class ListBox : Widget, IFocusable, IAcceptInput
    {
        internal ListBox() {
            Items.CollectionChanged += Items_CollectionChanged;
        }
        public ListBox(Widget Parent)
            : base(Parent)
        {
            Background = ConsoleColor.DarkGray;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            Items.CollectionChanged += Items_CollectionChanged;
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Draw();
        }

        [XmlElement("ListItem", typeof(String))]
        public ObservableCollection<Object> Items = new ObservableCollection<Object>();

        private int _SelectedIndex = 0;
        [XmlIgnore]
        public int SelectedIndex
        {
            get
            {
                return _SelectedIndex;
            }
            set
            {
                if (value != _SelectedIndex)
                {
                    _SelectedIndex = value;
                    if (ScrollPosition > SelectedIndex)
                    {
                        ScrollPosition = SelectedIndex;
                    }
                    else if (ScrollPosition + Height < SelectedIndex + 1)
                    {
                        ScrollPosition = SelectedIndex - Height + 1;
                    }
                    Draw();
                }
            }
        }

        [XmlIgnore]
        public object SelectedItem
        {
            get
            {
                return Items[SelectedIndex];
            }
            set
            {
                SelectedIndex = Items.IndexOf(value);
            }
        }

        private int ScrollPosition = 0;

        internal override void Render()
        {
            var max = Math.Min(Items.Count, Height);
            DrawBackground();
            for (var i = 0; i < max; i++)
            {
                var index = i + ScrollPosition;
                ConsoleColor itemBG = (HasFocus && index == SelectedIndex) ? ActiveBackground : Background;
                var drawText = ShortenString(Items[index].ToString(), Width - 1);
                if (drawText.Length < Width)
                {
                    drawText = drawText + new String(' ', Width - drawText.Length);
                }
                ConsoleHelper.DrawText(DisplayLeft, DisplayTop + i, Foreground, itemBG, drawText);
            }

            ConsoleColor ScrollFG = (HasFocus) ? ConsoleColor.Gray : ConsoleColor.Black;

            if (Items.Count > Height)
            {
                var scrollHeight = Items.Count - 1 - Height;
                var pos = (int)(((double)ScrollPosition / (double)scrollHeight) * (Height - 1));

                for (int i = 0; i < Height; i++)
                {
                    var c = (pos == i) ? '█' : ' ';
                    ConsoleHelper.DrawText(DisplayLeft + Width, DisplayTop + i, ScrollFG, ConsoleColor.White, "{0}", c);
                }
            }
        }

        public bool Keypress(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (SelectedIndex > 0) { SelectedIndex--; return false; }
                    return true;
                case ConsoleKey.DownArrow:
                    if (SelectedIndex < Items.Count - 1) { SelectedIndex++; return false; } 
                    return true;
                default:
                    return true;
            }
            
        }
    }
}
