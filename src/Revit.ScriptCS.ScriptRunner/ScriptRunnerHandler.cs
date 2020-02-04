using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Scripting.Hosting;

namespace Revit.ScriptCS.ScriptRunner
{
    public class ScriptRunnerHandler : IExternalEventHandler
    {
        public string ScriptText { get; internal set; }

        public string ScriptResult { get; private set; }

        public RoslynEditorViewModel RoslynEditorViewModel { get; set; }

        public void Execute(UIApplication app)
        {
            var assembliesToRef = new List<Assembly>
            {
                typeof(object).Assembly, //mscorlib
                typeof(Autodesk.Revit.UI.UIApplication).Assembly, // Microsoft.CodeAnalysis.Workspaces
                typeof(Autodesk.Revit.DB.Document).Assembly, // Microsoft.Build
            };

            var namespaces = new List<string>
            {
                "Autodesk.Revit.UI",
                "Autodesk.Revit.DB",
                "Autodesk.Revit.DB.Structure",
                "System",
                "System.Collections.Generic",
                "System.IO",
                "System.Linq"
            };

            var globals = new ScriptGlobals { doc = app.ActiveUIDocument.Document };

            var options = ScriptOptions.Default.AddReferences(assembliesToRef).WithImports(namespaces);


            try
            {
                object result = CSharpScript.EvaluateAsync<object>(ScriptText, options, globals).Result;
                RoslynEditorViewModel.Result = CSharpObjectFormatter.Instance.FormatObject(result);
            }
            catch ( System.Exception ex )
            {
                RoslynEditorViewModel.Result = CSharpObjectFormatter.Instance.FormatObject(ex);
            }
        }

        public string GetName()
        {
            return "A Script Runner";
        }


    }
}