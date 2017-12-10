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
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using PushkinA.EnglishVocabulary.Services;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    public class MessageBoxViewModel : DialogViewModelBase
    {
        public System.Windows.Forms.DialogResult Result { get; private set; }

        public System.Windows.Forms.MessageBoxIcon Icon { get; set; }

        public System.Windows.Forms.MessageBoxButtons Buttons { get; set; }

        private string message;
        public string Message
        {
            get { return message; }
            set {
                if (message!=value)
                {
                    message = value;
                    RaisePropertyChanged(() => Message);
                }
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set {
                if (title != value)
                {
                    title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

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

    public static class MessageBox
    {
        public static DialogResult ShowDialog(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            return ShowDialog(System.Windows.Application.Current.MainWindow, message, title, buttons, icon);
        }

        public static DialogResult ShowDialog(Window parent, string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            var dialogService = ServiceLocator.Current.GetInstance<IDialogService>();

            var vm = new MessageBoxViewModel()
            {
                Message = message,
                Title = title,
                Buttons = buttons,
                Icon = icon
            };

            dialogService.ShowDialog(vm, "modalDialog");
            return vm.Result;
        }
    }
}

