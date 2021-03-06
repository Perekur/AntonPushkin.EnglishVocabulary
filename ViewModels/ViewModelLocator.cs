/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PushkinA.EnglishVocabulary"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace PushkinA.EnglishVocabulary.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<Services.IDataService, Services.DataService>();
                SimpleIoc.Default.Register<Services.IDialogService, Services.DialogService>();
                SimpleIoc.Default.Register<Services.IDialogService, Services.DialogService>();
                SimpleIoc.Default.Register<Services.ITranslationService, Services.TranslationService>();
                SimpleIoc.Default.Register<Services.ISpeachService, Services.SpeachService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return CommonServiceLocator.ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public static T Resolve<T>() {
                return ServiceLocator.Current.GetInstance<T>();
        }

        public void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}