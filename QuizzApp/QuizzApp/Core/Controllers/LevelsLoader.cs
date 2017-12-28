using QuizzApp.Core.Db;
using QuizzApp.Core.Entities;
using QuizzApp.Core.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace QuizzApp.Core.Controllers
{

    public static class LevelsLoader
    {
        private const string PACKAGES_FOLDER_PATH = "packages_quizzapp";
        private const string TEMP_FOLDER_PATH = "temp";
        private const string PACKAGES_TEMP_FOLDER_PATH = "temp\\packages";
        private const string DOWNLOAD_TEMP_FOLDER_PATH = "temp\\dl";

        private const string LEVEL_PREFIX = "level_";
        public const string LEVEL_COMPRESS_EXTENSION = ".zip";

        public static string GetPackagesTempFolderPath()
        {
            return PACKAGES_TEMP_FOLDER_PATH + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;
        }

        public static List<string> LevelsNewlyInstalledDbPath;

        public static void DeployLevelsInApp(ResourceManager appRessourceManager)
        {
            List<string> levelsToDeployList = new List<string>();
            List<string> levelsInstalledDbPath = new List<string>();

            // Retrieve package list of current Language
            //ResourceManager rm = new ResourceManager("MovieQuizz2.Resources.AppResources", Assembly.GetExecutingAssembly());
            ResourceManager rm = appRessourceManager;
            ResourceSet resourceSet = rm.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key as string;
                Debug.WriteLine(key);
                if (key != null && key.StartsWith(LEVEL_PREFIX) && entry.Value.GetType() == typeof(byte[]))
                {
                    Debug.WriteLine(key + " " + entry.Value);
                    levelsToDeployList.Add(key);
                }
            }

            foreach (string levelName in levelsToDeployList)
            {
                string levelNameZip = levelName + LEVEL_COMPRESS_EXTENSION;

                // test if package is already installed
                if (!IsLevelAlreadyInstalled(levelName))
                {
                    //var sr = Application.GetResourceStream(new Uri(f, UriKind.Relative));
                    byte[] sr = rm.GetObject(levelName, CultureInfo.CurrentUICulture) as byte[];


                    if (sr != null)
                    {
                        IsolatedStorageHelpers.SaveFileAndCreateParentFolders(PACKAGES_TEMP_FOLDER_PATH + Path.DirectorySeparatorChar + levelNameZip, sr);
                        string dbPath = InstallLevelFiles(levelNameZip, levelName);
                        levelsInstalledDbPath.Add(dbPath);
                    }
                }
            }

            LevelsNewlyInstalledDbPath = levelsInstalledDbPath;
        }


        public static bool IsLevelAlreadyInstalled(string levelName)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                return isoStore.DirectoryExists(PACKAGES_FOLDER_PATH + Path.DirectorySeparatorChar + levelName);
            }
        }


        public static string InstallLevelFiles(string zipNameOnTempFolder, string levelNameOnLevelsDir)
        {
            string zipPath = PACKAGES_TEMP_FOLDER_PATH + Path.DirectorySeparatorChar + zipNameOnTempFolder;
            string outputPath = PACKAGES_FOLDER_PATH + Path.DirectorySeparatorChar + levelNameOnLevelsDir;

            ExtractZipFile(zipPath, string.Empty, outputPath);

            // We return the database path
            string ret = outputPath + Path.DirectorySeparatorChar + levelNameOnLevelsDir + GameDBHelper.DB_FILE_NAME_EXTENSION;

            // delete zip
            try
            {
                using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                    if (isoStore.FileExists(zipPath))
                        isoStore.DeleteFile(zipPath);
            }
            catch (Exception) // We leave the file
            {
            }

            return ret;
        }

        //public static Level GetDownloadedLevel(string levelDirectoryName)
        //{
        //    string rootPckPath = PACKAGES_FOLDER_PATH + Path.DirectorySeparatorChar + CultureInfo.CurrentUICulture.TwoLetterISOLanguageName + Path.DirectorySeparatorChar;

        //    using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
        //    {
        //        if (levelDirectoryName.StartsWith(LEVEL_PREFIX) && isoStore.FileExists(rootPckPath + levelDirectoryName + Path.DirectorySeparatorChar + levelDirectoryName + GameDBHelper.DB_FILE_NAME_EXTENSION))
        //        {
        //            string dbPath = rootPckPath + levelDirectoryName + Path.DirectorySeparatorChar + levelDirectoryName + GameDBHelper.DB_FILE_NAME_EXTENSION;
        //            return GetDownloadedLevelFromDbPath(dbPath);


        //            //using (PackageDal dal = new PackageDal(rootPckPath + packDirectoryName + Path.DirectorySeparatorChar + packDirectoryName + PackageDal.DB_FILE_NAME_EXTENSION))
        //            //{
        //            //    if (dal.OpenDatabase())
        //            //    {
        //            //        return dal.GetPackage();
        //            //    }
        //            //}
        //        }
        //    }
        //    return null;
        //}


        //public static Level GetDownloadedLevelFromDbPath(string fullDataBasePath)
        //{
        //    using (LevelDBHelper dal = new LevelDBHelper(fullDataBasePath))
        //    {
        //        if (dal.OpenDatabase())
        //        {
        //            var levels = dal.GetLevels();
        //            if (levels == null || levels.Count == 0)
        //                return null;

        //            return levels.First().Value;
        //        }
        //    }
        //    return null;
        //}


        private static void ExtractZipFile(string archiveFilenameIn, string password, string outFolderPath)
        {
            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                Stream stream = null;
                try
                {
                    using (UnZipper unzip = new UnZipper(isoStore.OpenFile(archiveFilenameIn, FileMode.Open)))
                    {
                        foreach (string filename in unzip.FileNamesInZip)
                        {
                            stream = unzip.GetFileStream(filename);
                            string correctedFilename = filename.Replace("/", "" + Path.DirectorySeparatorChar);
                            IsolatedStorageHelpers.SaveFileAndCreateParentFolders(outFolderPath + Path.DirectorySeparatorChar + correctedFilename, stream);
                        }
                    }
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close(); // Ensure we release resources
                    }
                }                
            }
        }




    }
}
