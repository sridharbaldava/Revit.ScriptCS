using Autodesk.Revit.UI;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using RoslynPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Revit.ScriptCS.ScriptRunner
{
    public class RoslynEditorViewModel : ViewModelBase
    {
        private readonly ExternalEvent _externalEvent;
        private readonly ScriptRunnerHandler _scriptRunnerHandler;
        private string _result;

        public RoslynEditorViewModel(RevitRoslynHost host, ExternalEvent externalEvent, ScriptRunnerHandler scriptRunnerHandler)
        {
            Host = host;
            scriptRunnerHandler.RoslynEditorViewModel = this;
            _externalEvent = externalEvent;
            _scriptRunnerHandler = scriptRunnerHandler;
        }

        public RevitRoslynHost Host { get; }

        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        ObservableCollection<DocumentViewModel> _documents = new ObservableCollection<DocumentViewModel>();
        ReadOnlyObservableCollection<DocumentViewModel> _readonlydocuments = null;
        public ReadOnlyObservableCollection<DocumentViewModel> Documents
        {
            get
            {
                if ( _readonlydocuments == null )
                    _readonlydocuments = new ReadOnlyObservableCollection<DocumentViewModel>(_documents);

                return _readonlydocuments;
            }
        }

        #region OpenCommand
        RelayCommand _openCommand = null;
        public ICommand OpenCommand
        {
            get
            {
                if ( _openCommand == null )
                {
                    _openCommand = new RelayCommand((p) => OnOpen(p), (p) => CanOpen(p));
                }

                return _openCommand;
            }
        }

        private bool CanOpen(object parameter)
        {
            return true;
        }

        private void OnOpen(object parameter)
        {
            var dlg = new OpenFileDialog();
            if ( dlg.ShowDialog().GetValueOrDefault() )
            {
                var DocumentViewModel = Open(dlg.FileName);
                ActiveDocument = DocumentViewModel;
            }
        }

        public DocumentViewModel Open(string filepath)
        {
            var DocumentViewModel = _documents.FirstOrDefault(fm => fm.FilePath == filepath);
            if ( DocumentViewModel != null )
                return DocumentViewModel;

            DocumentViewModel = new DocumentViewModel(this, filepath);
            _documents.Add(DocumentViewModel);

            return DocumentViewModel;
        }

        #endregion

        #region NewCommand
        RelayCommand _newCommand = null;
        public ICommand NewCommand
        {
            get
            {
                if ( _newCommand == null )
                {
                    _newCommand = new RelayCommand((p) => OnNew(p), (p) => CanNew(p));
                }

                return _newCommand;
            }
        }

        private bool CanNew(object parameter)
        {
            return true;
        }

        private void OnNew(object parameter)
        {
           // _documents.Add(new DocumentViewModel(this) { Document = new TextDocument() });
            _documents.Add(new DocumentViewModel(this));
            ActiveDocument = _documents.Last();
        }

        #endregion

        #region ActiveDocument

        private DocumentViewModel _activeDocument = null;
        public DocumentViewModel ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if ( _activeDocument != value )
                {
                    _activeDocument = value;
                    OnPropertyChanged(nameof(ActiveDocument));
                    ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ActiveDocumentChanged;

        #endregion

        internal void Close(DocumentViewModel fileToClose)
        {
            if ( fileToClose.IsDirty )
            {
                var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "Revit ScriptCS", MessageBoxButton.YesNoCancel);
                if ( res == MessageBoxResult.Cancel )
                    return;
                if ( res == MessageBoxResult.Yes )
                {
                    Save(fileToClose);
                }
            }
            _documents.Remove(fileToClose);
        }

        internal void Save(DocumentViewModel fileToSave, bool saveAsFlag = false)
        {
            if ( fileToSave.FilePath == null || saveAsFlag )
            {
                var dlg = new SaveFileDialog();
                dlg.Filter = "C# Script file (*.csx)|*.csx";
                if ( dlg.ShowDialog().GetValueOrDefault() )
                    fileToSave.FilePath = dlg.FileName;
            }

            File.WriteAllText(fileToSave.FilePath, fileToSave.Text);
            ActiveDocument.IsDirty = false;
        }

        internal void Run(DocumentViewModel documentViewModel)
        {
            _scriptRunnerHandler.ScriptText = documentViewModel.Text;
            _externalEvent.Raise();
        }
    }
}
