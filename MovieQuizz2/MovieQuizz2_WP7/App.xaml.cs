using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using MovieQuizz2.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Threading;
using MovieQuizz2.Resources;
using QuizzApp.Tombstone;
using QuizzApp.Core.Helpers;

namespace MovieQuizz2
{
    public partial class App : Application
    {
                
        /// <summary>
        /// Permet d'accéder facilement au frame racine de l'application téléphonique.
        /// </summary>
        /// <returns>Frame racine de l'application téléphonique.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructeur pour l'objet Application.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // set
            //App.Current.Resources["LocalizedStrings"] = new LocalizedStrings() ;
            
            //// Determine language to use
            //CultureInfo cu = QuizzApp.Core.Helpers.Utils.DetermineLanguageToUse();
            //var toto = Thread.CurrentThread.CurrentUICulture;


            // Initialisation spécifique au téléphone
            InitializePhoneApplication();

            //ThemeManager.ToLightTheme();
            //this.ForceLightTheme();

            // Affichez des informations de profilage graphique lors du débogage.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Affichez les compteurs de fréquence des trames actuels.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Affichez les zones de l'application qui sont redessinées dans chaque frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // qui montre les zones d'une page sur lesquelles une accélération GPU est produite avec une superposition colorée.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Attention :- À utiliser uniquement en mode de débogage. Les applications qui désactivent la détection d'inactivité de l'utilisateur continueront de s'exécuter
                // et seront alimentées par la batterie lorsque l'utilisateur ne se sert pas du téléphone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // Code à exécuter lorsque l'application démarre (par exemple, à partir de Démarrer)
        // Ce code ne s'exécute pas lorsque l'application est réactivée
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            Debug.WriteLine("App Application_Launching");

            // Backup app lauching language
            QuizzApp.Core.Helpers.LanguagesUtils.BackupLanguageOnAppLaunch();

            // Determine language to use
            CultureInfo cu = QuizzApp.Core.Helpers.LanguagesUtils.DetermineLanguageToUse();
            var toto = Thread.CurrentThread.CurrentUICulture;

            // Notify PersistenceManager of launch
            PersistenceManager.Instance.OnLaunching();
        }

        // Code à exécuter lorsque l'application est activée (affichée au premier plan)
        // Ce code ne s'exécute pas lorsque l'application est démarrée pour la première fois
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            Debug.WriteLine("App Application_Activated");

            // Determine language to use
            CultureInfo cu = QuizzApp.Core.Helpers.LanguagesUtils.DetermineLanguageToUse();
            var toto = Thread.CurrentThread.CurrentUICulture;

            // Notify PersistenceManager of activation
            PersistenceManager.Instance.OnActivated(e.IsApplicationInstancePreserved);
        }

        // Code à exécuter lorsque l'application est désactivée (envoyée à l'arrière-plan)
        // Ce code ne s'exécute pas lors de la fermeture de l'application
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("App Application_Deactivated");
            // Notify PersistenceManager of deactivation
            PersistenceManager.Instance.OnDeactivated();
            Debug.WriteLine("App Application_Deactivated sucess");
        }

        // Code à exécuter lors de la fermeture de l'application (par exemple, lorsque l'utilisateur clique sur Précédent)
        // Ce code ne s'exécute pas lorsque l'application est désactivée
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("App Application_Closing");
            // Notify PersistenceManager of closing
            PersistenceManager.Instance.OnClosing();
        }

        // Code à exécuter en cas d'échec d'une navigation
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Échec d'une navigation ; arrêt dans le débogueur
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code à exécuter sur les exceptions non gérées
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is QuitException)
                return;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Une exception non gérée s'est produite ; arrêt dans le débogueur
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Initialisation de l'application téléphonique

        // Éviter l'initialisation double
        private bool phoneApplicationInitialized = false;

        // Ne pas ajouter de code supplémentaire à cette méthode
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Créez le frame, mais ne le définissez pas encore comme RootVisual ; cela permet à l'écran de
            // démarrage de rester actif jusqu'à ce que l'application soit prête pour le rendu.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.UriMapper = Resources["UriMapper"] as UriMapper;

            // Gérer les erreurs de navigation
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Garantir de ne pas retenter l'initialisation
            phoneApplicationInitialized = true;
        }

        // Ne pas ajouter de code supplémentaire à cette méthode
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Définir le Visual racine pour permettre à l'application d'effectuer le rendu
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Supprimer ce gestionnaire, puisqu'il est devenu inutile
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion


        
        

        


        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            _isInDesignMode
                = (bool)DependencyPropertyDescriptor
                .FromProperty(prop, typeof(FrameworkElement))
                .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode.Value;
            }
        }


    }
}