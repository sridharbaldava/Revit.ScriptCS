using Autodesk.Revit.UI;
using Microsoft.CodeAnalysis;
using RoslynPad.Editor;
using RoslynPad.Roslyn;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Revit.ScriptCS.ScriptRunner
{
    /// <summary>
    /// Interaction logic for RoslynEditor.xaml
    /// </summary>
    public partial class RoslynEditor : Window
    {        
        public RoslynEditor(RoslynEditorViewModel document)
        {
            InitializeComponent();
            DataContext = document;            
        }

        private async void CodeEditor_Loaded(object sender, RoutedEventArgs e)
        {
            var editor = (RoslynCodeEditor)sender;
            editor.Loaded -= CodeEditor_Loaded;
            editor.Focus();

            var viewModel = (RoslynEditorViewModel)DataContext;
            var documentViewModel = (DocumentViewModel)editor.DataContext;
            var workingDirectory = Directory.GetCurrentDirectory();

            var documentId = await editor.InitializeAsync(viewModel.Host, new ClassificationHighlightColors(),
                workingDirectory, string.Empty, SourceCodeKind.Script).ConfigureAwait(false);

            documentViewModel.Initialize(documentId);
            editor.Document.TextChanged += documentViewModel.OnTextChanged;
        }

        private void dockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {
            e.Cancel = true;
            var documentViewModel = (DocumentViewModel)e.Document.Content;
            var viewModel = (RoslynEditorViewModel)DataContext;
            viewModel.Close(documentViewModel);
        }
    }
}
