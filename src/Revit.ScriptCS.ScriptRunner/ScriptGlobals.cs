using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Revit.ScriptCS.ScriptRunner
{
    public class ScriptGlobals
    {
        public Document doc;
        public UIDocument uidoc;
        public IProgress<string> progress;
    }
}