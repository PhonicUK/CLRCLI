CLRCLI README
===============

About
-----
CLRCLI is an event-driven windowing system using a line-art interface for use
by command-line applications.


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