def PlaceBreakpointAndRun():
	dte.ActiveDocument.Selection.SelectAll()
	dte.ActiveDocument.Selection.Text = "int main() \r\n { \r\n int x = 3; \r\n return x; \r\n }"
	dte.Debugger.Breakpoints.Add("", dte.ActiveDocument.FullName, 4, 1, "", 0, "", "", 0, "", 1, 1)
	dte.Debugger.Go()

# jkuusama, here you go
def StepNextWithoutInterrupt():
	dte.ExecuteCommand("Edit.LineDown")
	dte.ExecuteCommand("Debug.RunToCursor")
