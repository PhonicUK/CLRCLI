CLRCLI README
===============

About
-----
CLRCLI (Common-Language-Runtime Command-Line-Interface) is an event-driven
windowing system using a line-art interface for use by command-line
applications.

![example screenshot](https://i.imgur.com/yJjTSdD.png)

Getting Started
---------------
Your starting point is the RootWindow. You only have one instance per 
application. You can add widgets directly to the root window, or to other
widgets.

Once the UI has been built (either in code, or by loading it from an XML
source) - the Run() method of the RootWindow is used to begin taking user input
and handling events. You will not be able to recieve any keyboard input via
Console.ReadLine() or similar while it is active.

RootWindow.Detach() is used to release control of the RootWindow so you can use
the console as normal. Note that this can only be done in response to an event
raised by a control or it may not terminate properly.

See the TestHarness app for a short 'Hello World' example.

Supported Widgets
-----------------

Currently the following widgets are implemented:

* Root Window
* Dialog
* Button
* Label
* Checkbox
* Radiobox
* Slider (Stylized Checkbox)
* Horizontal Progress Bar
* Listbox (with scrolling)
* Single-line textbox (with horizontal scroll, and password field support)
* Horizontal and vertical lines
* Borders with optional titles
* Simple horizontal bar graph
* Spinners (3x3 and 1x1 styles)

Writing new widgets
-------------------

New widgets are required to implement two constructors, one public - which
takes a parent widget as a parameter (which is passed to base()), and one
internal which must not take any arguments. Both are required to do any setup
needed to make the widget function. The internal constructor is used when the
widget is created by loading its details from an XML file, the public one is
used when creating the widget at runtime in code.

In addition any new widgets must be added to the list of XmlElements in the
Widget base class for the Children property. This allows it to be saved/loaded
with its proper name rather than just appearing as a widget with a type.

Widgets implement a Render() method. You must not use any Console methods
directly from the widget, but rather use the methods exposed by ConsoleHelper.
Render() needs to take into account whether or not a shadow is to be drawn by
checking the DrawShadow property, and whether the widget currently has focus.

The Widget base class exposes a DrawBorderMethod() which uses the appropriate
border method for the BorderType chosen for the widget.

Widgets that can be clicked need to implement IFocusable. There's no actual
implementation involved, and the RootWindow handles firing the clicked event.

Widgets that need to handle keyboard input (such as textboxes) need to
implement IAcceptInput. They will be passed every keypress via Keypress method.
If the input will be 'swallowed' (i.e. the widget is using the input itself) 
it should return False. Otherwise it should return True to allow the input to
be processed as normal.
