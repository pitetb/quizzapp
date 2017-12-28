using QuizzApp.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace QuizzApp.WP.ViewModels.Helpers.MigrationManager
{
    internal class MigrationManager
    {

        // APPSETTINGS KEYS
        private const string ARE_MOVIE_QUIZZ_1_MIGRATION_KEYS_SAVED = "ARE_MOVIE_QUIZZ_1_MIGRATION_KEYS_SAVED";
        private const string ARE_MOVIE_QUIZZ_1_PACKS_DELETED = "ARE_MOVIE_QUIZZ_1_PACKS_DELETED";
        private const string MOVIE_QUIZZ_1_PACKS_TERMINATED_IDS = "MOVIE_QUIZZ_1_PACKS_TERMINATED_IDS";

        // MQ1 paths and files
        private const string MQ1_PACKAGES_FOLDER_PATH = "packages";
        private const string MQ1_PACKAGE_PREFIX = "pack_";

        internal enum MQ1PacksTypes
        {
            Originaux = 0,
            Extensions = 1,
            Mensuels = 2,
            Experts = 3
        }


        private bool AreMovieQuizz1MigrationKeysSaved()
        {
            if (!AppSettings.Instance.ContainsKey(ARE_MOVIE_QUIZZ_1_MIGRATION_KEYS_SAVED))
            {
                AppSettings.Instance.AddOrUpdateValue(ARE_MOVIE_QUIZZ_1_MIGRATION_KEYS_SAVED, false);
                AppSettings.Instance.Save();
            }

            return AppSettings.Instance.GetValueOrDefault(ARE_MOVIE_QUIZZ_1_MIGRATION_KEYS_SAVED, false);
        }


        private bool AreMovieQuizz1PacksDeletedDone()
        {
            if (!AppSettings.Instance.ContainsKey(ARE_MOVIE_QUIZZ_1_PACKS_DELETED))
            {
                AppSettings.Instance.AddOrUpdateValue(ARE_MOVIE_QUIZZ_1_PACKS_DELETED, false);
                AppSettings.Instance.Save();
            }

            return AppSettings.Instance.GetValueOrDefault(ARE_MOVIE_QUIZZ_1_PACKS_DELETED, false);
        }



        public void MigrateMovieQuizz1Packs()
        {
            List<int> mq1PacksFinishedIds = new List<int>(); 

            // Check status
            if (!AreMovieQuizz1MigrationKeysSaved())
            { 
                // Loop on all packsTypes
                var mq1Types = EnumHelpers.GetEnumValues<MQ1PacksTypes>();

                foreach (MQ1PacksTypes packType in mq1Types)
                {

                    // Loop on all MQ1 installed packs
                    // if pack is finished, add to list           
                    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        string rootPckPath = MQ1_PACKAGES_FOLDER_PATH + Path.DirectorySeparatorChar + AppSettings.Instance.LanguageOnAppLaunchBck + Path.DirectorySeparatorChar + (int)packType + Path.DirectorySeparatorChar;
                        if (!isoStore.DirectoryExists(rootPckPath))
                            continue;

                        // Retrieve directories.
                        List<String> directoryList = new List<String>(isoStore.GetDirectoryNames(rootPckPath + "*"));

                        // Add all directories at this directory
                        foreach (string directoryName in directoryList)
                        {
                            if (directoryName == "pack_30")
                            {
                                int aa = 0;
                                aa++;
                            }

                            if (directoryName.StartsWith(MQ1_PACKAGE_PREFIX) && isoStore.FileExists(rootPckPath + directoryName + Path.DirectorySeparatorChar + directoryName + MigrationDbHelper.DB_FILE_NAME_EXTENSION))
                            {
                                int packId;
                                if (int.TryParse(directoryName.Replace(MQ1_PACKAGE_PREFIX, string.Empty), out packId))
                                {
                                    using (MigrationDbHelper dal = new MigrationDbHelper(rootPckPath + directoryName + Path.DirectorySeparatorChar + directoryName + MigrationDbHelper.DB_FILE_NAME_EXTENSION))
                                    {
                                        if (dal.OpenDatabase() && dal.IsPackFinished(packId))
                                        {
                                            mq1PacksFinishedIds.Add(packId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Save List on AppSettings
                AppSettings.Instance.AddOrUpdateValue(MOVIE_QUIZZ_1_PACKS_TERMINATED_IDS, mq1PacksFinishedIds);

                // Flag import done
                AppSettings.Instance.AddOrUpdateValue(ARE_MOVIE_QUIZZ_1_MIGRATION_KEYS_SAVED, true);

                AppSettings.Instance.Save();
            }



            // DELETE ALL MOVIE QUIZZ 1 PACKS
            if (! AreMovieQuizz1PacksDeletedDone() && AreMovieQuizz1MigrationKeysSaved())
            {
                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string rootPckPath = MQ1_PACKAGES_FOLDER_PATH + Path.DirectorySeparatorChar;
                    if (isoStore.DirectoryExists(rootPckPath))
                        IsolatedStorageHelpers.CleanAndDeleteDirectoryRecursive(rootPckPath);
                }

                AppSettings.Instance.AddOrUpdateValue(ARE_MOVIE_QUIZZ_1_PACKS_DELETED, true);
                AppSettings.Instance.Save();
            }
        }


        public List<int> GetMQ1PacksCompleted()
        {            
            return AppSettings.Instance.GetValueOrDefault(MOVIE_QUIZZ_1_PACKS_TERMINATED_IDS, new List<int>());
        }
    }
}
