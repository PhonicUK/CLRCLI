using CLRCLI;
using CLRCLI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new RootWindow();

            var dialog = new Dialog(root) { Text = "Hello World!", Width = 24, Height = 8, Top = 4, Left = 4, Border = BorderStyle.Thick };
            new Label(dialog) {Text = "This is a dialog!", Top = 2, Left = 2};
            var button = new Button(dialog) { Text = "Oooooh", Top = 4, Left = 6 };

            button.Clicked += button_Clicked;

            root.Run();
        }

        static void button_Clicked(object sender, EventArgs e)
        {
            (sender as Button).RootWindow.Detatch();
        }
    }
}
