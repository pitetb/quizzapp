using TvShowQuizz.Resources;

namespace TvShowQuizz.Resources
{
    /// <summary>
    /// Permet d'accéder aux ressources de chaîne.
    /// </summary>
    public class LocalizedStrings
    {
        public AppResources AppResources { get; set; }

        public LocalizedStrings()
        {
            AppResources = new AppResources();
        }        
    }
}