using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
    }

    public class DialogService : IDialogService
    {
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

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
