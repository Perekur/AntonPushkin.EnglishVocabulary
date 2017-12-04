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

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class VocabularyItemViewModel:DialogViewModelBase
    {
        private Action<Question> onSaveItem;

        public VocabularyItemViewModel(Action<Question> onSaveItem)

        {
            Question = question;
            this.onSaveItem = onSaveItem;

            SaveCommand = new RelayCommand(SaveCommandHandler, ()=>Question!=null);
            CancelCommand = new RelayCommand(() => { Close(); });
        }

        private void SaveCommandHandler()
        {
            if (onSaveItem != null)
            {
                onSaveItem.Invoke(Question);
                Close();
            }
        }

        private Question question;
        public Question Question
        {
            get
            {
                return question;
            }
            set
            {
                if (question != value)
                {
                    question = value;
                    RaisePropertyChanged(() => Question);
                }
            }
        }


        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
    }
}
