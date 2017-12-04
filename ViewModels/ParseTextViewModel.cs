using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Infrastructure;
using PushkinA.EnglishVocabulary.Model;
using PushkinA.EnglishVocabulary.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class ParseTextViewModel : DialogViewModelBase
    {
        private Action<Question[]> onSaveParsedQuestions;

        public ParseTextViewModel(Action<Question[]> onSaveParsedQuestions)
        {
            this.onSaveParsedQuestions = onSaveParsedQuestions;


            SaveCommand = new RelayCommand(SaveCommandHandler);
            CancelCommand = new RelayCommand(() => { if (cts != null) cts.Cancel(); dialog.Close(); });

            BrowseFileCommand = new RelayCommand(BrowseFileCommandHandler, ()=> CanChangeFileName);
            ParseFileCommand = new RelayCommand(ParseFileCommandHandler, ()=> !string.IsNullOrEmpty(FileName));

            CanChangeFileName = true;
        }

        private CancellationTokenSource cts;

        private bool canChangeFileName;
        public bool CanChangeFileName
        {
            get
            {
                return canChangeFileName;
            }
            private set
            {
                if (value != canChangeFileName)
                {
                    canChangeFileName = value;
                    RaisePropertyChanged(() => CanChangeFileName);                    
                }
            }
        }

        private void ParseFileCommandHandler()
        {
            cts = new CancellationTokenSource();
            Task.Run(new Action(ParseTextFile), cts.Token);
        }

        private void ParseTextFile()
        {
            CanChangeFileName = false;
            try
            {
                using (var sr = new StreamReader(FileName))
                {
                    var dicWords = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

                    while(!sr.EndOfStream && !cts.IsCancellationRequested)
                    {
                        var txtLine = sr.ReadLine();
                        var words = txtLine.Split(new char[] { ' ', ',', '.', ';', '!', '?'}, StringSplitOptions.RemoveEmptyEntries);                                               

                        foreach (var w in words)
                        {
                            if (!IsWord(w)) continue;

                            if (dicWords.ContainsKey(w))
                                dicWords[w] = dicWords[w] + 1;
                            else
                                dicWords.Add(w, 1);
                        }
                    }                   

                    var questions = dicWords.OrderByDescending(i => i.Value).Select(q => new Question() { ForeignText = q.Key, NativeText = q.Value.ToString() }).ToArray();

                    dialog.Dispatcher.BeginInvoke(new Action(() => {
                        onSaveParsedQuestions(questions);
                        Close();
                    }));                                       
                }
            }
            finally
            {
                CanChangeFileName = true;                
            }
        }

        private bool IsWord(string word)
        {
            if (string.IsNullOrEmpty(word)) return false;
            return Char.IsLetter(word[0]) && Char.IsLetter(word[word.Length - 1]);
        }

        private void BrowseFileCommandHandler()
        {
            using (var dlgFileOpen = new OpenFileDialog())
            {
                dlgFileOpen.Filter = "Text files|*.txt;*.csv;*.srt|All files|*.*";
                if (dlgFileOpen.ShowDialog() == DialogResult.OK)
                {
                    FileName = dlgFileOpen.FileName;
                }
            }
        }

        public Dictionary<string, int> Words;

        private string fileName;
        public string FileName { get { return fileName; } set { if (value != fileName) { fileName=value; RaisePropertyChanged(() => FileName); BrowseFileCommand.RaiseCanExecuteChanged(); ParseFileCommand.RaiseCanExecuteChanged(); } } }

        private void SaveCommandHandler()
        {
            throw new NotImplementedException();
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public RelayCommand BrowseFileCommand { get; private set; }
        public RelayCommand ParseFileCommand { get; private set; }
    }
}
