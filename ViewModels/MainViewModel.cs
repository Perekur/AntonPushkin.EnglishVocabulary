using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Model;
using PushkinA.EnglishVocabulary.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object _vm;
        public object ViewModel
        {
            get
            {
                return _vm;
            }
            set
            {
                if (_vm != value)
                {
                    _vm = value;
                    RaisePropertyChanged(() => ViewModel);
                }
            }
        }        

        public MainViewModel(IDataService<Question> dataService, IDialogService dialogService)
        {
            var files = dataService.GetFiles();
            var vocabularies = files.Select(file => new VocabularyListViewModel(dataService, dialogService) { FileName = file }).ToArray();

            var vm = new VocabulariesViewModel(dataService, dialogService);
            vm.Vocabularies = new ObservableCollection<VocabularyListViewModel>(vocabularies);
            ViewModel = vm;
        }
    }
}