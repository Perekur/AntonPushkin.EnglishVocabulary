using GalaSoft.MvvmLight.Command;
using PushkinA.EnglishVocabulary.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Globalization;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class MessageBoxViewModel : DialogViewModelBase
    {
        public System.Windows.Forms.DialogResult Result { get; private set; }

        public System.Windows.Forms.MessageBoxIcon Icon { get; set; }

        public System.Windows.Forms.MessageBoxButtons Buttons { get; set; }

        public string Message { get; set; }

        public string Title { get; set; }

        public bool IsOkVisible
        {
            get
            {
                return Buttons == MessageBoxButtons.OK;
            }
        }

        public bool IsOkCancelVisible
        {
            get
            {
                return Buttons == MessageBoxButtons.OKCancel;
            }
        }

        public bool IsAbortRetryIgnoreVisible
        {
            get
            {
                return Buttons == MessageBoxButtons.AbortRetryIgnore;
            }
        }

        public bool IsRetryCancelVisible
        {
            get
            {
                return Buttons == MessageBoxButtons.RetryCancel;
            }
        }

        public bool IsYesNoVisible
        {
            get
            {
                return Buttons == MessageBoxButtons.YesNo;
            }
        }

        public bool IsYesNoCancelVisible
        {
            get
            {
                return Buttons == MessageBoxButtons.YesNoCancel;
            }
        }

        public MessageBoxViewModel()
        {
            Result = DialogResult.None;
            ClickCommand = new RelayCommand<System.Windows.Forms.DialogResult>(ClickCommandHandle);
        }

        private void ClickCommandHandle(DialogResult obj)
        {
            Result = obj;
            Close();
        }

        public RelayCommand<System.Windows.Forms.DialogResult> ClickCommand { get; private set; }
    }
}

