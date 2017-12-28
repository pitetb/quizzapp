using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.WP.ViewModels.MQKeyboard
{
    public class KeyboardLetterControlViewModel : ViewModelBase<KeyboardLetterControlViewModel>
    {

        public KeyboardLetterControlViewModel()
        {
            this.Letter = 'V';
        }

        private char letter;
        public char Letter
        {
            get { return letter; }
            private set
            {
                letter = value;
                NotifyPropertyChanged(m => m.Letter);
            }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                NotifyPropertyChanged(m => m.IsVisible);
            }
        }

        public event EventHandler<NotificationEventArgs> ReplaceLetterAnimation;

        // Helper method to notify View of an error
        private void NotifyReplaceLetterAnimation()
        {
            // Notify view of an error
            Notify(ReplaceLetterAnimation, new NotificationEventArgs());
        }

        public void ReplaceLetter(char newLetter, bool doAnimation)
        {
            if (doAnimation)
                NotifyReplaceLetterAnimation();
            this.Letter = newLetter;
        }

                
        
        public event EventHandler<NotificationEventArgs> LaunchBlink;
        private void NotifyLaunchBlinkAnimation()
        {
            if (LaunchBlink != null)
                Notify(LaunchBlink, new NotificationEventArgs());
        }

        public void MakeBlink()
        {
            this.NotifyLaunchBlinkAnimation();
        }

                
        public event EventHandler<NotificationEventArgs<char>> LetterClicked;

        public void OnLetterClicked()
        {
            this.NotifyLetterClicked(this.Letter);
        }

        private void NotifyLetterClicked(char tapedChar)
        {
            if (this.LetterClicked != null)
                Notify(LetterClicked, new NotificationEventArgs<char>(string.Empty, tapedChar));
        }

    }
}
