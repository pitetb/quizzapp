using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Entities
{
    public class Level : BaseLevel
    {        
        private List<Pack> packs;
        public List<Pack> Packs
        {
            get { return packs; }
            set
            {
                packs = value;
                NotifyPropertyChanged(m => this.Packs);
                NotifyPropertyChanged(m => this.Progression);
            }
        }

        
        public float Progression
        {
            get
            {
                if (this.Packs == null || this.Packs.Count == 0)
                    return 0;                    
                return (float) ( ((float)NbPacksTerminated) / this.Packs.Count);
            }            
        }

        private int nbPacksTerminated;
        public int NbPacksTerminated
        {
            get { return nbPacksTerminated; }
            set
            {
                nbPacksTerminated = value;
                NotifyPropertyChanged(m => this.NbPacksTerminated);
                NotifyPropertyChanged(m => this.Progression);
            }
        }

        public Level()
        {
            this.Packs = new List<Pack>();
        }


    }
}
