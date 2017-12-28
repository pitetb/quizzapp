using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Collections.ObjectModel;
using QuizzApp.Core.Helpers;

namespace QuizzApp.WP.ViewModels.MQKeyboard
{
    public class KeyboardControlViewModel : ViewModelBaseWithDesign<KeyboardControlViewModel>
    {

        public KeyboardControlViewModel()
        {
            this.DefaultBackgroundLetterBrush = new SolidColorBrush(Color.FromArgb(255, 255, 187, 51)); //
            this.GoodLetterBackgroundColorBrush = new SolidColorBrush(Color.FromArgb(255, 153, 204, 0));
            this.BadLetterBackgroundColorBrush = new SolidColorBrush(Color.FromArgb(255, 255, 68, 68));
            this.titleAreaBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 136, 0));
            this.NbTotalLetters = 14;
            this.IsTitleFound = false;
            this.nbSecondsForHelp = 10;

            if (this.IsInDesignMode)
            {
                this.TitleLetters = new List<TitleLetterModel>();
                TitleLetterModel letterModel = new TitleLetterModel();
                letterModel.Letter = 'A';
                this.SetTitleLetterState(letterModel, TitleLetterState.LetterSetted);
                this.TitleLetters.Add(letterModel);
            }

        }

        private string title;
        public string Title
        {
            get { return title; }
            private set
            {
                title = value;
                NotifyPropertyChanged(m => m.Title);
            }
        }

        private List<TitleLetterModel> titleLetters;
        public List<TitleLetterModel> TitleLetters
        {
            get { return titleLetters; }
            set
            {
                titleLetters = value;
                NotifyPropertyChanged(m => m.TitleLetters);
            }
        }

        private List<KeyboardLetterControlViewModel> keysViewModels;
        public List<KeyboardLetterControlViewModel> KeysViewModels
        {
            get { return keysViewModels; }
            set
            {
                if (keysViewModels != null)
                {
                    foreach (var item in keysViewModels)
                        item.LetterClicked -= keyLetterClicked;
                }

                keysViewModels = value;
                NotifyPropertyChanged(m => m.KeysViewModels);

                if (keysViewModels != null)
                {
                    foreach (var item in keysViewModels)
                        item.LetterClicked += keyLetterClicked;
                }
            }
        }




        private int nbTotalLetters;
        public int NbTotalLetters
        {
            get { return nbTotalLetters; }
            set
            {
                nbTotalLetters = value;
                NotifyPropertyChanged(m => m.NbTotalLetters);
            }
        }


        private Brush titleAreaBackgroundBrush;
        public Brush TitleAreaBackgroundBrush
        {
            get { return titleAreaBackgroundBrush; }
            set
            {
                titleAreaBackgroundBrush = value;
                NotifyPropertyChanged(m => m.TitleAreaBackgroundBrush);
            }
        }

        private Brush defaultBackgroundColorBrush;
        public Brush DefaultBackgroundLetterBrush
        {
            get { return defaultBackgroundColorBrush; }
            set
            {
                defaultBackgroundColorBrush = value;
                NotifyPropertyChanged(m => m.DefaultBackgroundLetterBrush);
            }
        }

        private Brush goodLetterBackgroundColorBrush;
        public Brush GoodLetterBackgroundColorBrush
        {
            get { return goodLetterBackgroundColorBrush; }
            set
            {
                goodLetterBackgroundColorBrush = value;
                NotifyPropertyChanged(m => m.GoodLetterBackgroundColorBrush);
            }
        }

        private Brush badLetterBackgroundColorBrush;
        public Brush BadLetterBackgroundColorBrush
        {
            get { return badLetterBackgroundColorBrush; }
            set
            {
                badLetterBackgroundColorBrush = value;
                NotifyPropertyChanged(m => m.BadLetterBackgroundColorBrush);
            }
        }


        private List<char> randomLetters;
        public List<char> RandomLetters
        {
            get { return randomLetters; }
            set
            {
                randomLetters = value;
                NotifyPropertyChanged(m => m.RandomLetters);
            }
        }

        private bool isTitleFound;
        public bool IsTitleFound
        {
            get { return isTitleFound; }
            set
            {
                if (value != isTitleFound)
                {
                    bool oldValue = isTitleFound;

                    isTitleFound = value;
                    NotifyPropertyChanged(m => m.IsTitleFound);

                    if (isTitleFound && ! isOnInitialization)
                        this.NotifyGoodAnswer();
                }
            }
        }



        private int nbSecondsForHelp;

        private int currentRandomLettersIndex;
        private List<string> words;
        private int currentWordsIndex;
        private string normalizedAnswer;
        private string currentAnswer;
        private int nbLettersFound;
        private QuizzAppCheckWordResult lastCheckWordResult;

        private List<KeyboardLetterControlViewModel> chosenLetterButtonKeys;
        private List<char> chosenLetterButtonValues;


        private bool isOnInitialization = false;
        public void InitializeTitle(string title, bool isAlreadyFound, int nbSecondsForHelp)
        {
            this.isOnInitialization = true;
            this.Title = title;
            this.nbSecondsForHelp = nbSecondsForHelp;

            if (string.IsNullOrEmpty(title))
                return;

            // Normalized the title
            // We remove accents
            // We upper case
            this.normalizedAnswer = StringTools.RemoveAccents(title).ToUpper();
            Debug.WriteLine("title with no accents : " + this.normalizedAnswer);

            // We split on all characters that are not 'abcdedfghi'
            // Split on all non-word characters.
            this.words = this.SplitIntoWords(this.normalizedAnswer);
            foreach (var item in words)
                Debug.WriteLine("words : " + item);

            // Create real Letters char list
            List<char> realLetters = new List<char>();
            words.ForEach(f => realLetters.AddRange(f.ToCharArray()));


            // Case title already found
            if (isAlreadyFound)
            {
                this.PrepareTitleLetters();

                foreach (var item in this.normalizedAnswer)
                {
                    // Update title view
                    this.TitleLetters[CurrentTextInputIndex].Letter = item;
                    if (!StringTools.IsOnQuizzAppAlphabet(item))
                    {
                        this.SetTitleLetterState(this.TitleLetters[CurrentTextInputIndex], TitleLetterState.StaticLetter);
                    }
                    else
                    {
                        this.SetTitleLetterState(this.TitleLetters[CurrentTextInputIndex], TitleLetterState.LetterSettedAndValid);
                    }
                    this.CurrentTextInputIndex++;
                }
                this.IsTitleFound = true;
            }
            else
            {

                // Create fake letters char list
                int nbRandomLetters = (int)Math.Floor((float)(realLetters.Count / 4));
                int nbChosenLetters = (realLetters.Count + nbRandomLetters);
                if (nbChosenLetters < this.nbTotalLetters)
                {
                    nbRandomLetters += (nbTotalLetters - nbChosenLetters);
                }

                List<char> fakeLetters = new List<char>(nbRandomLetters);
                for (int i = 0; i < nbRandomLetters; i++)
                {
                    fakeLetters.Add(StringTools.GetRandomLetterUpperCase());
                }

                // Create randoms letters tab
                List<char> allLetters = new List<char>(realLetters.Count + fakeLetters.Count);
                allLetters.AddRange(realLetters);

                // Insert randomly fake letters
                Random randomer = new Random();
                for (int i = 0; i < fakeLetters.Count; i++)
                {
                    int insertIndex = randomer.Next(0, allLetters.Count - 1);
                    allLetters.Insert(insertIndex, fakeLetters[i]);
                }

                // randomize first Nbitems
                var shuffledLetters = allLetters.Take(this.NbTotalLetters).OrderBy(a => randomer.NextDouble()).ToList();


                // Create random final tab
                List<char> randomizedLetters = new List<char>(allLetters.Count);
                for (int i = 0; i < allLetters.Count; i++)
                {
                    if (i < shuffledLetters.Count)
                        randomizedLetters.Insert(i, shuffledLetters[i]);
                    else
                        randomizedLetters.Insert(i, allLetters[i]);
                }


                this.RandomLetters = randomizedLetters;
                this.currentRandomLettersIndex = this.NbTotalLetters - 1;

                // Prepare keyboard letters with random letters
                for (int i = 0; i < this.KeysViewModels.Count; i++)
                {
                    if (i < this.RandomLetters.Count)
                    {
                        char aChar = this.RandomLetters[i];
                        this.KeysViewModels[i].ReplaceLetter(aChar, false);
                        this.KeysViewModels[i].IsVisible = true;
                    }
                }


                // Reinit variables
                this.chosenLetterButtonKeys = new List<KeyboardLetterControlViewModel>(this.RandomLetters.Count);
                this.chosenLetterButtonValues = new List<char>(this.RandomLetters.Count);
                this.currentWordsIndex = 0;
                this.nbLettersFound = 0;
                this.currentAnswer = null;
                this.lastCheckWordResult = QuizzAppCheckWordResult.QuizzAppCheckWordNone;

                this.PrepareTitleLetters();
            }
            this.isOnInitialization = false;
        }


        private List<string> SplitIntoWords(string normalizedMediaTitle)
        {
            // We split on all characters that are not 'abcdedfghi'
            // Split on all non-word characters.
            return Regex.Split(normalizedMediaTitle, @"\W+").Where(m => string.IsNullOrWhiteSpace(m) == false).ToList();
        }



        private void PrepareTitleLetters()
        {
            var list = new List<TitleLetterModel>();
            int? startNormalLetterIndex = null;


            int wordIndex = 0;

            if (!string.IsNullOrEmpty(this.normalizedAnswer))
            {
                var charArray = this.normalizedAnswer.ToCharArray();
                for (int i = 0; i < charArray.Length; i++)
                {
                    char item = charArray[i];
                    TitleLetterModel titleLetter = new TitleLetterModel();
                    this.SetTitleLetterState(titleLetter, TitleLetterState.LetterNotSetted);

                    if (!StringTools.IsOnQuizzAppAlphabet(item))
                    {
                        titleLetter.Letter = item;
                        titleLetter.wordIndex = -1;
                        this.SetTitleLetterState(titleLetter, TitleLetterState.StaticLetter);

                        if (i > 0 && StringTools.IsOnQuizzAppAlphabet(charArray[i - 1]))
                            wordIndex++;
                    }
                    else
                    {
                        titleLetter.wordIndex = wordIndex;
                        if (startNormalLetterIndex == null)
                            startNormalLetterIndex = i;

                        if (this.IsTitleFound)
                        {
                            titleLetter.Letter = item;
                            this.SetTitleLetterState(titleLetter, TitleLetterState.LetterSettedAndValid);
                        }
                    }

                    list.Add(titleLetter);
                }
            }

            this.TitleLetters = list;
            this.CurrentTextInputIndex = startNormalLetterIndex.HasValue ? startNormalLetterIndex.Value : 0;
        }



        private void SetTitleLetterState(TitleLetterModel letter, TitleLetterState titleLetterState)
        {
            switch (titleLetterState)
            {
                case TitleLetterState.StaticLetter:
                    letter.BackgroundBrush = this.DefaultBackgroundLetterBrush;
                    letter.TitleLetterState = titleLetterState;
                    letter.BackgroundOpacity = 0.4;
                    break;
                case TitleLetterState.LetterNotSetted:
                    letter.BackgroundBrush = this.DefaultBackgroundLetterBrush;
                    letter.TitleLetterState = titleLetterState;
                    letter.BackgroundOpacity = 1;
                    break;
                case TitleLetterState.LetterSetted:
                    letter.BackgroundBrush = this.DefaultBackgroundLetterBrush;
                    letter.TitleLetterState = titleLetterState;
                    letter.BackgroundOpacity = 1;
                    break;
                case TitleLetterState.LetterSettedAndValid:
                    letter.BackgroundBrush = this.GoodLetterBackgroundColorBrush;
                    letter.TitleLetterState = titleLetterState;
                    letter.BackgroundOpacity = 1;
                    break;
                case TitleLetterState.LetterSettedAndInvalid:
                    letter.BackgroundBrush = this.BadLetterBackgroundColorBrush;
                    letter.TitleLetterState = titleLetterState;
                    letter.BackgroundOpacity = 1;
                    break;
                default:
                    break;
            }
        }






        private int currentTextInputIndex;
        public int CurrentTextInputIndex
        {
            get { return currentTextInputIndex; }
            set
            {
                currentTextInputIndex = value;
                NotifyPropertyChanged(m => m.CurrentTextInputIndex);
            }
        }

        private void keyLetterClicked(object sender, NotificationEventArgs<char> e)
        {
            KeyboardLetterControlViewModel letterViewModel = sender as KeyboardLetterControlViewModel;
            if (letterViewModel == null)
                return;

            //Anti exception (stats result)
            if (this.TitleLetters == null || this.RandomLetters == null)
                return;

            if (!CanTypeAnswer)
                return;

            //Retrieve button
            char letter = letterViewModel.Letter;
            this.chosenLetterButtonKeys.Add(letterViewModel);
            this.chosenLetterButtonValues.Add(letter);

            if (this.currentAnswer == null)
                this.currentAnswer = letter.ToString();
            else
                this.currentAnswer += letter;

            if (this.CurrentTextInputIndex > (this.TitleLetters.Count - 1))
                return;

            // Update title view
            this.TitleLetters[CurrentTextInputIndex].Letter = letter;
            this.TitleLetters[CurrentTextInputIndex].TitleLetterState = TitleLetterState.LetterSetted;
            this.CurrentTextInputIndex++;

            // Check word
            this.lastCheckWordResult = this.CheckWord();

            // Going to next character index on title
            while (this.CurrentTextInputIndex < this.TitleLetters.Count - 1 && this.TitleLetters[CurrentTextInputIndex].TitleLetterState == TitleLetterState.StaticLetter)
                this.CurrentTextInputIndex++;

            // Going to next character on random letters
            char newKeyChar = ' ';
            this.currentRandomLettersIndex++;
            if (this.currentRandomLettersIndex < this.RandomLetters.Count)
                newKeyChar = this.RandomLetters[this.currentRandomLettersIndex];
            if (newKeyChar == ' ')
                letterViewModel.IsVisible = false;
            letterViewModel.ReplaceLetter(newKeyChar, true);
        }


        //public bool CanTypeAnswer()
        //{
        //    return this.currentAnswer == null || this.currentAnswer.Length < this.words[this.currentWordsIndex].Length;
        //}


        public bool CanTypeAnswer
        {
            get
            {
                return this.lastCheckWordResult != QuizzAppCheckWordResult.QuizzAppCheckWordWrong;
            }
        }


        public bool CanDelete
        {
            get
            {
                return this.currentAnswer != null && this.currentAnswer.Length > 0;
            }
        }


        public void DeletePreviousLetter()
        {
            if (this.CanDelete == false)
                return;

            // Update current answer
            this.currentAnswer = this.currentAnswer.Substring(0, this.currentAnswer.Length - 1);

                        
            // We go bak on index until find a character not 'special' on title 
            this.CurrentTextInputIndex--;
            while (this.CurrentTextInputIndex > 0 && this.TitleLetters[CurrentTextInputIndex].TitleLetterState == TitleLetterState.StaticLetter)
                this.CurrentTextInputIndex--;

            this.TitleLetters[CurrentTextInputIndex].Letter = ' ';
            int wordIndex = this.TitleLetters[CurrentTextInputIndex].wordIndex;
            // reset to Letter Not setted all letters of corrected word
            foreach (var item in this.TitleLetters)
            {
                if (item.wordIndex == wordIndex)
                    this.SetTitleLetterState(item, TitleLetterState.LetterNotSetted);
            }
            

            // Going to previous character index on title
            while (this.CurrentTextInputIndex > 0 && this.TitleLetters[CurrentTextInputIndex].TitleLetterState == TitleLetterState.StaticLetter)
                this.CurrentTextInputIndex--;

            // Going to previous character on random letters
            this.currentRandomLettersIndex--;
            var lastLetterButton = this.chosenLetterButtonKeys.Last();
            char replacementLetter = this.chosenLetterButtonValues.Last();
            if (lastLetterButton != null)
            {
                lastLetterButton.ReplaceLetter(replacementLetter, true);
                lastLetterButton.IsVisible = true;

                this.chosenLetterButtonKeys.RemoveAt(this.chosenLetterButtonKeys.Count - 1);
                this.chosenLetterButtonValues.RemoveAt(this.chosenLetterButtonValues.Count - 1);
            }

            this.lastCheckWordResult = this.CheckWord();
        }

        public void MakeBlinkHelpLetters()
        {
            if (this.words == null || this.KeysViewModels == null)
                return;

            if (this.currentWordsIndex >= this.words.Count)
                return;

            // highlight good letters on keyboard keys
            string word = this.words[this.currentWordsIndex];

            foreach (var item in this.KeysViewModels)
            {
                if (word.Contains(item.Letter))
                    item.MakeBlink();
            }

            // highlight letter on answer
            foreach (var item in this.TitleLetters)
            {
                if (item.wordIndex == this.currentWordsIndex)
                    item.LaunchBlinkAnimation();
            }

            this.NextHelpDateAvailability = DateTime.Now.AddSeconds(this.nbSecondsForHelp);
        }


        private QuizzAppCheckWordResult CheckWord()
        {
            QuizzAppCheckWordResult returnValue = QuizzAppCheckWordResult.QuizzAppCheckWordNone;

            // check Lenght
            if (this.currentAnswer.Length == this.words[this.currentWordsIndex].Length)
            {
                returnValue = QuizzAppCheckWordResult.QuizzAppCheckWordWrong; // word wrong

                if (this.currentAnswer.Equals(this.words[this.currentWordsIndex]))
                {
                    returnValue = QuizzAppCheckWordResult.QuizzAppCheckWordFound; // Word found
                    this.nbLettersFound += this.words[this.currentWordsIndex].Length;

                    // Update title view
                    int from = this.CurrentTextInputIndex - this.words[this.currentWordsIndex].Length;
                    for (int i = from; i < from + this.words[this.currentWordsIndex].Length; i++)
                    {
                        this.SetTitleLetterState(this.TitleLetters[i], TitleLetterState.LetterSettedAndValid);
                    }

                    // reset current answer
                    currentAnswer = null;
                    currentWordsIndex++;

                    // Go to next word
                    if (this.currentWordsIndex < this.words.Count)
                    {
                        // Notify good word
                    }
                    else
                    {
                        // Found media response
                        this.IsTitleFound = true;
                    }
                }
                else
                {
                    // Update title view
                    int from = this.CurrentTextInputIndex - this.words[this.currentWordsIndex].Length;
                    for (int i = from; i < from + this.words[this.currentWordsIndex].Length; i++)
                        this.SetTitleLetterState(this.TitleLetters[i], TitleLetterState.LetterSettedAndInvalid);
                }
            }

            if (returnValue == QuizzAppCheckWordResult.QuizzAppCheckWordWrong)
            {
                // Notify bad word
                this.NotifyBadWord();
            }

            return returnValue;
        }

        public event EventHandler<NotificationEventArgs> GoodAnswer;
        private void NotifyGoodAnswer()
        {
            if (GoodAnswer != null)
                Notify(GoodAnswer, new NotificationEventArgs());
        }


        public event EventHandler<NotificationEventArgs> BadWord;
        private void NotifyBadWord()
        {
            if (BadWord != null)
                Notify(BadWord, new NotificationEventArgs());
        }


        private DateTime nextHelpDateAvailability = DateTime.Now;
        public DateTime NextHelpDateAvailability
        {
            get { return nextHelpDateAvailability; }
            set
            {
                nextHelpDateAvailability = value;
                NotifyPropertyChanged(m => m.NextHelpDateAvailability);
            }
        }

        public bool IsHelpAvailable()
        {
            if (NextHelpDateAvailability < DateTime.Now)
                return true;
            else
                return false;
        }

    }

    public enum QuizzAppCheckWordResult
    {
        QuizzAppCheckWordNone = 0,
        QuizzAppCheckWordFound = 1,
        QuizzAppCheckWordWrong = -1,
    }



    public enum TitleLetterState
    {
        StaticLetter,
        LetterNotSetted,
        LetterSetted,
        LetterSettedAndValid,
        LetterSettedAndInvalid,
    }

    public class TitleLetterModel : ModelBase<TitleLetterModel>
    {

        private char letter;
        public char Letter
        {
            get { return letter; }
            set
            {
                letter = value;
                NotifyPropertyChanged(m => m.Letter);
            }
        }

        public int wordIndex { get; set; }

        private TitleLetterState titleLetterState;
        public TitleLetterState TitleLetterState
        {
            get { return titleLetterState; }
            set
            {
                titleLetterState = value;
                NotifyPropertyChanged(m => m.TitleLetterState);
            }
        }


        private Brush backgroundBrush;
        public Brush BackgroundBrush
        {
            get { return backgroundBrush; }
            set
            {
                backgroundBrush = value;
                NotifyPropertyChanged(m => m.BackgroundBrush);
            }
        }

        private double backgroundOpacity;
        public double BackgroundOpacity
        {
            get { return backgroundOpacity; }
            set
            {
                backgroundOpacity = value;
                NotifyPropertyChanged(m => m.BackgroundOpacity);
            }
        }




        #region animation
        public delegate void LaunchBlinkEventHandler(object sender, EventArgs e);
        public event LaunchBlinkEventHandler LaunchBlink;

        public void LaunchBlinkAnimation()
        {
            if (LaunchBlink != null)
                LaunchBlink(this, new EventArgs());
        }
        #endregion


    }

}
