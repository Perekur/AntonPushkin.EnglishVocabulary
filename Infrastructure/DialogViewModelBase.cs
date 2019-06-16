using GalaSoft.MvvmLight;
using PushkinA.EnglishVocabulary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PushkinA.EnglishVocabulary.Infrastructure
{
    public class DialogViewModelBase: ViewModelBase, IDialogViewModel
    {
        public event EventHandler<EventArgs> CloseDialogRequested;
        public void OnClosing(object sender, EventArgs arg)
        {
        }

        public void OnLoaded(object sender, EventArgs arg)
        {
        }

        public string Title { get; }

        public void Close()
        {
            if (CloseDialogRequested!=null)
                CloseDialogRequested.Invoke(this, new EventArgs());
        }
    }
}
