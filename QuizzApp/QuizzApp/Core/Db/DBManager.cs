using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Db
{

    //public class GameDatabaseManager 
    //{      
    //    static GameDatabaseManager() { }
    //    private static GameDatabaseManager _instance = new GameDatabaseManager();
    //    public static GameDatabaseManager Instance { get { return _instance; } }



    //    //private Dictionary<string, T> databaseHelpers = new Dictionary<string, T>();
    //    private GameDBHelper gameDBHelper;
    //    public GameDBHelper GameDBHelper
    //    {
    //        get { return gameDBHelper; }
    //        set { gameDBHelper = value; }
    //    }
        

    //    private GameDatabaseManager()
    //    {
    //        this.GameDBHelper = new GameDBHelper();
    //    }
    //}


    //public class DatabaseManager<T> where T : DBHelper
    //{

    //    private Dictionary<string, T> databaseHelpers = new Dictionary<string, T>();
    //    //private volatile int mOpenCounter = 0;

    //    private static DatabaseManager<T> instance;
    //    //private static T mDatabaseHelper;

    //    public static void InitializeInstance(T db)
    //    {
    //        if (instance == null)
    //        {
    //            instance = new DatabaseManager<T>();
                
    //            mDatabaseHelper = db;
    //        }
    //    }

    //    public static DatabaseManager<T> GetInstance()
    //    {
    //        if (instance == null)
    //        {
    //            throw new Exception((typeof(DatabaseManager<T>)).Name +
    //                    " is not initialized, call InitializeInstance method first.");
    //        }
    //        return instance;
    //    }

    //    public T OpenDatabase()
    //    {
    //        if (mOpenCounter == 0)
    //        {
    //            // Opening new database
    //            if (mDatabaseHelper.OpenDatabase())
    //                mOpenCounter++;
    //        }
    //        return mDatabaseHelper;
    //    }

    //    public void CloseDatabase()
    //    {

    //        if (mOpenCounter == 1)
    //        {
    //            // Closing database
    //            mDatabaseHelper.CloseDatabase();

    //        }
    //        mOpenCounter--;
    //    }
    //}
}
