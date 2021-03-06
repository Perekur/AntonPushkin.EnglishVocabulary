using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Model;
using PushkinA.EnglishVocabulary.Services;
using System.Collections.ObjectModel;
using System.Linq;

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

        public MainViewModel(IDataService dataService, IDialogService dialogService)
        {
            var vm = new VocabulariesViewModel(dataService, dialogService);            
            ViewModel = vm;            
        }
    }
}