using CLRCLI.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI
{
    public enum BorderStyle
    {
        Block,
        Thick,
        Thin,
        None
    }

    public abstract class Widget
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public int Top { get; set; }
        [XmlAttribute]
        public int Left { get; set; }
        [XmlAttribute]
        public int Width { get; set; }
        [XmlAttribute]
        public int Height { get; set; }
        [XmlAttribute]
        [DefaultValue(ConsoleColor.Gray)]
        public ConsoleColor Background { get; set; }
        [XmlAttribute]
        [DefaultValue(ConsoleColor.White)]
        public ConsoleColor Foreground { get; set; }
        [XmlAttribute]
        [DefaultValue(ConsoleColor.Magenta)]
        public ConsoleColor SelectedBackground { get; set; }
        [XmlAttribute]
        [DefaultValue(ConsoleColor.DarkMagenta)]
        public ConsoleColor ActiveBackground { get; set; }
        [XmlAttribute]
        [DefaultValue(ConsoleColor.Black)]
        public ConsoleColor ActiveForeground { get; set; }
        [XmlAttribute]
        [DefaultValue(0)]
        public int TabStop { get; set; }
        [XmlAttribute]
        [DefaultValue(BorderStyle.Block)]
        public BorderStyle Border { get; set; }
        [XmlAttribute]
        [DefaultValue(true)]
        public bool Enabled { get; set; }
        [XmlAttribute]
        [DefaultValue(false)]
        public bool DrawShadow { get; set; }

        [XmlIgnore]
        public Widget Parent {get; internal set;}

        private bool _HasFocus;
        [XmlIgnore]
        public bool HasFocus
        {
            get
            {
                return _HasFocus;
            }
            internal set
            {
                if (value != _HasFocus)
                {
                    _HasFocus = value;
                    if (value)
                    {
                        if (GotFocus != null) { GotFocus(this, EventArgs.Empty); }
                    }
                    else
                    {
                        if (LostFocus != null) { LostFocus(this, EventArgs.Empty); }
                    }
                }
            }
        }

        internal void FireTextChanged()
        {
            if (TextChanged != null) { TextChanged(this, EventArgs.Empty); }
        }

        internal string _Text = "";
        [XmlAttribute]
        [DefaultValue("")]
        public string Text
        {
            get { return _Text; }
            set
            {
                if (value != _Text)
                {
                    bool parentRedraw = false;
                    if (value.Length < _Text.Length)
                    {
                        parentRedraw = true;
                    }
                    _Text = value;
                    
                    if (TextChanged != null) { TextChanged(this, EventArgs.Empty); }

                    if (Parent != null) 
                    {
                        if (parentRedraw) { Parent.Draw(); }
                        else { Draw(); }
                    }
                }
            }
        }

        internal ConsoleHelper.DrawBoxMethod BorderDrawMethod
        {
            get
            {
                switch (Border)
                {
                    case BorderStyle.Block: return ConsoleHelper.DrawBlockOutline;
                    case BorderStyle.Thick: return ConsoleHelper.DrawDoubleOutline;
                    case BorderStyle.Thin: return ConsoleHelper.DrawSingleOutline;
                    default: return ConsoleHelper.DrawNothing;
                }
            }
        }

        internal string ShortenString(string input, int maxLen)
        {
            if (input.Length < maxLen)
            {
                return input;
            }

            return input.Substring(0, maxLen);
        }

        internal void DrawBackground()
        {
            ConsoleHelper.DrawRectSolid(DisplayLeft, DisplayTop, Width, Height, Background);
        }

        [XmlElement(typeof(Border)), XmlElement(typeof(Button)), XmlElement(typeof(Checkbox)),
        XmlElement(typeof(Dialog)), XmlElement(typeof(HorizontalBarGraph)), XmlElement(typeof(HorizontalLine)), XmlElement(typeof(VerticalLine)),
        XmlElement(typeof(HorizontalProgressBar)), XmlElement(typeof(Label)), XmlElement(typeof(ListBox)),
        XmlElement(typeof(RadioButton)), XmlElement(typeof(SingleLineTextbox)), XmlElement(typeof(SlideToggle)),
        XmlElement(typeof(Spinner)), XmlElement(typeof(TinySpinner))]
        public List<Widget> Children { get; set; }

        internal abstract void Render();

        public event EventHandler Clicked;
        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
        public event EventHandler TextChanged;

        private bool _visible = true;
        [XmlAttribute]
        [DefaultValue(true)]
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value != _visible)
                {
                    _visible = value;

                    /*
                     * Null check prevents drawing before fully instantiated (specifically when there's no parent).
                     * Works because everything (Except RootWindow) has a parent but only after it has been fully set up.
                     */
                    if (Parent != null) { Draw(); } 
                }
            }
        }

        [XmlIgnore]
        public bool IsVisible
        {
            get
            {
                return this.Visible && (Parent == null ? true : Parent.IsVisible);
            }
        }

        [XmlIgnore]
        public int DisplayTop
        {
            get
            {
                return (Parent == null) ? Top : Parent.DisplayTop + Top;
            }
        }

        [XmlIgnore]
        public int DisplayLeft
        {
            get
            {
                return (Parent == null) ? Top : Parent.DisplayLeft + Left;
            }
        }

        internal void FireClicked()
        {
            if (Clicked != null)
            {
                var OldBG = ActiveBackground;
                ActiveBackground = SelectedBackground;
                Draw();
                Thread.Sleep(100);
                ActiveBackground = OldBG;
                Draw();
                Clicked(this, EventArgs.Empty);
            }
        }

        internal Widget(){}
        public Widget(Widget parent)
        {
            Visible = true;
            Enabled = true;
            Children = new List<Widget>();
            Parent = parent;
            if (parent != null)
            {
                parent.Children.Add(this);
                RootWindow.AllChildren.Add(this);
            }
        }

        internal void FixChildParents(RootWindow root)
        {
            Children.ForEach(c =>
            {
                root.AllChildren.Add(c);
                c.Parent = this;
                c.FixChildParents(root);
            });
        }

        [XmlIgnore]
        public RootWindow RootWindow
        {
            get
            {
                if (CachedRootWindow == null) { CachedRootWindow = FindRootWindow(); }
                return CachedRootWindow;
            }
        }

        private RootWindow CachedRootWindow = null;

        private RootWindow FindRootWindow()
        {
            if (this.GetType() == typeof(RootWindow))
            {
                return (RootWindow)this;
            }
            else
            {
                return Parent.FindRootWindow();
            }
        }

        [XmlIgnore]
        public List<Widget> Siblings
        {
            get
            {
                if (Parent == null)
                {
                    return new List<Widget>();
                }
                else
                {
                    return Parent.Children;
                }
            }
        }

        public void SetFocus()
        {
            RootWindow.ActiveWidget = this;
        }

        internal void Draw()
        {
            if (RootWindow.AllowDraw == false) { return; }
            if (Visible && (Parent == null || Parent.Visible))
            {
                lock (Console.Out)
                {
                    Render();
                }

                if (Children != null)
                {
                    Children.Where(c => c.Visible).ToList().ForEach(c => c.Draw());
                }
            }
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Parent.Render();
            Visible = false;
        }

        private void Blank()
        {
            lock (Console.Out)
            {
                ConsoleHelper.DrawRectSolid(DisplayLeft, DisplayTop, Width, Height, (Parent == null) ? Background : Parent.Background);
            }
        }

        public Widget FindChildren(string Id)
        {
            if (Children.Any() == false)
            {
                return null;
            }

            var Find = Children.FirstOrDefault(c => c.Id == Id);

            if (Find != null) { return Find; }

            foreach (var c in Children)
            {
                var FindInChild = c.FindChildren(Id);
                if (FindInChild != null) { return FindInChild; }
            }

            return null;
        }

        public Widget Nearest<T>() where T : Widget
        {
            if (this is T) { return this; }
            if (this.Parent == null) { return null; }
            return Parent.Nearest<T>();
        }

        [XmlIgnore]
        public List<Widget> FocusableChildren
        {
            get
            {
                return Children.Where(c => c is IFocusable).OrderBy(c => c.TabStop).ToList();
            }
        }
    }
}
