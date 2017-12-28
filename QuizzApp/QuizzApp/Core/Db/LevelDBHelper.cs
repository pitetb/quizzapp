using Community.CsharpSqlite.SQLiteClient;
using QuizzApp.Core.Entities;
using QuizzApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuizzApp.Core.Db
{
    public class LevelDBHelper : DBHelper
    {

        protected static string IMAGE_FILE_PATH = "img" + Path.DirectorySeparatorChar + "media_{mediaId}.jpg";
        protected static string MEDIA_ID_REGEX = "{mediaId}";
                      
        public LevelDBHelper(string dbPath)
            : base(dbPath)
        {
           
        }


        protected virtual string GetFullMediaPath(int levelId, int mediaId)
        {
            return dbFolderPath + IMAGE_FILE_PATH.Replace(MEDIA_ID_REGEX, "" + mediaId);
        }

        
        public Dictionary<int, BaseLevel> GetLevels()
        {
            Dictionary<int, BaseLevel> levels = new Dictionary<int, BaseLevel>();
            using (SqliteCommand cmd = connection.CreateCommand())
            { 
                cmd.CommandText = "SELECT id, value, difficulty_id, release_date, language FROM levels;";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        BaseLevel lev = this.CreateBaseLevelFromSqlReader(reader);                        
                        levels.Add(lev.Id, lev);
                    }
                }
            }
            return levels;
        }

        public BaseLevel GetLevelById(int id)
        {
            BaseLevel level = new BaseLevel();
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, value, difficulty_id, release_date, language FROM levels where id = " + id + ";";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        level = this.CreateBaseLevelFromSqlReader(reader);
                        break;
                    }
                }
            }
            return level;
        }

        private BaseLevel CreateBaseLevelFromSqlReader(SqliteDataReader reader)
        {
            //'id' int(11) ,
            //'value' int(11) ,
            //'difficulty_id' int(11) ,
            //'release_date' int(11) NOT NULL ,
            //'language' text ,

            BaseLevel lev = new BaseLevel();
            lev.Id = reader.GetInt32(0);
            lev.Val = reader.GetInt32(1);
            lev.DifficultyId = reader.GetInt32(2);

            // SOME PACKS HAVE DATETIME AS DATA TYPE OTHERS HAVE INT32, we have to try...
            DateTime releaseDate;
            try
            {
                int releaseDateLong = reader.GetInt32(3);
                releaseDate = DateTimeHelpers.FromUnixTime(releaseDateLong);
            }
            catch (Exception)
            {
                releaseDate = reader.GetDateTime(3);
            }
            lev.ReleaseDate = releaseDate;
            lev.Language = reader.GetString(4);
            return lev;
        }


        public Dictionary<int, Pack> GetPacks(int levelId)
        {
            Dictionary<int, Pack> packs = new Dictionary<int, Pack>();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                //'id' int(11) ,
                //'level_id' int(11),
                //'title' text ,
                //'author' text ,
                //'language'  text,

                

               

                cmd.CommandText = "SELECT id, level_id, title, author, language FROM packs Where level_id = " + levelId;
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int levId = reader.GetInt32(1);
                        string title = reader.GetString(2);
                        string author = reader.GetString(3);
                        string language = reader.GetString(4);
                        
                        string packageInfos = string.Format("{0},{1},{2},{3},{4}",
                            id, levId, title, title, author, language);

                        //Debug.WriteLine(packageInfos);

                        List<Media> medias = GetMediasOnPack(id);
                        Pack pack = new Pack();
                        pack.Id = id;
                        pack.LevelId = levId;
                        pack.Title = title;
                        pack.Author = author;
                        pack.Language = language;

                        float sumDifficulty = 0;
                        float nbMedias = 0;

                        foreach (var item in medias)
                        {          
                  
                            sumDifficulty += item.Difficulty;                            
                            nbMedias++;
                            pack.AddMedia(item);
                        }

                        pack.Difficulty = sumDifficulty/nbMedias;                        
                        packs.Add(pack.Id, pack);
                    }
                }
            }

            return packs;
        }


        public List<int> GetAllMediasIds()
        {
            List<int> medias = new List<int>();

            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT " +
                    "m.id, m.title " +
                    "FROM medias m ORDER BY id ASC;";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    // DB Column 
                    // Media
                    // 'id' int(11),
                    //'title' text ,
                    //'rects' text ,
                    //'difficulty' int(11) ,
                    //'language' text ,
                    //'time' real,
                    //'completed' int(11), // IGNORE IT
                    //'variants' text ,
                    //'extra1' text ,
                    //'extra2' text ,
                    //'extra3' text ,
                    //'fextra1' float ,
                    //'fextra2' float ,
                    //'fextra3' float ,

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        medias.Add(id);                        
                    }
                }
            }
            return medias;
        }

        

        public virtual List<Media> GetMediasOnPack(int packId)
        {
            List<Media> movies = new List<Media>();
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT " + 
                    "m.id, m.title, m.rects, m.difficulty, m.language, m.time, m.variants, m.extra1, m.extra2, m.extra3, m.fextra1, m.fextra2, m.fextra3, " +
                    "pm.position, " +
                    "p.level_id " + 
                    "FROM medias m, packs_medias pm, packs p WHERE pm.pack_id = " + packId + " AND m.id = pm.media_id AND p.id = pm.pack_id ORDER BY pm.position ASC;";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                      // DB Column 
                      //packs_media
                      //  'media_id' int(11) ,
                      //'pack_id' int(11) ,
                      //'position' int(11) , 
                      //completed INTEGER DEFAULT 0,

                      // Media
                      // 'id' int(11),
                      //'title' text ,
                      //'rects' text ,
                      //'difficulty' int(11) ,
                      //'language' text ,
                      //'time' real,
                      //'completed' int(11), // IGNORE IT
                      //'variants' text ,
                      //'extra1' text ,
                      //'extra2' text ,
                      //'extra3' text ,
                      //'fextra1' float ,
                    //'fextra2' float ,
                    //'fextra3' float ,

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string title = reader.GetString(1);
                        string rects = reader.GetString(2);
                        int difficulty = reader.GetInt32(3);
                        string lang = reader.GetString(4);
                        double time = reader.GetDouble(5);
                        string variants = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                        string extra1 = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                        string extra2 = reader.IsDBNull(8) ? string.Empty : reader.GetString(8);
                        string extra3 = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);
                        float? fextra1 = reader.IsDBNull(10) ? default(float?) : reader.GetFloat(10);
                        float? fextra2 = reader.IsDBNull(11) ? default(float?) : reader.GetFloat(11);
                        float? fextra3 = reader.IsDBNull(12) ? default(float?) : reader.GetFloat(12);

                        int position = reader.GetInt32(13);
                        //bool completed = reader.GetBoolean(14);
                        bool completed = false;
                        int levelId = reader.GetInt32(14);

                        string movieInfo = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                            id, title, rects, difficulty, lang, time, variants, extra1, extra2, extra3, fextra1, fextra2, fextra3, position, completed, levelId);

                        //Debug.WriteLine(movieInfo);

                        Media media = new Media();
                        media.Id = id;
                        media.Title = title;
                        media.Difficulty = difficulty;
                        media.Language = lang;
                        media.Time = TimeSpan.FromMilliseconds(time);
                        media.IsCompleted = completed;
                        media.VariantsString = variants;
                        media.Variants = variants.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToList();
                        if (!media.Variants.Contains(media.Title))
                            media.Variants.Add(media.Title);

                        media.Position = position;
                        media.Extra1 = extra1;
                        media.Extra2 = extra1;
                        media.Extra3 = extra1;
                        media.FExtra1 = fextra1;
                        media.FExtra2 = fextra2;
                        media.FExtra3 = fextra3;
                        List<Rect> floutedAreas = this.GetFloutedRectsFromString(rects);

                        media.Poster = new MediaImage(GetFullMediaPath(levelId, media.Id), floutedAreas, (!completed), position <= 3);
                        media.Poster.FloutedAreasString = rects;

                        movies.Add(media);
                    }
                }
            }
            return movies;
        }

        protected List<Rect> GetFloutedRectsFromString(string rects)
        {
            List<Rect> floutedAreas = new List<Rect>();
            List<double> points = rects.Split(';').Where(s => !string.IsNullOrEmpty(s)).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToList();
            Rect currentRect = new Rect(); ;
            for (int i = 0; i < points.Count; i++)
            {
                if ((i % 4) == 0)
                {
                    currentRect = new Rect();
                    currentRect.X = points[i];
                }
                else if ((i % 4) == 1)
                {
                    currentRect.Y = points[i];
                }
                else if ((i % 4) == 2)
                {
                    currentRect.Width = points[i] - currentRect.X;
                }
                else
                {
                    currentRect.Height = points[i] - currentRect.Y;
                    floutedAreas.Add(currentRect);
                }
            }
            return floutedAreas;
        }
    }
}
