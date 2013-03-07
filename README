Choreo is an extension to Atmel Studio that lets uses write macros in python (IronPython, actually) to "choreograph" actions. The full Visual Studio automation API is available (except language services) to play with, in addition to the .NET BCL.

Some trivial examples of what it can do

* Write current date/time at the current cursor position
* Launch a google search for the current highlighted word
* Do a "Run to Cursor" to the next line in the editor - useful for bypassing interrupts when debugging.

Using Choreo
-------------

Macros in choreo are just python functions. All .py files in %localappdata%\Atmel\AtmelStudio\<version>\Extensions\Senthil Kumar Selvaraj\Choreo\<version>\Macros are picked up, and functions in the files are recognized as macros.

The functions show up as commands in Atmel Studio, named as Choreo.<filename>.<functionname>. You can then map keystrokes to them (Tools->Options->Keyboard and search for Choreo in the "Show Commands containing" textbox). Tools->Refresh Choreo reloads everything in the Macros directory, so you can add/modify macros without restarting Atmel Studio.


Changelog
---------

0.1 Initial Version

