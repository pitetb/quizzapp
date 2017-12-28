using Community.CsharpSqlite.SQLiteClient;
using QuizzApp.Core.Entities;
using QuizzApp.Core.Entities.Json;
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
    public class GameDBHelper : LevelDBHelper
    {
        private const string LEVEL_FOLDER_PREFIX = "level_";

        protected string mediaFolderPath;

        public GameDBHelper(string dbPath, string mediaFolderPath)
            : base(dbPath)
        {
            this.mediaFolderPath = mediaFolderPath;
        }

        protected override string GetFullMediaPath(int levelId, int mediaId)
        {
            return mediaFolderPath + Path.DirectorySeparatorChar + LEVEL_FOLDER_PREFIX + levelId + Path.DirectorySeparatorChar + IMAGE_FILE_PATH.Replace(MEDIA_ID_REGEX, "" + mediaId);
        }

        public void AddDifficulty(InitDifficultyByLang difficulty)
        {
            Dictionary<string, object> valuesToInsert = new Dictionary<string, object>();
            valuesToInsert.Add("id", difficulty.Difficulty.Id);
            valuesToInsert.Add("name", difficulty.Difficulty.Name);
            valuesToInsert.Add("enum_value", difficulty.Difficulty.EnumValue);
            valuesToInsert.Add("language", difficulty.Language.Value);
            this.Insert("Difficulties", valuesToInsert);
        }

        public Dictionary<int, Difficulty> GetDifficulties()
        {
            Dictionary<int, Difficulty> difficulties = new Dictionary<int, Difficulty>();
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, name, enum_value, language FROM Difficulties;";
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Difficulty dif = new Difficulty();
                        dif.Id = reader.GetInt32(0);
                        dif.Name = reader.GetString(1);
                        dif.EnumVal = reader.GetInt32(2);
                        dif.Language = reader.GetString(3);
                        difficulties.Add(dif.Id, dif);
                    }
                }
            }
            return difficulties;
        }

        public void AddLevel(BaseLevel level)
        {
            Dictionary<string, object> valuesToInsert = new Dictionary<string, object>();
            valuesToInsert.Add("id", level.Id);
            valuesToInsert.Add("value", level.Val);
            valuesToInsert.Add("difficulty_id", level.DifficultyId);
            valuesToInsert.Add("release_date", DateTimeHelpers.ToUnixTime(level.ReleaseDate));
            valuesToInsert.Add("language", level.Language);
            this.Insert("Levels", valuesToInsert);
        }


        public void AddPack(Pack aPack)
        {
             //'id' int(11) ,
             //'level_id' int(11) ,
             //'title' text ,
             //'author' text ,
             //'language' text ,
            Dictionary<string, object> valuesToInsert = new Dictionary<string, object>();
            valuesToInsert.Add("id", aPack.Id);
            valuesToInsert.Add("level_id", aPack.LevelId);
            valuesToInsert.Add("title", aPack.Title);
            valuesToInsert.Add("author", aPack.Author);
            valuesToInsert.Add("language", aPack.Language);
            this.Insert("packs", valuesToInsert);
        }

        public void AddMedia(Media aMedia)
        {
             //'id' int(11),
             // 'title' text ,
             // 'rects' text ,
             // 'difficulty' int(11) ,
             // 'language' text ,
             // 'time' real,
             // 'completed' int(11),
             // 'variants' text ,
             // 'extra1' text ,
             // 'extra2' text ,
             // 'extra3' text ,
             // 'fextra1' text ,
             // 'fextra2' text ,
             // 'fextra3' text ,
            Dictionary<string, object> valuesToInsert = new Dictionary<string, object>();
            valuesToInsert.Add("id", aMedia.Id);
            valuesToInsert.Add("title", aMedia.Title);
            valuesToInsert.Add("rects", aMedia.Poster.FloutedAreasString);
            valuesToInsert.Add("difficulty", aMedia.Difficulty);
            valuesToInsert.Add("language", aMedia.Language);
            valuesToInsert.Add("time", aMedia.Time.TotalMilliseconds);
            valuesToInsert.Add("variants", aMedia.VariantsString);
            valuesToInsert.Add("extra1", aMedia.Extra1);
            valuesToInsert.Add("extra2", aMedia.Extra2);
            valuesToInsert.Add("extra3", aMedia.Extra3);
            valuesToInsert.Add("fextra1", aMedia.FExtra1);
            valuesToInsert.Add("fextra2", aMedia.FExtra2);
            valuesToInsert.Add("fextra3", aMedia.FExtra3);
            this.Insert("medias", valuesToInsert);
        }

        public void AddPackMediaAssoc(int packId, int mediaId, int position, bool isCompleted)
        {
             //'media_id' int(11) ,
             //'pack_id' int(11) ,
             //'position' int(11) , completed INTEGER DEFAULT 0,
            Dictionary<string, object> valuesToInsert = new Dictionary<string, object>();
            valuesToInsert.Add("media_id", mediaId);
            valuesToInsert.Add("pack_id", packId);
            valuesToInsert.Add("position", position);
            valuesToInsert.Add("completed", isCompleted ? 1 : 0);           
            this.Insert("packs_medias", valuesToInsert);
        }


        /// <summary>
        /// We override GetMediasOnPack to retrieve "isCompleted" property on association
        /// </summary>
        /// <param name="packId"></param>
        /// <returns></returns>
        public override List<Media> GetMediasOnPack(int packId)
        {
            List<Media> movies = new List<Media>();
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT " +
                    "m.id, m.title, m.rects, m.difficulty, m.language, m.time, m.variants, m.extra1, m.extra2, m.extra3, m.fextra1, m.fextra2, m.fextra3, " +
                    "pm.position, pm.completed, " +
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
                        bool completed = reader.GetBoolean(14);
                        int levelId = reader.GetInt32(15);

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
                        media.Variants = variants.Split(';').ToList();
                        if (!media.Variants.Contains(media.Title))
                            media.Variants.Add(media.Title);

                        media.Position = position;
                        media.Extra1 = extra1;
                        media.Extra2 = extra1;
                        media.Extra3 = extra1;
                        media.FExtra1 = fextra1;
                        media.FExtra2 = fextra2;
                        media.FExtra3 = fextra3;


                        List<double> points = rects.Split(';').Where(s => !string.IsNullOrEmpty(s)).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToList();
                        List<Rect> floutedAreas = new List<Rect>();
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

                        //System.Windows.Rect rectangle = new System.Windows.Rect();
                        media.Poster = new MediaImage(GetFullMediaPath(levelId, media.Id), floutedAreas, (!completed), position <= 3);

                        movies.Add(media);
                    }
                }
            }
            return movies;
        }

        public Pack GetPackById(int packId)
        {
            Pack pack = null;
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                //'id' int(11) ,
                //'level_id' int(11),
                //'title' text ,
                //'author' text ,
                //'language'  text,
                cmd.CommandText = "SELECT id, level_id, title, author, language FROM packs Where id = " + packId;
                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
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
                        pack = new Pack();
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
                        pack.Difficulty = sumDifficulty / nbMedias;                       
                    }
                }
            }
            return pack;
        }


        public void SetIsCompleted(Media movie, Pack pack, bool isCompleted)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                string completed = isCompleted ? "1" : "0";                
                cmd.CommandText = "UPDATE packs_medias SET completed = " + completed  + " WHERE media_id = " + movie.Id + " AND pack_id = " + pack.Id + ";";
                cmd.ExecuteNonQuery();
            }
        }


        public void SetFullPackCompleted(Pack pack, bool isCompleted)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                string medias_list = string.Join(",", pack.Medias.Select(s => s.Id));
                string completed = isCompleted ? "1" : "0";
                cmd.CommandText = "UPDATE packs_medias SET completed = " + completed + " WHERE media_id in (" + medias_list + " ) AND pack_id = " + pack.Id + ";";
                cmd.ExecuteNonQuery();
            }
        }

        public void SetTimePassed(Media movie)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE Movie set time = " + (int)Math.Round(movie.Time.TotalMilliseconds, 0) + " WHERE id = " + movie.Id + ";";
                cmd.ExecuteNonQuery();
            }
        }

        public void SetLocked(int packId, bool isLocked)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE Pack set locked = " + (isLocked ? "" + 1 : "" + 0) + " WHERE id = " + packId + ";";
                cmd.ExecuteNonQuery();
            }
        }

        public void ResetPack(int packId)
        {
            using (SqliteCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE Movie SET Completed = 0 , Time = 0 ;";
                cmd.ExecuteNonQuery();
            }
        }

    }

}
