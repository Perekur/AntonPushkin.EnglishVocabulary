using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PushkinA.EnglishVocabulary.AttachedProperties
{
    public static class AttachedWindowProps
    {
        public static readonly DependencyProperty WindowStartupLocationProperty;

        public static void SetWindowStartupLocation(DependencyObject DepObject, WindowStartupLocation value)
        {
            DepObject.SetValue(WindowStartupLocationProperty, value);
        }

        public static WindowStartupLocation GetWindowStartupLocation(DependencyObject DepObject)
        {
            return (WindowStartupLocation)DepObject.GetValue(WindowStartupLocationProperty);
        }

        static AttachedWindowProps()
        {
            WindowStartupLocationProperty = DependencyProperty.RegisterAttached("WindowStartupLocation",
                                                      typeof(WindowStartupLocation),
                                                      typeof(AttachedWindowProps),
                                                      new UIPropertyMetadata(WindowStartupLocation.Manual, OnWindowStartupLocationChanged));
        }

        private static void OnWindowStartupLocationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Window window = sender as Window;

            if (window != null)
            {
                window.WindowStartupLocation = GetWindowStartupLocation(window);
            }
        }
    }
}
