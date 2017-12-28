using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace QuizzApp.Core.Helpers
{


    public static class IsolatedStorageHelpers
    {
        public static void SaveFileAndCreateParentFolders(string fileName, Stream stream, bool eraseExistingFile = false)
        {
            byte[] data = ReadFully(stream);
            SaveFileAndCreateParentFolders(fileName, data, eraseExistingFile);
        }

        public static void SaveFileAndCreateParentFolders(string fileName, byte[] data, bool eraseExistingFile = false)
        {
            string strBaseDir = string.Empty;
            string[] dirsPath = fileName.Split(new[] { Path.DirectorySeparatorChar });

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Recreate the directory structure
                for (int i = 0; i < dirsPath.Length - 1; i++)
                {
                    strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                    isoStore.CreateDirectory(strBaseDir);
                }

                if (eraseExistingFile && isoStore.FileExists(fileName))
                    isoStore.DeleteFile(fileName);

                if (!isoStore.FileExists(fileName))
                {
                    using (var bw = new BinaryWriter(isoStore.CreateFile(fileName)))
                    {
                        bw.Write(data);
                    }
                }
            }
        }

        public static void CreateParentFolders(string fileName)
        {
            string strBaseDir = string.Empty;
            string[] dirsPath = fileName.Split(new[] { Path.DirectorySeparatorChar });

            using (var isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // Recreate the directory structure
                for (int i = 0; i < dirsPath.Length - 1; i++)
                {
                    strBaseDir = System.IO.Path.Combine(strBaseDir, dirsPath[i]);
                    isoStore.CreateDirectory(strBaseDir);
                }
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public static void CopyBinaryFile(IsolatedStorageFile isf, string filename, bool replace = false)
        {
            if (!isf.FileExists(filename) || replace == true)
            {
                BinaryReader fileReader = new BinaryReader(TitleContainer.OpenStream(filename));
                IsolatedStorageFileStream outFile = isf.CreateFile(filename);

                bool eof = false;
                long fileLength = fileReader.BaseStream.Length;
                int writeLength = 512;
                while (!eof)
                {
                    if (fileLength < 512)
                    {
                        writeLength = Convert.ToInt32(fileLength);
                        outFile.Write(fileReader.ReadBytes(writeLength), 0, writeLength);
                    }
                    else
                    {
                        outFile.Write(fileReader.ReadBytes(writeLength), 0, writeLength);
                    }

                    fileLength = fileLength - 512;

                    if (fileLength <= 0) eof = true;
                }
                fileReader.Close();
                outFile.Close();
            }

        }



        public static void CleanAndDeleteDirectoryRecursive(string directory)
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            if (iso.DirectoryExists(directory))
            {
                string[] files = iso.GetFileNames(directory + @"/*");
                foreach (string file in files)
                {
                    iso.DeleteFile(directory + @"/" + file);
                    Debug.WriteLine("Deleted file: " + directory + @"/" + file);
                }

                string[] subDirectories = iso.GetDirectoryNames(directory + @"/*");
                foreach (string subDirectory in subDirectories)
                {
                    CleanAndDeleteDirectoryRecursive(directory + @"/" + subDirectory);
                }

                iso.DeleteDirectory(directory);
                Debug.WriteLine("Deleted directory: " + directory);
            }
        }
    }    
}
