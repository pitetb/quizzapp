using MovieQuizz2.Resources;

namespace MovieQuizz2.Resources
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