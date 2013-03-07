from System import DateTime
from System.Diagnostics import Process

def InsertDateTime():
    dte.ActiveDocument.Selection.Text = DateTime.Now.ToString()

def SearchGoogle():
    Process.Start("https://www.google.com/search?q=" + dte.ActiveDocument.Selection.Text)
    




