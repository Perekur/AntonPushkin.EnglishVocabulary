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
    public class VocabularyItemDialogViewModel:DialogViewModelBase
    {
        private Action<VocabularyRecord> onSaveItem;
        private ITranslationService translationService;
        private ISpeachService speachService;

        public VocabularyItemDialogViewModel(Action<VocabularyRecord> onSaveItem)

        {
            Question = question;
            this.onSaveItem = onSaveItem;

            translationService = ViewModelLocator.Resolve<ITranslationService>();
            speachService = ViewModelLocator.Resolve<ISpeachService>();

            SaveCommand = new RelayCommand(SaveCommandHandler, ()=>Question!=null);
            CancelCommand = new RelayCommand(() => { Close(); });

            TranslateForeignCommand = new RelayCommand(TranslateForeignCommandHandler, () => Question != null);
            TranslateNativeCommand = new RelayCommand(TranslateNativeCommandHandler, () => Question != null);

            SpeakForeignCommand = new RelayCommand(SpeakForeignCommandHandler);
            SpeakNativeCommand = new RelayCommand(SpeakNativeCommandHandler);
        }

        private void SpeakNativeCommandHandler()
        {
            speachService.SpeachAsync(Question.NativeText);
        }

        private void SpeakForeignCommandHandler()
        {
            speachService.SpeachAsync(Question.ForeignText);
        }

        private async void TranslateForeignCommandHandler()
        {
            Question.ForeignText = translationService.Translate(Question.NativeText, "uk", "en") + (string.IsNullOrEmpty(Question.ForeignText) ? string.Empty : "\r\n") + Question.ForeignText;
        }

        private async void TranslateNativeCommandHandler()
        {
            Question.NativeText += translationService.Translate(Question.ForeignText, "en", "uk") + (string.IsNullOrEmpty(Question.NativeText) ? string.Empty : "\r\n") + Question.NativeText;
        }

        private void SaveCommandHandler()
        {
            if (onSaveItem != null)
            {
                onSaveItem.Invoke(Question);
                Close();
            }
        }

        private VocabularyRecord question;
        public VocabularyRecord Question
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

        public RelayCommand TranslateForeignCommand { get; private set; }
        public RelayCommand TranslateNativeCommand { get; private set; }

        public RelayCommand SpeakForeignCommand { get; private set; }
        public RelayCommand SpeakNativeCommand { get; private set; }

    }
}

