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
        protected Window dialog;

        public void Close()
        {
            if (dialog != null)
                dialog.Close();
        }

        public void SetWindow(Window dialog)
        {
            this.dialog = dialog;
        }
    }
}
