using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Revit.ScriptCS.ScriptRunner
{
    public class ScriptGlobals
    {
        public Document doc;
        public UIDocument uidoc;
        private readonly IProgress<string> progress;

        public ScriptGlobals(IProgress<string> Progress)
        {
            progress = Progress;
        }

        public void Print(string Message)
        {
            progress.Report(Message);
        }
    }
}