using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Web;
using System.Windows.Input;

namespace CrossNote.ViewModels
{
    internal class NoteViewModel : ObservableObject, IQueryAttributable
    {
        #region Fields

        private Models.Note _note;

        public DateTime Date => _note.Date;

        public string Identifier => _note.Filename;

        #endregion

        #region Properties

        public string PreviewSource
        {
            get => GetPreviewSource(_note?.Text);
        }

        public string EditorSource
        {
            get => GetEditorHtmlSource(_note?.Text);
        }

        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        #endregion

        #region Constructors

        public NoteViewModel()
        {
            _note = new Models.Note();
            SaveCommand = new AsyncRelayCommand<WebView>(Save);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }

        public NoteViewModel(Models.Note note)
        {
            _note = note;
            SaveCommand = new AsyncRelayCommand<WebView>(Save);
            DeleteCommand = new AsyncRelayCommand(Delete);
        }

        #endregion

        #region Methods

        private async Task Save(WebView? editorWebView)
        {
            if (editorWebView != null)
            {
                var result = await editorWebView.EvaluateJavaScriptAsync($"getEditorContent()");
                _note.Text = HttpUtility.UrlDecode(result);
            }
            _note.Date = DateTime.Now;
            _note.Save();
            await Shell.Current.GoToAsync($"..?saved={_note.Filename}");
        }

        private async Task Delete()
        {
            _note.Delete();
            await Shell.Current.GoToAsync($"..?deleted={_note.Filename}");
        }

        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                _note = Models.Note.Load(query["load"].ToString());
                RefreshProperties();
            }
        }

        public void Reload()
        {
            _note = Models.Note.Load(_note.Filename);
            RefreshProperties();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(PreviewSource));
            OnPropertyChanged(nameof(EditorSource));
            OnPropertyChanged(nameof(Date));
        }

        private string GetEditorHtmlSource(string? prevData)
        {
            return @"<html>
                 <head>
                     <meta name=""viewport"" width=""device-width,"" initial-scale=""1"">
                     <link href='https://cdn.quilljs.com/1.3.6/quill.snow.css' rel='stylesheet'>
                 
                 <style>
                    div.sticky {
                      position: -webkit-sticky;
                      position: sticky;
                      top: 0;
                      padding: 5px;
                      background-color: black;
                      z-index: 1;
                    }
                    
                    .ql-snow .ql-stroke{
                        stroke: white;
                    }
                    
                    .ql-snow .ql-picker{
                        color: white;
                    }
                    
                    .ql-snow .ql-picker-options{
                        background-color: black;
                    }
                 </style>
                 </head>
                 <body onload=""initialize()"" style='height: fit-content;'>
                     <div id='editor' style='border: none; height: fit-content;'>
                     </div>
                 
                     <script src='https://cdn.quilljs.com/1.3.6/quill.js'></script>
                 
                     <script type=""text/javascript"">
                         var quill = null;

                         var prevData = " + (String.IsNullOrWhiteSpace(prevData) ? @"null" : prevData) + @";
                 
                         function initialize() {
                             if (quill === null) {
                                 // Resolve reload issue
                                 document.getElementsByClassName('ql-toolbar ql-snow')[0]?.remove();

                                 //document.getElementsByClassName('ql-toolbar ql-snow')[0].classList.add('sticky');

                                 quill = new Quill('#editor', {
                                     theme: 'snow'
                                 });

                                 document.getElementsByClassName('ql-toolbar ql-snow')[0].classList.add('sticky');

                                 if (prevData !== null) {
                                     quill.setContents(prevData);
                                 }
                             }
                         }

                         function getEditorContent(){
                             if(quill !== null){
                                return encodeURIComponent(JSON.stringify(quill.editor.delta.ops));
                             }else return '';
                         }
                     </script>
                 </body>
                 </html>";
        }

        private string GetPreviewSource(string? prevData)
        {
            return @"<html>
                 <head>
                     <meta name=""viewport"" width=""device-width,"" initial-scale=""1"">
                     <link href='https://cdn.quilljs.com/1.3.6/quill.snow.css' rel='stylesheet'>
                 <style>
                    .body{
                        overflow-y: hidden !important;
                    }

                    .ql-editor{
                        overflow-y: hidden !important;
                    }

                    .ql-editor > * {
                        cursor: default;
                    }
                 </style>
                 </head>
                 <body onload=""initialize()"" style='height: fit-content;'>
                     <div id='editor' style='border: none; height: 70px;'>
                     </div>
                 
                     <script src='https://cdn.quilljs.com/1.3.6/quill.js'></script>
                 
                     <script type=""text/javascript"">
                         var quill = null;

                         var prevData = " + (String.IsNullOrWhiteSpace(prevData) ? @"null" : prevData) + @";
                 
                         function initialize() {
                             if (quill === null) {
                                 quill = new Quill('#editor', {
                                     theme: 'bubble'
                                 });

                                 quill.enable(false);

                                 if (prevData !== null) {
                                     quill.setContents(prevData);
                                 }
                             }
                         }
                     </script>
                 </body>
                 </html>";
        }

        #endregion
    }
}
