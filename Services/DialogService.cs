using PushkinA.EnglishVocabulary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PushkinA.EnglishVocabulary.Services
{
    public interface IDialogViewModel
    {
        void SetWindow(Window dialog);
        void Close();
    }

    public interface IDialogService
    {
        void ShowDialog<T>(T ViewModel, string dialogStyle="") where T: class, IDialogViewModel;

        DialogResult MessageBox(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None);

        string InputBox(string title, string stringValue = "", string description = "");
    }

    public class DialogService : IDialogService
    {
        public string InputBox(string title, string stringValue="", string description = "")
        {
            var retVal = string.Empty;
            var vm = new InputBoxViewModel((val) => retVal = val)
            {
                Description = description,
                StringValue = stringValue
            };
            ShowDialog(vm, "modalDialog");
            return retVal;
        }

        public DialogResult MessageBox(string message, string title = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            var vm = new MessageBoxViewModel()
            {
                Message = message,
                Title = title,
                Buttons = buttons,
                Icon = icon
            };
            ShowDialog(vm, "modalDialog");
            return vm.Result;
        }

        public void ShowDialog<T>(T ViewModel, string dialogStyle="") where T : class, IDialogViewModel
        {            
            var window = new Window();
            window.DataContext = ViewModel;
            ViewModel.SetWindow(window);
            
            var contentPresenter = new ContentControl();
            window.Content = contentPresenter;

            var binding = new System.Windows.Data.Binding();
            binding.RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.Self);
            binding.Path = new PropertyPath("DataContext");
            binding.Mode = System.Windows.Data.BindingMode.OneWay;

            contentPresenter.SetBinding(ContentControl.ContentProperty, binding);
            contentPresenter.Margin = new Thickness(6.0);

            var style = string.IsNullOrEmpty(dialogStyle) ? null: window.FindResource(dialogStyle) as Style;
            window.Style = style != null ? style: window.Style;

            window.Owner = System.Windows.Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
