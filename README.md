Choreo is an extension to Atmel Studio that lets uses write macros in python (IronPython, actually) to "choreograph" actions. The full Visual Studio automation API is available (except language services) to play with, in addition to the .NET BCL.

Some trivial examples of what it can do

* Write current date/time at the current cursor position
* Launch a google search for the current highlighted word
* Do a "Run to Cursor" to the next line in the editor - useful for bypassing interrupts when debugging.

Using Choreo
-------------

Macros in choreo are just python functions. All .py files in %localappdata%\Atmel\AtmelStudio\<version>\Extensions\Senthil Kumar Selvaraj\Choreo\<version>\Macros are picked up, and functions in the files are recognized as macros.

The functions show up as commands in Atmel Studio, named as Choreo.&lt;filename&gt;.&lt;functionname&gt;. You can then map keystrokes to them (Tools->Options->Keyboard and search for Choreo in the "Show Commands containing" textbox). Tools->Refresh Choreo reloads everything in the Macros directory, so you can add/modify macros without restarting Atmel Studio.

As an example, assuming you're running Atmel Studio 6.1 and Choreo 0.1,

1. Launch Atmel Studio
2. Create a new file, example.py, in %localappdata%\Atmel\AtmelStudio\6.1\Extensions\Senthil Kumar Selvaraj\Choreo\0.1\Macros, and save it with the following contents

```python

    def InsertChoreoRocks():
	    dte.ActiveDocument.Selection.Text = "Choreo Rocks"
```
3. In Atmel Studio, click on Tools -> Refresh Choreo
4. Open Tools -> Options -> Environment -> Keyboard, and enter Choreo in the "Show Commands Containing" textbox. You should see Choreo.example.InsertChoreoRocks in the list. Bind it to a keystroke, say Ctrl+Shift+C, by keying in the sequence in the "Press shortcut keys" textbox, and click Assign, then OK.
5. In any open text document in Atmel Studio, hit Ctrl+Shift+C to run the macro, and you'll see the text "Choreo Rocks" gets inserted at the current cursor position.

That's all there is to it. You can change code for an existing macro and run it right away. New functions and new .py files need a refresh (Tools -> Refresh Choreo) to show up in the commands list.

Changelog
---------

0.1 Initial Version

