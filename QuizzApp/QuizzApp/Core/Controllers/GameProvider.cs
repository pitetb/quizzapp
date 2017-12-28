using QuizzApp.Core.Db;
using QuizzApp.Core.Entities;
using QuizzApp.Core.Entities.Json;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace QuizzApp.Core.Controllers
{
    public abstract class GameProvider
    {
        protected const string QUIZZ_APP_INIT_BASE_URL = @"http://www.quizz-app.com/services";
        protected const string QUIZZ_APP_SERVICE_INFO = @"info";
        protected const string QUIZZ_APP_SERVICE_LEVELS_LIST = @"levels_list";
        protected const string QUIZZ_APP_SERVICE_DOWNLOAD_LEVEL = @"download_level";
              
        

        //private const int QUIZZ_APP_REQUEST_TIMEOUT = 10; // Not used for now

        protected const string MAIN_DATABASE_FILE_NAME = "game.sqlite";
        protected const string MAIN_DATABASE_FOLDER_NAME = "Db";
        protected const string MAIN_DATABASE_PATH = MAIN_DATABASE_FOLDER_NAME + @"\" + MAIN_DATABASE_FILE_NAME;

        protected const string MAIN_PACKS_FOLDER_PATH = "packages_quizzapp" + @"\";        
        
       
        public DateTime? LastDlLevelsListTime { get; protected set; }
        private List<LevelDl> toDlLevels;
        
        public string QUIZZ_APP_MEDIA_TYPE_IDENTIFIER { get; protected set; }

        public DateTime? LastInitInfosRepTime { get; protected set; }
        public InitInfosRep LastInitInfosRep { get; protected set; }

        public event EventHandler<ControllerNotificationEventArgs<Pack>> PackDownloaded;

        public GameProvider(string mediaTypeIdentifier)
        {
            this.QUIZZ_APP_MEDIA_TYPE_IDENTIFIER = mediaTypeIdentifier;
        }


        public void InstallMainDb(ResourceManager appRessourceManager)
        {
            // We install main DB
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            //string[] resources = currentAssembly.GetManifestResourceNames();

            //// Build the string of resources.
            //foreach (string resource in resources)
            //    Debug.WriteLine(resource);

            // Create a stream for the file in the installation folder.
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();

            byte[] main_Db_Ressource = QuizzApp.Ressources.QuizzAppResource.game;

            //Uri uri = new Uri("QuizzApp;component/Ressources/Database/" + MAIN_DATABASE_FILE_NAME, UriKind.Relative);            
            //var resStream = Application.GetResourceStream(uri);

            using (Stream input = new MemoryStream(main_Db_Ressource))
            {
                // Create a stream for the new file in the local folder.
                IsolatedStorageHelpers.CreateParentFolders(MAIN_DATABASE_PATH);
                using (IsolatedStorageFileStream output = iso.CreateFile(MAIN_DATABASE_PATH))
                {
                    // Initialize the buffer.
                    byte[] readBuffer = new byte[4096];
                    int bytesRead = -1;

                    // Copy the file from the installation folder to the local folder. 
                    while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    {
                        output.Write(readBuffer, 0, bytesRead);
                    }
                }
            }


            // Create Default Difficulties

        }

        public void DeployPackagesInApp(ResourceManager appRessourceManager)
        {            
            // We deploy files of levels "on app"
            LevelsLoader.DeployLevelsInApp(appRessourceManager);

            List<string> levelsNewlyInstalled = LevelsLoader.LevelsNewlyInstalledDbPath;

            // we generate thumbnail for new installed packages and we insert data on main DB
            if (levelsNewlyInstalled != null)
            {
                foreach (var item in levelsNewlyInstalled)
                {
                    // Install Level on MainDb
                    InstallLevelOnMainDb(item, null, new CancellationToken());                                      
                }
            }

            //var installedLevels = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH).GetLevels();
            LevelsLoader.LevelsNewlyInstalledDbPath.Clear();
        }


        private void InstallLevelOnMainDb(string levelDbPath, Action<int> installProgress, CancellationToken cancellationToken)
        {
            using (GameDBHelper gameDbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            using (LevelDBHelper levelDb = new LevelDBHelper(levelDbPath))
            {
                int percent = 0;
                
                Dictionary<int, BaseLevel> levels = gameDbHelper.GetLevels();

                foreach (var aLevel in levelDb.GetLevels())
                {
                    // Retrieve packs
                    cancellationToken.ThrowIfCancellationRequested();
                    var newPacks = levelDb.GetPacks(aLevel.Value.Id);

                    var existingPacks = gameDbHelper.GetPacks(aLevel.Value.Id);
                    var existingMedias = gameDbHelper.GetAllMediasIds();

                    
                    // Copy level on main DB
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!levels.ContainsKey(aLevel.Key))
                        gameDbHelper.AddLevel(aLevel.Value);



                    // for each newPacks, generate thumbnails...
                    double i = 0;                    
                    foreach (var pack in newPacks)
                    {
                        i = i + 1;

                        // Generate thumbnails
                        cancellationToken.ThrowIfCancellationRequested();
                        GenerateThumbnails(pack.Value);
                        cancellationToken.ThrowIfCancellationRequested();
                    
                        percent = (int)(( i / (newPacks.Count *2)) * 100); // multiply by 2 because the newpack list will be iterated twice
                       
                        Debug.WriteLine("Generate thum percent : " + percent);
                        if (installProgress != null)
                            installProgress.Invoke( percent);

                        /* ORIGINAL CODE COMMENTED FOR CANCELLATION OPTIM
                        // Copy values on Main Db
                        if (!existingPacks.ContainsKey(pack.Key))
                            gameDbHelper.AddPack(pack.Value);

                        
                        // Add media off pack
                        foreach (var aMedia in pack.Value.Medias)
                        {
                            if (!existingMedias.Contains(aMedia.Id))
                            {
                                gameDbHelper.AddMedia(aMedia);
                                gameDbHelper.AddPackMediaAssoc(pack.Value.Id, aMedia.Id, aMedia.Position, aMedia.IsCompleted);
                            }
                        }
                        */
                    }

                    // .. then add them to main DB if nor cancelled
                    cancellationToken.ThrowIfCancellationRequested();
                    Debug.WriteLine("Registering packs on main DB");
                    foreach (var pack in newPacks)
                    {
                        // Copy values on Main Db
                        if (!existingPacks.ContainsKey(pack.Key))
                            gameDbHelper.AddPack(pack.Value);


                        // Add media off pack
                        foreach (var aMedia in pack.Value.Medias)
                        {
                            if (!existingMedias.Contains(aMedia.Id))
                            {
                                gameDbHelper.AddMedia(aMedia);
                                gameDbHelper.AddPackMediaAssoc(pack.Value.Id, aMedia.Id, aMedia.Position, aMedia.IsCompleted);
                            }
                        }

                        i = i + 1;
                        percent = (int)((i / (newPacks.Count * 2)) * 100);
                        Debug.WriteLine("Packs registered on DB, percent : " + percent);
                        if (installProgress != null)
                            installProgress.Invoke(percent);
                    }                    
                }
            }
        }

        private void GenerateThumbnails(Pack package)
        {
            Debug.WriteLine(DateTime.Now.ToString(new CultureInfo("fr-FR")) + " : Generating thumb for package : " + package.Title);

            List<Media> orderedMovie = package.Medias.OrderBy(m => m.Position).ToList();
            if (orderedMovie.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    orderedMovie[i].Poster.GenerateThumbnail();
                }
            }

            Debug.WriteLine(DateTime.Now.ToString(new CultureInfo("fr-FR")) + " : End generating thumb for package : " + package.Title);
        }


        public List<Difficulty> GetDifficulties()
        {
            //List<Difficulty> difficulties = new List<Difficulty>();
            //if (this.LastInitInfosRep != null)
            //    difficulties = this.LastInitInfosRep.Difficulties.Where(m => m.Language.Value.Equals(Utils.GetCurrentLanguageOnQuizzAppFormat(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            
            using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            {
                var installedDifficulties = dbHelper.GetDifficulties();
                return installedDifficulties.Values.Where(m => m.Language.Equals(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
        }

        public List<BaseLevel> GetBaseLevels()
        {
            List<BaseLevel> levels = new List<BaseLevel>();            

            using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            {
                var installedLevels = dbHelper.GetLevels();
                levels.AddRange(installedLevels.Values.Where(m => m.Language.Equals(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat(), StringComparison.InvariantCultureIgnoreCase)));
            }

            //// Debug purpose
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);
            //levels.Add(levels[0]);

            return levels;
        }

        public Level GetLevel(int levelId)
        {
            Level level = new Level();
            using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            {
                BaseLevel baseLevel = dbHelper.GetLevelById(levelId);
                if (baseLevel == null)
                    return null;

                // Copying values from base level type
                level.Id = baseLevel.Id;
                level.DifficultyId = baseLevel.DifficultyId;
                level.Language = baseLevel.Language;
                level.Md5 = baseLevel.Md5;
                level.PackIds = baseLevel.PackIds;
                level.ReleaseDate = baseLevel.ReleaseDate;
                level.Val = baseLevel.Val;
                level.ZipSize = baseLevel.ZipSize;

                var packs = dbHelper.GetPacks(levelId);
                int nbPacksTerminated = 0;
                foreach (var item in packs.Values)
                {
                    if (item.IsTerminated)
                        nbPacksTerminated++;

                    level.Packs.Add(item);
                }

                //if (level.Packs.Count == 0)
                //    level.Progression = 0;
                //else
                //    level.Progression = nbPacksTerminated / level.Packs.Count;

                level.NbPacksTerminated = nbPacksTerminated;
            }
            return level;
        }


        public Pack GetPackById(int id)
        {
            using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            {
                return dbHelper.GetPackById(id);
            }
        }

        

        public void PackTerminated(Pack packTerminated)
        {
            // We start by unlocking packs
            
        }

        public void SetIsMediaCompleted(Media media, Pack pack, bool isCompleted)
        {
            using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            {
                dbHelper.SetIsCompleted(media, pack, isCompleted);
            }
        }


        public void SetFullPackCompleted(Pack pack, bool isCompleted)
        {
            using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
            {
                dbHelper.SetFullPackCompleted(pack, isCompleted);
            }
        }

        





        #region packs download

        public async Task<List<LevelDl>> GetDlLevelListAsync(bool forceRefreshFromServer)
        {
            if (this.LastDlLevelsListTime != null && this.toDlLevels != null && forceRefreshFromServer == false)
                return this.toDlLevels;
                        
            if (await RefreshDlLevelListAsync())
            {
                var filteredList = this.toDlLevels.Where(m => m.Language.Equals(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                return filteredList;
            }
            else
                return new List<LevelDl>();
        }

        private async Task<bool> RefreshDlLevelListAsync()
        {
            try
            {
                JsonWebClient client = new JsonWebClient();
                List<LevelDl> resp = await client.DoRequestJsonAsync<List<LevelDl>>(QUIZZ_APP_INIT_BASE_URL + "/" + QUIZZ_APP_SERVICE_LEVELS_LIST + "/" + QUIZZ_APP_MEDIA_TYPE_IDENTIFIER);

                this.toDlLevels = resp;
                this.LastDlLevelsListTime = DateTime.Now;
                return this.toDlLevels != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error retrieving packs to dl list !!", ex);
                return false;
            }
        }

        #endregion

        #region initRequest

        // Called a the startup
        public async Task<bool> GetInitInfosAsync(bool updateIndicatorOfAllPacks)
        {
            if (this.LastInitInfosRepTime != null)
                return this.LastInitInfosRep != null;

            return await RefreshInitInfosAsync(updateIndicatorOfAllPacks);            
        }

        private async Task<bool> RefreshInitInfosAsync(bool updateIndicatorOfAllPacks)
        {
            try
            {
                JsonWebClient client = new JsonWebClient();
                InitInfosRep resp = await client.DoRequestJsonAsync<InitInfosRep>(QUIZZ_APP_INIT_BASE_URL + "/" + QUIZZ_APP_SERVICE_INFO + "/" + QUIZZ_APP_MEDIA_TYPE_IDENTIFIER);

                this.LastInitInfosRep = resp;
                this.LastInitInfosRepTime = DateTime.Now;

                // Inject dificulties
                if (this.LastInitInfosRep == null)
                    return false;

                var difficulties = this.LastInitInfosRep.Difficulties.Where(m => m.Language.Value.Equals(LanguagesUtils.GetCurrentLanguageOnQuizzAppFormat(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                using (var dbHelper = new GameDBHelper(MAIN_DATABASE_PATH, MAIN_PACKS_FOLDER_PATH))
                {
                    var installedDifficulties = dbHelper.GetDifficulties();
                    foreach (var item in difficulties)
                    {
                        if (!installedDifficulties.ContainsKey(item.Difficulty.Id))
                            dbHelper.AddDifficulty(item);
                    }

                    var toto = dbHelper.GetDifficulties();
                }
                
                

                //if (updateIndicatorOfAllPacks)
                //    this.UpdateIndicatorNewOfPacks();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error doing 'init' on server!!", ex);
                return false;
            }
        }

        #endregion

        #region downloadLevel

        


        public async Task<bool> DownloadAndInstallLevel(int idLevel, Action<int> downloadProgress, Action<int> installProgress, CancellationToken cancellationToken)
        {
            try
            {
               
                string url = QUIZZ_APP_INIT_BASE_URL + "/" + QUIZZ_APP_SERVICE_DOWNLOAD_LEVEL + "/" + idLevel;
                MQHttpWebClient client = new MQHttpWebClient();
                string fileDownloadedName = await client.DoDownloadRequestAsync(url, LevelsLoader.GetPackagesTempFolderPath(), downloadProgress, cancellationToken);
                string extractToFolderName = fileDownloadedName.Replace(LevelsLoader.LEVEL_COMPRESS_EXTENSION, string.Empty);

                cancellationToken.ThrowIfCancellationRequested();

                await Task.Factory.StartNew(() =>
                {
                    // We install the pack and ad it to list
                    string dbPath = LevelsLoader.InstallLevelFiles(fileDownloadedName, fileDownloadedName.Replace(LevelsLoader.LEVEL_COMPRESS_EXTENSION, string.Empty));

                    cancellationToken.ThrowIfCancellationRequested();

                    this.InstallLevelOnMainDb(dbPath, installProgress, cancellationToken);
                }, cancellationToken);

               
                return true;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Downloading pack cancelled !!");                
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error downloading pack !!", ex);                
                return false;
            }
        }


        //private void NotifyPackDownloaded(Package pack)
        //{
        //    // Notify view of an error
        //    Notify(PackDownloaded, new ControllerNotificationEventArgs<Package>(pack.PackType.ToString(), pack));
        //}

        //protected void Notify
        //    (EventHandler<ControllerNotificationEventArgs> handler,
        //    ControllerNotificationEventArgs e)
        //{
        //    if (handler != null)
        //    {
        //        InternalNotify(() => handler(this, e));
        //    }
        //}

       
        //protected void Notify<TOutgoing>
        //    (EventHandler<ControllerNotificationEventArgs<TOutgoing>> handler,
        //    ControllerNotificationEventArgs<TOutgoing> e)
        //{
        //    if (handler != null)
        //    {
        //        InternalNotify(() => handler(this, e));
        //    }
        //}

        //private void InternalNotify(Action method)
        //{
        //    // Fire the event
        //    method.Invoke();
        //}

        #endregion

    }



}
