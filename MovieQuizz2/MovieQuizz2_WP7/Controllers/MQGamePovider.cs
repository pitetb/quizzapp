using QuizzApp.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieQuizz2.Controllers
{
    public class MQGameProvider : GameProvider
    {
        // Movie quizz game provider
        public MQGameProvider()
            : base("movie")
        {

        }

        //#region Singleton pattern

        //private static MQGameProvider instance;
        //public static MQGameProvider Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            instance = new MQGameProvider();
        //        }
        //        return instance;
        //    }
        //}
        //#endregion       
    }
}
