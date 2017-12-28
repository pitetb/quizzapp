/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SimpleMvvm_WindowsPhone"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using QuizzApp.WP.ViewModels;
using QuizzApp.WP.ViewModels.MQKeyboard;
// Toolkit namespace
using SimpleMvvmToolkit;

namespace TvShowQuizz.Locators
{
    /// <summary>
    /// This class creates ViewModels on demand for Views, supplying a
    /// ServiceAgent to the ViewModel if required.
    /// <para>
    /// Place the ViewModelLocator in the App.xaml resources:
    /// </para>
    /// <code>
    /// &lt;Application.Resources&gt;
    ///     &lt;vm:ViewModelLocator xmlns:vm="clr-namespace:SimpleMvvm_WindowsPhone"
    ///                                  x:Key="Locator" /&gt;
    /// &lt;/Application.Resources&gt;
    /// </code>
    /// <para>
    /// Then use:
    /// </para>
    /// <code>
    /// DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
    /// </code>
    /// <para>
    /// Use the <strong>mvvmlocator</strong> or <strong>mvvmlocatornosa</strong>
    /// code snippets to add ViewModels to this locator.
    /// </para>
    /// </summary>
    public class WindowsPhoneControlsViewModelLocator
    {


        public WindowsPhoneControlsViewModelLocator()
        {
        }

        public ThreePosterViewModel ThreePosterViewModel
        {
            get { return new ThreePosterViewModel(); }
        }

        public PackagePresenterViewModel PackagePresenterViewModel
        {
            get { return new PackagePresenterViewModel(); }
        }

        public AdBannerControlViewModel AdBannerControlViewModel
        {
            get { return new AdBannerControlViewModel(); }
        }

        public KeyboardLetterControlViewModel KeyboardLetterControlViewModel
        {
            get { return new KeyboardLetterControlViewModel(); }
        }
        
        public KeyboardControlViewModel KeyboardControlViewModel
        {
            get { return new KeyboardControlViewModel(); }
        }

    }
}