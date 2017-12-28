using SimpleMvvmToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Entities
{
    public class Difficulty : ModelBase<Difficulty>
    {
       
        private int id;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged(m => m.Id);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(m => m.Name);
            }
        }

        private int enumVal;
        public int EnumVal
        {
            get { return enumVal; }
            set
            {
                enumVal = value;
                NotifyPropertyChanged(m => m.EnumVal);
            }
        }


        private string language;
        public string Language
        {
            get { return language; }
            set
            {
                language = value;
                NotifyPropertyChanged(m => m.Language);
            }
        }


    }   
}
