using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revit.ScriptCS.ScriptRunner
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ScriptRunnerExternalCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			try
			{
				ScriptRunnerApp.thisApp.ShowWPF();
				return Result.Succeeded;
			}
			catch ( Exception ex)
			{
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
