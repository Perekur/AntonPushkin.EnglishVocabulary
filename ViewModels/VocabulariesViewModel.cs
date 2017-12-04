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
    public class VocabulariesViewModel : ViewModelBase
    {
        private ObservableCollection<VocabularyListViewModel> vocabularies;
        public ObservableCollection<VocabularyListViewModel> Vocabularies
        {
            get { return vocabularies; }
            set
            {
                if (value != vocabularies)
                {
                    vocabularies = value;
                    RaisePropertyChanged(() => Vocabularies);
                    if (vocabularies.Count>0)
                    SelectedVocabularyList = vocabularies[0];
                }
            }
        }

        private VocabularyListViewModel selectedVocabularyList;
        public VocabularyListViewModel SelectedVocabularyList
        {
            get { return selectedVocabularyList; }
            set
            {
                if (value != selectedVocabularyList)
                {
                    selectedVocabularyList = value;
                    RaisePropertyChanged(() => SelectedVocabularyList);
                    selectedVocabularyList.RefreshCommand.Execute(null);
                }
            }
        }
        
        private bool isTestMode;
        public bool IsTestMode
        {
            get { return isTestMode; }
            set { isTestMode = value;

                foreach(var v in vocabularies)
                {
                    v.IsTestMode = value;
                }

                RaisePropertyChanged(() => IsTestMode);
            }
        }

        private readonly IDialogService dialogService;
        private readonly IDataService<Question> dataService;

        public VocabulariesViewModel(IDataService<Question> dataService, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.dataService = dataService;

            AddVocabularyCommand = new RelayCommand(AddVocabularyCommandHandler);
        }

        private void AddVocabularyCommandHandler()
        {
            var vm = new InputBoxViewModel(CreateVocabularyList);
            vm.Description = "Please, enter file name of new vocabulary list";
            dialogService.ShowDialog(vm, "modalDialog");
            AddVocabularyCommand.RaiseCanExecuteChanged();
        }

        private void CreateVocabularyList(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            if (Vocabularies.Any(v => string.Compare(v.FileName, fileName, true) == 0)) return;

            var vocabularyList = new VocabularyListViewModel(dataService, dialogService) { FileName = fileName };
            Vocabularies.Add(vocabularyList);
            vocabularyList.SaveCommand.Execute(null);
        }

        public RelayCommand AddVocabularyCommand { get; private set; }
    }
}
