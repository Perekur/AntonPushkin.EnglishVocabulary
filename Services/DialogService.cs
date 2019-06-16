using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PushkinA.EnglishVocabulary.Services
{

    public interface IDialogViewModel
    {
        event EventHandler<EventArgs> CloseDialogRequested;
        void OnClosing(object sender, EventArgs arg);
        void OnLoaded(object sender, EventArgs arg);
        string Title { get; }
    }

    public interface IDialogService
    {
        void ShowDialog(IDialogViewModel viewModel, bool resizeable = false, string style = "DialogWindow");
        void ShowInContainer(IDialogViewModel viewModel, bool onlyOneInstance = true);
    }

    public class DialogService : IDialogService
    {
        private readonly IContainerViewModel _containerVm;

        //public DialogService(IContainerViewModel containerVm)
        //{
        //    _containerVm = containerVm ?? throw new ArgumentNullException(nameof(containerVm));
        //}

        public void ShowDialog(IDialogViewModel viewModel, bool resizeable, string style)
        {
            var window = new Window
            {
                Content = viewModel,
                Title = viewModel.Title ?? string.Empty,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
                ResizeMode = resizeable ? ResizeMode.CanResize : ResizeMode.NoResize,
            };

            window.Style = (Style)window.FindResource(style);

            viewModel.CloseDialogRequested += (s, args) => { window.Close(); };
            window.Closing += (s, arg) => { viewModel.OnClosing(s, arg); };
            window.Loaded += (s, arg) => { viewModel.OnLoaded(s, arg); };

            window.ShowDialog();
        }

        public void ShowInContainer(IDialogViewModel viewModel, bool onlyOneInstance = true)
        {
            if (_containerVm.Items.Any(i => i.Title == viewModel.Title))
                _containerVm.SelectedItem = _containerVm.Items.First(i => i.Title == viewModel.Title);
            else
                _containerVm.AddChildItemViewModel(viewModel);
            viewModel.CloseDialogRequested += (s, args) => { _containerVm.RemoveChildItemViewModel(viewModel); };

            //window.Closing += (s, arg) => { viewModel.OnClosing(s, arg); };
            //window.Loaded += (s, arg) => { viewModel.OnLoaded(s, arg); };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IContainerViewModel
    {
        void AddChildItemViewModel(IDialogViewModel viewModel);

        IDialogViewModel SelectedItem { get; set; }

        ObservableCollection<IDialogViewModel> Items { get; }

        void RemoveChildItemViewModel(IDialogViewModel viewModel);
    }
}
