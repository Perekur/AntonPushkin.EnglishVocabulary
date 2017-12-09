using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Model;
using PushkinA.EnglishVocabulary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class VocabularyListViewModel : ViewModelBase
    {
        private readonly IDataService<VocabularyRecord> dataService;

        private readonly IDialogService dialogService;

        private ObservableCollection<VocabularyRecord> questionList;

        private readonly ISpeachService speachService;

        public ObservableCollection<VocabularyRecord> QuestionList
        {
            get
            {
                return questionList;
            }
            set
            {
                if (value != questionList)
                {
                    questionList = value;
                    RaisePropertyChanged(() => QuestionList);
                }
            }
        }

        private VocabularyRecord question;
        public VocabularyRecord Question
        {
            get { return question; }
            set
            {
                if (question != value)
                {
                    question = value;
                    RaisePropertyChanged(() => Question);
                    RaiseCanExecuteChanged();

                    if (IsSpeachWord && speachService != null && question!=null)
                        speachService.SpeachAsync(question.ForeignText);
                }
            }
        }

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    RaisePropertyChanged(() => FileName);
                    RaiseCanExecuteChanged();
                }
            }
        }


        private bool isForeignTextVisible=true;
        public bool IsForeignTextVisible
        {
            get { return isForeignTextVisible; }
            set
            {
                isForeignTextVisible = value;
                RaisePropertyChanged(() => IsForeignTextVisible);
            }
        }

        private bool isSpeachWord;

        public bool IsSpeachWord
        {
            get { return isSpeachWord; }
            set {
                isSpeachWord = value;
                RaisePropertyChanged(() => IsForeignTextVisible);
                RaiseCanExecuteChanged();
            }
        }


        private bool isTranslationVisible=true;
        public bool IsTranslationVisible
        {
            get { return isTranslationVisible; }
            set
            {
                isTranslationVisible = value;
                RaisePropertyChanged(() => IsTranslationVisible);
            }
        }


        private void RaiseCanExecuteChanged()
        {
            RaisePropertyChanged();

            NextCommand.RaiseCanExecuteChanged();
            PrevCommand.RaiseCanExecuteChanged();
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            DelCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public VocabularyListViewModel(IDataService<VocabularyRecord> dataService, IDialogService dialogService)
        {
            this.dataService = dataService;
            this.dialogService = dialogService;

            this.speachService = ViewModelLocator.Resolve<ISpeachService>();

            QuestionList = new ObservableCollection<VocabularyRecord>();

            NextCommand = new RelayCommand(NextCommandHandler, () => { return QuestionList.Contains(Question) && QuestionList.IndexOf(Question) < QuestionList.Count - 1; });
            PrevCommand = new RelayCommand(PrevCommandHandler, () => { return QuestionList.Contains(Question) && QuestionList.IndexOf(Question) > 0; });
            AddCommand = new RelayCommand(AddCommandHandler);            
            EditCommand = new RelayCommand(EditCommandHandler, () => Question != null);
            SaveCommand = new RelayCommand(SaveCommandHandler);
            RefreshCommand = new RelayCommand(RefreshCommandHandler);
            DelCommand = new RelayCommand(DeleteCommandHandler, () => Question != null);
            ParseFileCommand = new RelayCommand(ParseFileCommandHandler, () => QuestionList.Count==0);

            RaiseCanExecuteChanged();
        }

        private void ParseFileCommandHandler()
        {
            var vm = new ParseTextViewModel(SaveParsedWords);
            dialogService.ShowDialog(vm, "modalDialog");
            ParseFileCommand.RaiseCanExecuteChanged();
        }

        private void SaveParsedWords(VocabularyRecord[] items)
        {
            foreach (var item in items)
            {
                if (!QuestionList.Any(q => string.Compare(q.ForeignText, item.ForeignText, true) == 0))
                    QuestionList.Add(item);
            }
            SaveCommand.Execute(null);
        }

        private void RefreshCommandHandler()
        {
            QuestionList = new ObservableCollection<VocabularyRecord>(dataService.Get(FileName));
            RaiseCanExecuteChanged();
        }

        private void SaveCommandHandler()
        {
            dataService.Set(QuestionList.ToArray(), FileName);
            RaiseCanExecuteChanged();
        }

        private void AddCommandHandler()
        {
            var question = new VocabularyRecord() { ShowDateStart = DateTime.Now.Date, ShowDateEnd = DateTime.Now.Date.AddDays(3) };
            var vm = new VocabularyItemDialogViewModel(
                (item) => { QuestionList.Add(item); SaveCommand.Execute(null); }
            );
            vm.Question = question;
            dialogService.ShowDialog(vm, "modalDialog");
            RaiseCanExecuteChanged();
        }

        private void EditCommandHandler()
        {
            var vm = new VocabularyItemDialogViewModel(
                (item) => { SaveCommand.Execute(null); }
            );
            vm.Question = Question;
            dialogService.ShowDialog(vm, "modalDialog");
            RaiseCanExecuteChanged();
        }

        private void PrevCommandHandler()
        {
            int index = QuestionList.IndexOf(Question);
            if (index > 0) index--;
            Question = QuestionList[index];
            RaiseCanExecuteChanged();
        }

        private void NextCommandHandler()
        {
            int index = QuestionList.IndexOf(Question);
            if (index < QuestionList.Count - 1) index++;
            Question = QuestionList[index];
            RaiseCanExecuteChanged();
        }

        private void DeleteCommandHandler()
        {
            int index = QuestionList.IndexOf(Question);
            if (index >= 0 && index < QuestionList.Count)
                QuestionList.Remove(Question);
            SaveCommand.Execute(null);
            RaiseCanExecuteChanged();
        }

        public RelayCommand NextCommand      { get; private set; }
        public RelayCommand PrevCommand      { get; private set; }
        public RelayCommand AddCommand       { get; private set; }        
        public RelayCommand EditCommand      { get; private set; }
        public RelayCommand SaveCommand      { get; private set; }
        public RelayCommand DelCommand       { get; private set; }
        public RelayCommand RefreshCommand   { get; private set; }
        public RelayCommand ParseFileCommand { get; private set; }
    }
}
