using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Revit.ScriptCS.ScriptRunner
{
    public class DocumentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DocumentViewTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if ( item is DocumentViewModel )
                return DocumentViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
