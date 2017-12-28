/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:SimpleMvvm_WindowsPhone"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using TvShowQuizz.Controllers;
using QuizzApp.Core.Controllers;
using QuizzApp.WP.ViewModels;
//using TvShowQuizz.ViewModels;
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
    public class WindowsPhoneViewModelLocator
    {

        private GameProvider gameProvider = new MQGameProvider();
 
        public WindowsPhoneViewModelLocator()
        {
        }

        public MainPageViewModel MainPageViewModel
        {
            get { return new MainPageViewModel(gameProvider); }
        }

        
        public PacksListViewModel PacksListViewModel
        {
            get { return new PacksListViewModel(gameProvider); }
        }

        public PopupSplashViewModel PopupSplashViewModel
        {
            get { return new PopupSplashViewModel(gameProvider); }
        }


        public LevelsListViewModel LevelsListViewModel
        {
            get { return new LevelsListViewModel(gameProvider); }
        }


        public GamePageViewModel GamePageViewModel
        {
            get { return new GamePageViewModel(gameProvider); }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return new SettingsViewModel(gameProvider); }
        }

        
        public HelpViewModel HelpViewModel
        {
            get { return new HelpViewModel(gameProvider); }
        }

    }
}