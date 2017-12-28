using Community.CsharpSqlite.SQLiteClient;
using QuizzApp.Core.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizzApp.WP.ViewModels.Helpers.MigrationManager
{
    
    public class MigrationDbHelper : DBHelper
    {

        private const int MOVIE_QUIZZ_1_MIGRATION_NB_POSTER_IN_PACK = 10;

        public MigrationDbHelper(string dbPath)
            : base(dbPath)
        {
            
        }

        public bool IsPackFinished(int packId)
        {
            int sumCompleted = 0;
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT SUM(m.completed) as sumCompleted FROM Movie m, Compose c WHERE c.id_pack = " + packId + " AND c.id_movie = m.id;";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sumCompleted = reader.GetInt32(0);                       
                        break;
                    }
                }
            }
            return (sumCompleted == MOVIE_QUIZZ_1_MIGRATION_NB_POSTER_IN_PACK); ;
        }

    }
}
