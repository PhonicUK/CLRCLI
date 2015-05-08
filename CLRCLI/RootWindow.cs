using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    public class RootWindow : Widget
    {
        [XmlIgnore]
        public List<Widget> AllChildren;
        private List<Widget> RootFocusableChildren;
        private List<Widget> ActivableChildren
        {
            get
            {
                return RootFocusableChildren.Where(c => c.IsVisible).ToList();
            }
        }

        [XmlIgnore]
        public Object ViewModel { get; set; }

        public RootWindow()
            : base(null)
        {
            Top = 0;
            Left = 0;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            Background = ConsoleColor.DarkBlue;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            ActiveWidget = null;
            AllowDraw = false;
            AllChildren = new List<Widget>();            
        }

        internal override void Render()
        {
            ConsoleHelper.DrawRectSolid(DisplayLeft, DisplayTop, Width, Height, Background);
        }

        internal bool AllowDraw { get; private set; }

        private Widget _activeWidget;
        public Widget ActiveWidget
        {
            get
            {
                return _activeWidget;
            }
            set
            {
                if (value != null && value != _activeWidget)
                {
                    if (_activeWidget != null)
                    {
                        _activeWidget.HasFocus = false;
                        _activeWidget.Draw();
                    }

                    _activeWidget = value;

                    _activeWidget.HasFocus = true;
                    _activeWidget.Draw();
                }
            }
        }

        private Dictionary<String, Widget> NameLookup;

        private void BuildLookup()
        {
            NameLookup = new Dictionary<string, Widget>();
            AllChildren.ForEach(c =>
            {
                if (!String.IsNullOrEmpty(c.Id))
                {
                    NameLookup.Add(c.Id, c);
                }
            });
        }

        /// <summary>
        /// Find a widget by its ID.
        /// </summary>
        /// <param name="Id">The ID of the widget to search for.</param>
        /// <returns>A widget object if it is found, or null if no such widget exists.</returns>
        public Widget Find(string Id)
        {
            if (NameLookup == null) { BuildLookup(); }

            if (NameLookup.ContainsKey(Id))
            {
                return NameLookup[Id];
            }
            else
            {
                return null;
            }
        }

        private bool Running = false;

        /// <summary>
        /// Stop displaying the UI and handling keyboard input. Can only be used in response to an in-UI event.
        /// </summary>
        public void Detach()
        {
            Console.CursorVisible = true;
            Running = false;
            Console.Clear();
        }

        /// <summary>
        /// Start displaying the UI and handling keyboard input.
        /// </summary>
        public void Run()
        {
            Running = true;
            AllowDraw = true;
            Console.CursorVisible = false;
            RootFocusableChildren = AllChildren.Where(c => c is IFocusable).OrderBy(c => c.TabStop).ToList();
            ActiveWidget = RootFocusableChildren.FirstOrDefault();

            Draw();

            while (Running)
            {
                var k = Console.ReadKey(true);

                bool ProcessKey = true;

                if (ActiveWidget is IAcceptInput)
                {
                    ProcessKey = false;
                    switch (k.Key)
                    {
                        case ConsoleKey.Tab:
                            CycleFocus();
                            break;
                        default:
                            ProcessKey = HandleWidgetInput(k);
                            break;
                    }
                }
                
                if (ProcessKey)
                {
                    switch (k.Key)
                    {
                        case ConsoleKey.Tab:
                            CycleFocus((k.Modifiers == ConsoleModifiers.Shift) ? -1 : 1);
                            break;
                        case ConsoleKey.RightArrow:
                            MoveRight();
                            break;
                        case ConsoleKey.LeftArrow:
                            MoveLeft();
                            break;
                        case ConsoleKey.UpArrow:
                            MoveUp();
                            break;
                        case ConsoleKey.DownArrow:
                            MoveDown();
                            break;
                        case ConsoleKey.Spacebar:
                        case ConsoleKey.Enter:
                            EnterPressed();
                            break;
                        case ConsoleKey.Escape:
                            Running = false;
                            break;
                    }
                }
            }
        }

        private bool HandleWidgetInput(ConsoleKeyInfo k)
        {
            return (ActiveWidget as IAcceptInput).Keypress(k);
        }

        private void MoveDown()
        {
            var w = FindFocusableWidgetBelow(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                lastIndex = RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void MoveUp()
        {
            var w = FindFocusableWidgetAbove(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                lastIndex = RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void MoveLeft()
        {
            var w = FindFocusableWidgetToLeftOf(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                lastIndex = RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void MoveRight()
        {
            var w = FindFocusableWidgetToRightOf(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                lastIndex = RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void EnterPressed()
        {
            if (ActiveWidget != null && ActiveWidget.Enabled)
            {
                ActiveWidget.FireClicked();
            }
        }

        private int lastIndex = 0;

        private Widget FindFocusableWidgetToRightOf(Widget from)
        {
            var ImmediateRight = ActivableChildren.Where(c => c.Top == from.Top && c.Left > from.Left).OrderBy(c => c.Left).FirstOrDefault();

            if (ImmediateRight == null)
            {
                var RoughRight = ActivableChildren.Where(c => c.Left > from.Left && Math.Abs(c.Top - from.Top) < 2).OrderBy(c => c.Left).OrderBy(c => Math.Abs(c.Top - from.Top)).FirstOrDefault();

                if (RoughRight == null)
                {
                    return ActivableChildren.Where(c => c.Top == from.Top).OrderBy(c => c.Left).FirstOrDefault();
                }

                return RoughRight;
            }

            return ImmediateRight;
        }

        private Widget FindFocusableWidgetToLeftOf(Widget from)
        {
            var ImmediateLeft = ActivableChildren.Where(c => c.Top == from.Top && c.Left < from.Left).OrderByDescending(c => c.Left).FirstOrDefault();

            if (ImmediateLeft == null)
            {
                var RoughLeft = ActivableChildren.Where(c => c.Left < from.Left && Math.Abs(c.Top - from.Top) < 2).OrderByDescending(c => c.Left).OrderBy(c => Math.Abs(c.Top - from.Top)).FirstOrDefault();

                if (RoughLeft == null)
                {
                    return ActivableChildren.Where(c => c.Top == from.Top).OrderByDescending(c => c.Left).FirstOrDefault();
                }

                return RoughLeft;
            }

            return ImmediateLeft;
        }

        private Widget FindFocusableWidgetAbove(Widget from)
        {
            var ImmediateAbove = ActivableChildren.Where(c => c.Left == from.Left && c.Top < from.Top).OrderByDescending(c => c.Top).FirstOrDefault();

            if (ImmediateAbove == null)
            {
                return ActivableChildren.Where(c => c.Top < from.Top).OrderByDescending(c => c.Top).OrderBy(c => c.Left).FirstOrDefault();
            }

            return ImmediateAbove;
        }

        private Widget FindFocusableWidgetBelow(Widget from)
        {
            var ImmediateBelow = ActivableChildren.Where(c => c.Left == from.Left && c.Top > from.Top).OrderBy(c => c.Top).FirstOrDefault();

            if (ImmediateBelow == null)
            {
                return ActivableChildren.Where(c => c.Top > from.Top).OrderBy(c => c.Left).OrderBy(c => c.Top).FirstOrDefault();
            }

            return ImmediateBelow;
        }

        private void CycleFocus(int Direction = 1)
        {
            if (ActiveWidget == null)
            {
                lastIndex = 0;
                ActiveWidget = ActivableChildren.FirstOrDefault();
            }
            else
            {
                lastIndex = (lastIndex + Direction) % ActivableChildren.Count;
                if (lastIndex == -1) { lastIndex = ActivableChildren.Count - 1; }
                ActiveWidget = ActivableChildren[lastIndex];
            }
        }

        private static Type[] GetWidgetTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Widget))).ToArray();
        }

        private XmlSerializerNamespaces GetNS()
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            return ns;
        }

        public void Save(Stream stream)
        {
            var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());
            ser.Serialize(stream, this, GetNS());
        }

        public void Save(string Filename)
        {
            var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

            using (var stream = new StreamWriter(Filename))
            {
                ser.Serialize(stream, this, GetNS());
            }
        }

        public String Save()
        {
            var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

            using (var stream = new StringWriter())
            {
                ser.Serialize(stream, this, GetNS());
                return stream.ToString();
            }
        }

        public static RootWindow LoadFromStream(Stream stream)
        {
            var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

            var obj = (RootWindow)ser.Deserialize(stream);
            obj.FixChildParents(obj);
            obj.BuildLookup();
            return obj;
        }

        public static RootWindow LoadFromString(String data)
        {
            var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

            var obj = (RootWindow)ser.Deserialize(new StringReader(data));
            obj.FixChildParents(obj);
            obj.BuildLookup();
            return obj;
        }

        public static RootWindow LoadFromFile(string Filename)
        {
            var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

            using (var stream = new StreamReader(Filename))
            {
                var obj = (RootWindow)ser.Deserialize(stream);
                obj.FixChildParents(obj);
                obj.BuildLookup();
                return obj;
            }
        }
    }
}
