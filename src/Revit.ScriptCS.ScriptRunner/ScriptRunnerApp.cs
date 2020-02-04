using Autodesk.Revit.UI;
using RoslynPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Revit.ScriptCS.ScriptRunner
{
    public class ScriptRunnerApp : IExternalApplication
    {
        internal static ScriptRunnerApp thisApp = null;
        private Window scriptEditor;
        public Result OnShutdown(UIControlledApplication application)
        {
            if ( scriptEditor != null && scriptEditor.IsLoaded )
            {
                scriptEditor.Close();
            }
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            scriptEditor = null;
            thisApp = this;
            return Result.Succeeded;
        }

        public void ShowWPF()
        {
            try
            {

                if ( scriptEditor == null || !scriptEditor.IsLoaded )
                {
                    var handler = new ScriptRunnerHandler();
                    ExternalEvent externalEvent = ExternalEvent.Create(handler);
                    var assembliesToRef = new List<Assembly>
                {
                    typeof(object).Assembly, //mscorlib
                    typeof(Autodesk.Revit.UI.UIApplication).Assembly,
                    typeof(Autodesk.Revit.DB.Document).Assembly,
                    Assembly.Load("RoslynPad.Roslyn.Windows"),
                    Assembly.Load("RoslynPad.Editor.Windows")
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

                    //var roslynHostReferences = RoslynHostReferences.Empty.With(imports: namespaces);

                    var roslynHost = new RevitRoslynHost(
                        additionalAssemblies: assembliesToRef,
                        references: RoslynHostReferences.NamespaceDefault.With(typeNamespaceImports: new[] { typeof(UIApplication), typeof(Autodesk.Revit.DB.Document), typeof(Dictionary<,>), typeof(System.Linq.Enumerable), typeof(ScriptGlobals) }),
                        disabledDiagnostics: ImmutableArray.Create("CS1701", "CS1702", "CS0518"));

                    var document = new RoslynEditorViewModel(roslynHost, externalEvent, handler);
                    scriptEditor = new RoslynEditor(document);
                    scriptEditor.Show();
                }
            }
            catch ( Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                //throw;
            }
        }
    }
}
