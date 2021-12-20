using Autodesk.Revit.UI;
using RoslynPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Revit.ScriptCS.ScriptRunner
{
    public class ScriptRunnerApp : IExternalApplication
    {
        internal static ScriptRunnerApp thisApp = null;
        static readonly string ExecutingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        private Window scriptEditor;

        private Thread scriptEditorThread;
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            scriptEditor = null;
            thisApp = this;

            RibbonPanel rvtRibbonPanel = application.CreateRibbonPanel("Scripting");

            PushButtonData pushButtonData = new PushButtonData("RunScriptCS", "C# Scripting", ExecutingAssemblyPath, "Revit.ScriptCS.ScriptRunner.ScriptRunnerExternalCommand");
            PushButton runButton = rvtRibbonPanel.AddItem(pushButtonData) as PushButton;

            runButton.ToolTip = "Run C# Scripts";
            runButton.Image = GetEmbeddedImage("Revit.ScriptCS.ScriptRunner.Resources.logo_Csharp_16x16.png");
            runButton.LargeImage = GetEmbeddedImage("Revit.ScriptCS.ScriptRunner.Resources.logo_Csharp_32x32.png");

            return Result.Succeeded;
        }

        static BitmapSource GetEmbeddedImage(string name)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                using ( var s = a.GetManifestResourceStream(name) )
                {
                    return BitmapFrame.Create(s);
                }
            }
            catch
            {
                return null;
            }
        }

        public void ShowWPF()
        {
            try
            {
                if ( scriptEditorThread is null || !scriptEditorThread.IsAlive )
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

                    var roslynHost = new RevitRoslynHost(
                        additionalAssemblies: assembliesToRef,
                        references: RoslynHostReferences.NamespaceDefault.With(typeNamespaceImports: new[] { typeof(UIApplication), typeof(Autodesk.Revit.DB.Document), typeof(Dictionary<,>), typeof(System.Linq.Enumerable), typeof(ScriptGlobals) }),
                        disabledDiagnostics: ImmutableArray.Create("CS1701", "CS1702", "CS0518"));
                                      
                    var document = new RoslynEditorViewModel(roslynHost, externalEvent, handler);                    

                    scriptEditorThread = new Thread(new ThreadStart(() =>
                    {
                        SynchronizationContext.SetSynchronizationContext(
                            new DispatcherSynchronizationContext(
                                Dispatcher.CurrentDispatcher));

                        scriptEditor = new RoslynEditor(document);
                        scriptEditor.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        handler.Progress = new Progress<string>(message => document.Result += message + Environment.NewLine);
                        scriptEditor.Closed += (s, e) => Dispatcher.CurrentDispatcher.InvokeShutdown();
                        scriptEditor.Show();
                        Dispatcher.Run();
                    }));

                    scriptEditorThread.SetApartmentState(ApartmentState.STA);
                    scriptEditorThread.IsBackground = true;
                    scriptEditorThread.Start();
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }

    }
}
