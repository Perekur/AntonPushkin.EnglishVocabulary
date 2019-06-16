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
using Application = System.Windows.Application;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class ParseTextViewModel : DialogViewModelBase
    {
        private Action<VocabularyRecord[]> onSaveParsedQuestions;

        public ParseTextViewModel(Action<VocabularyRecord[]> onSaveParsedQuestions)
        {
            this.onSaveParsedQuestions = onSaveParsedQuestions;


            SaveCommand = new RelayCommand(SaveCommandHandler);
            CancelCommand = new RelayCommand(() => { if (cts != null) cts.Cancel(); Close(); });

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

        private bool isSentenceParse=true;
        public bool IsSentenceParse
        {
            get { return isSentenceParse; }
            set
            {
                if (value != isSentenceParse)
                {
                    isSentenceParse = value;
                    RaisePropertyChanged(() => IsSentenceParse);
                }
            }
        }

        private void ParseFileCommandHandler()
        {
            cts = new CancellationTokenSource();
            if (isSentenceParse)
                Task.Run(new Action(ParseTextFileBySentences), cts.Token);
            else
                Task.Run(new Action(ParseTextFileByWords), cts.Token);
        }

        private void ParseTextFileBySentences()
        {
            CanChangeFileName = false;
            try
            {
                var vocabulary = new List<VocabularyRecord>();

                
                string dialogNum = "";
                string sentence = "";
                string txtOffset = "";

                SubtitleRowType rowType = SubtitleRowType.EmptyString;
                
                using (var sr = new StreamReader(FileName))
                {
                    while (!sr.EndOfStream && !cts.IsCancellationRequested)
                    {
                        var txtLine = sr.ReadLine();

                        rowType = rowType == SubtitleRowType.Text ? rowType = SubtitleRowType.EmptyString : (SubtitleRowType)((int)rowType + 1);
                        
                        switch (rowType)
                        {
                            case SubtitleRowType.EmptyString:
                                if (string.IsNullOrEmpty(txtLine))
                                {
                                    sentence = sentence.Replace("<i>", "");
                                    sentence = sentence.Replace("</i>", "");
                                    vocabulary.Add(new VocabularyRecord()
                                    {
                                        ForeignText = sentence,
                                        NativeText = txtOffset
                                    });
                                    sentence = "";
                                }
                                else
                                {
                                    sentence += " " + txtLine;
                                    rowType = SubtitleRowType.Text;
                                }
                                break;
                            case SubtitleRowType.Number:
                                sentence = txtLine;
                                break;
                            case SubtitleRowType.TimeOffset:
                                break;
                            case SubtitleRowType.Text:
                                if (!string.IsNullOrEmpty(sentence))
                                    sentence += (string.IsNullOrEmpty(sentence) ? "" : " ") + txtLine;
                                else
                                    rowType = SubtitleRowType.EmptyString;
                                break;
                        }                        
                    }

                    if (!string.IsNullOrEmpty(sentence))
                    {
                        sentence = sentence.Replace("<i>", "");
                        sentence = sentence.Replace("</i>", "");
                        vocabulary.Add(new VocabularyRecord() { ForeignText = sentence, NativeText = txtOffset });
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                        onSaveParsedQuestions(vocabulary.ToArray());
                        Close();
                    }));
                }
            }
            finally
            {
                CanChangeFileName = true;
            }
        }

        private void ParseTextFileByWords()
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

                    var questions = dicWords.OrderByDescending(i => i.Value).Select(q => new VocabularyRecord() { ForeignText = q.Key, NativeText = q.Value.ToString() }).ToArray();

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => {
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

        private bool IsTimeOffset(string line)
        {
            if (string.IsNullOrEmpty(line)) return false;
            return line.Contains("-->");
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

    enum SubtitleRowType
    {
        EmptyString = 0,
        Number = 1,
        TimeOffset = 2,
        Text = 3        
    }
}
