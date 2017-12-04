using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Infrastructure;
using PushkinA.EnglishVocabulary.Model;
using PushkinA.EnglishVocabulary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class InputBoxViewModel : DialogViewModelBase
    {
        private Action<string> onSaveInput;

        public InputBoxViewModel(Action<string> onSaveInput = null)
        {
            this.onSaveInput = onSaveInput;

            SaveCommand = new RelayCommand(SaveCommandHandler);
            CancelCommand = new RelayCommand(() => { dialog.Close(); });
        }

        private string inputString;
        public string InputString {
            get { return inputString; }
            set {
                if (value != inputString)
                {
                    inputString = value;
                    RaisePropertyChanged(() => InputString);
                }
            }
        }

        public string Description { get; set; }

        private void SaveCommandHandler()
        {
            if (onSaveInput != null) onSaveInput(InputString);
            dialog.Close();
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
    }
}
