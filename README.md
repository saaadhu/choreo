Choreo is an extension to Atmel Studio that lets uses write macros in python (IronPython, actually) to "choreograph" actions. The full [Visual Studio automation API][1] is available (except language services) to play with, in addition to the .NET BCL.
Choreo injects the top level DTE Visual Studio Automation object into the Python environment in which macros run. This means that macros can just use "dte" to refer to the top level DTE object. Like so
```python
  dte.ActiveDocument.Selection.Text = "Choreo Rocks"
```
Some trivial examples of what it can do

* Write current date/time at the current cursor position

```python
def InsertDateTime():
    dte.ActiveDocument.Selection.Text = DateTime.Now.ToString()
```
* Launch a google search for the current highlighted word

```python
def SearchGoogle():
    Process.Start("https://www.google.com/search?q=" + dte.ActiveDocument.Selection.Text)
```

* Do a "Run to Cursor" to the next line in the editor - useful for bypassing interrupts when debugging.

```python
def StepNextWithoutInterrupt():
	dte.ExecuteCommand("Edit.LineDown")
	dte.ExecuteCommand("Debug.RunToCursor")
```

* Replace current document's text with text from another source.

```python
def ReplaceWithNotepadContents():
    sel = dte.ActiveDocument.Selection
    sel.SelectAll()

    tempFile = Path.GetTempFileName()
    File.WriteAllText(tempFile, sel.Text)

    p = Process.Start("C:\\Windows\\notepad.exe", tempFile)
    p.WaitForExit()
    
    sel.Text = File.ReadAllText(tempFile)
```

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


### Use Atmel Studio SDK API from Choreo

The DTE object covers most of the Visual Studio Shell APIs. There are no other objects injected into the Python environment, but you can always reference DLLs from the python files themselves. For Atmel Studio specific functionality, there is the Atmel Studio XDK (http://gallery.atmel.com/Partner). 

The following snippet inserts the name of the device currently being debugged into the output window. It adds a reference to the SDK dll, imports a type, and then proceeds to call methods and access properties in that type. This also shows how you can use the VS output window as a console.

```python
import clr
from System import DateTime
from System.Diagnostics import Process

clr.AddReferenceToFileAndPath("C:\Program Files (x86)\Atmel\Atmel Studio 6.1\extensions\Application\Atmel.Studio.Services.Interfaces.dll")
from Atmel.Studio.Services import ATServiceProvider

def InsertTargetName():
    device = ATServiceProvider.TargetService2.GetLaunchedTarget().Device.Name
    writeToOutputWindow(device)

def writeToOutputWindow(message):
    window = dte.Windows.Item("{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}");
    window.Object.ActivePane.OutputString(message);
```

Changelog
---------

0.1 Initial Version

[1]: http://msdn.microsoft.com/en-us/library/vstudio/envdte.dte(v=vs.100).aspx
