﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Model;
using PushkinA.EnglishVocabulary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
                    if (vocabularies.Count > 0)
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
                    if (selectedVocabularyList!=null)
                        selectedVocabularyList.RefreshCommand.Execute(null);
                }
            }
        }

        private bool isForeignTextVisible = true;
        public bool IsForeignTextVisible
        {
            get { return isForeignTextVisible; }
            set
            {
                isForeignTextVisible = value;

                foreach (var v in vocabularies)
                {
                    v.IsForeignTextVisible = value;
                }

                RaisePropertyChanged(() => IsForeignTextVisible);
            }
        }

        private bool isTranslationVisible = true;
        public bool IsTranslationVisible
        {
            get { return isTranslationVisible; }
            set
            {
                isTranslationVisible = value;

                foreach (var v in vocabularies)
                {
                    v.IsTranslationVisible = value;
                }

                RaisePropertyChanged(() => IsTranslationVisible);
            }
        }

        private bool isSpeachWord;

        public bool IsSpeachWord
        {
            get { return isSpeachWord; }
            set
            {
                isSpeachWord = value;
                foreach (var v in vocabularies)
                {
                    v.IsSpeachWord = value;
                }
                RaisePropertyChanged(() => IsTranslationVisible);
            }
        }


        private readonly IDialogService dialogService;
        private readonly IDataService dataService;

        public VocabulariesViewModel(IDataService dataService, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.dataService = dataService;
            this.Vocabularies = new ObservableCollection<VocabularyListViewModel>();

            AddVocabularyCommand = new RelayCommand(AddVocabularyCommandHandler);
            RenameVocabularyCommand = new RelayCommand(RenameVocabularyCommandHandler);
            RemoveVocabularyCommand = new RelayCommand<VocabularyListViewModel>(RemoveVocabularyCommandHandler);
            RefreshCommand = new RelayCommand(Refresh);
        }

        private void Refresh()
        {
            var files = dataService.GetFiles();
            Vocabularies.Clear();
            foreach (var file in files)
            {
                Vocabularies.Add(new VocabularyListViewModel(dataService, dialogService) { FileName = file });
            };            
        }

        private void RemoveVocabularyCommandHandler(VocabularyListViewModel vocabulary)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                MessageBox.ShowDialog("Need use modifiers");
                return;
            }

            if (vocabulary != null)
            {
                if (MessageBox.ShowDialog("Are you sure want delete vocabulary?","Delete", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        DataService.Delete(vocabulary.FileName);
                        Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.ShowDialog(ex.Message, ex.GetType().Name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RenameVocabularyCommandHandler()
        {
            var newName = InputBox.ShowDialog("New name of vocabulary:", SelectedVocabularyList.FileName, "Rename");
            if (string.IsNullOrEmpty(newName)) return;

            try
            {
                DataService.Rename(SelectedVocabularyList.FileName, newName);
                SelectedVocabularyList.FileName = newName;
            }
            catch (Exception ex)
            {
                MessageBox.ShowDialog(ex.Message, ex.GetType().Name, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void AddVocabularyCommandHandler()
        {
            int weekOfYear = DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(DateTime.Now, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            string defaultName = string.Format("{0:MMdd}.Vocabulary{0:yyyy}.{1:0#}", DateTime.Now, weekOfYear);

            var strVocabularyName = InputBox.ShowDialog("Please, insert name of new Vocabulary", defaultName);
            if (string.IsNullOrEmpty(strVocabularyName)) return;

            if (Vocabularies.Any(v => string.Compare(v.FileName, strVocabularyName, true) == 0))
                MessageBox.ShowDialog(string.Format("Vocabulary '{0}' already exists.", strVocabularyName), "Warning");
            else
            {
                Vocabularies.Add(new VocabularyListViewModel(dataService, dialogService) { FileName = strVocabularyName });
                dataService.Set(new VocabularyRecord[0], strVocabularyName);
            }

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
        public RelayCommand RenameVocabularyCommand { get; private set; }
        public RelayCommand<VocabularyListViewModel> RemoveVocabularyCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
    }
}
