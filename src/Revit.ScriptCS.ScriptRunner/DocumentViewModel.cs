using ICSharpCode.AvalonEdit.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Windows.Input;
using TextDocument = ICSharpCode.AvalonEdit.Document.TextDocument;

namespace Revit.ScriptCS.ScriptRunner
{
    public class DocumentViewModel : ViewModelBase
    {
        private bool _isReadOnly;

        RoslynEditorViewModel RoslynEditorViewModel {get;}

        public DocumentViewModel( RoslynEditorViewModel roslynEditorViewModel, string FilePath)
        {
            RoslynEditorViewModel = roslynEditorViewModel;
            this.FilePath = FilePath;
            Title = FileName;
        }

        public DocumentViewModel(RoslynEditorViewModel roslynEditorViewModel)
        {
            RoslynEditorViewModel = roslynEditorViewModel;
            IsDirty = true;
            Title = FileName;
        }

        internal void Initialize(DocumentId id)
        {
            Id = id;

            if ( _document != null && _document.TextLength > 0 )
            {
                var doc = RoslynEditorViewModel.Host.GetDocument(id);
                if ( doc != null )
                {
                    RoslynEditorViewModel.Host.UpdateDocument(doc.WithText(SourceText.From(_document.Text)));
                }
            }
        }

        public DocumentId Id { get; private set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            private set { SetProperty(ref _isReadOnly, value); }
        }

        private TextDocument _document = null;
        public TextDocument Document
        {
            get { return _document; }
            set { SetProperty(ref _document, value);}
        }

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private bool _isDirty = false;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if ( _isDirty != value )
                {
                    _isDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        private string _isReadOnlyReason = string.Empty;
        public string IsReadOnlyReason
        {
            get
            {
                return _isReadOnlyReason;
            }
            protected set
            {
                SetProperty(ref _isReadOnlyReason, value);
            }
        }
        private string _filePath = null;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if ( _filePath != value )
                {
                    _filePath = value;
                    
                    OnPropertyChanged(nameof(FilePath));
                    OnPropertyChanged(nameof(FileName));
                    OnPropertyChanged(nameof(Title));

                    if ( File.Exists(_filePath) )
                    {
                        _document = new TextDocument();
                        _isDirty = false;
                        IsReadOnly = false;
                        
                        // Check file attributes and set to read-only if file attributes indicate that
                        if ( (File.GetAttributes(_filePath) & FileAttributes.ReadOnly) != 0 )
                        {
                            IsReadOnly = true;
                            IsReadOnlyReason = "This file cannot be edit because another process is currently writting to it.\n" +
                                                    "Change the file access permissions or save the file in a different location if you want to edit it.";
                        }

                        using ( FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read) )
                        {
                            using ( StreamReader reader = FileReader.OpenStream(fs, System.Text.Encoding.UTF8) )
                            {
                                _document = new TextDocument(reader.ReadToEnd());
                            }
                        }

                        ContentId = _filePath;
                    }
                }
            }
        }

        public string FileName
        {
            get
            {
                if ( FilePath == null )
                    return "Untitled" + (IsDirty ? "*" : "");

                return Path.GetFileName(FilePath) + (IsDirty ? "*" : "");
            }
        }

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set
            {
                SetProperty(ref _contentId, value);
            }
        }

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
            }
        }
        public string Text { get; set; }

        public bool HasError { get; private set; }

        //private static MethodInfo HasSubmissionResult { get; } =
        //    typeof(Compilation).GetMethod(nameof(HasSubmissionResult), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


        RelayCommand _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if ( _saveCommand == null )
                {
                    _saveCommand = new RelayCommand((p) => OnSave(p), (p) => CanSave(p));
                }

                return _saveCommand;
            }
        }

        private bool CanSave(object parameter)
        {
            return IsDirty;
        }

        private void OnSave(object parameter)
        {
            Text = GetCode();
            RoslynEditorViewModel.Save(this, false);
        }

        private string GetCode()
        {
            var document = RoslynEditorViewModel.Host.GetDocument(Id);
            if ( document == null )
                return string.Empty;

            return document.GetTextAsync().Result.ToString();
        }


        RelayCommand _saveAsCommand = null;
        public ICommand SaveAsCommand
        {
            get
            {
                if ( _saveAsCommand == null )
                {
                    _saveAsCommand = new RelayCommand((p) => OnSaveAs(p), (p) => CanSaveAs(p));
                }

                return _saveAsCommand;
            }
        }

        private bool CanSaveAs(object parameter)
        {
            return IsDirty;
        }

        private void OnSaveAs(object parameter)
        {
            Text = GetCode();
            RoslynEditorViewModel.Save(this, true);
        }

        RelayCommand _closeCommand = null;
        public ICommand CloseCommand
        {
            get
            {
                if ( _closeCommand == null )
                {
                    _closeCommand = new RelayCommand((p) => OnClose(), (p) => CanClose());
                }

                return _closeCommand;
            }
        }

        private bool CanClose()
        {
            return true;
        }

        private void OnClose()
        {
           RoslynEditorViewModel.Close(this);
        }

        RelayCommand _runCommand = null;
        public ICommand RunCommand
        {
            get
            {
                if(_runCommand == null)
                {
                    _runCommand = new RelayCommand(p => OnRun(), p => CanRun());
                }
                return _runCommand;
            }
        }

        private bool CanRun()
        {
            return true;
        }

        private void OnRun()
        {
            Text = GetCode();
            RoslynEditorViewModel.Run(this);
        }
    }
}
