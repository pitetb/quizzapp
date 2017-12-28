using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuizzApp.Core.Helpers
{
    
    public class LevelDownloader
    {

        public LevelDownloader(Uri url)
        {
            URL = url;
            TotalLength = -1;
            Debug.WriteLine("New LevelDownloader with URL : " + url);
        }

        
        public Uri URL { get; private set; }
        public string Name { get; set; }

        private int _progress;
        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                }
            }
        }

        private long TotalLength { get; set; }

        public MemoryStream memoryStream;
        HttpWebRequest request;

        public async Task<WebResponse> StartDownloadAsync(WebRequest req)
        {
            memoryStream = new MemoryStream();
            request = (HttpWebRequest)req;
            //request.AllowReadStreamBuffering = false;
            var asyncCallback = new AsyncCallback(GetData);


            //return await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);


            return await Task.Factory.FromAsync<WebResponse>(
                request.BeginGetResponse(asyncCallback, request),
                request.EndGetResponse);
        }

        
        void GetData(IAsyncResult result)
        {
            HttpWebRequest request = (HttpWebRequest)result.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);
            Stream rStream = response.GetResponseStream();

            
            byte[] data = new byte[16 * 1024];
            int read;

            long totalValue = response.ContentLength;
            long sum = 0;

            Debug.WriteLine("Progress : " + this.Progress);

            while ((read = rStream.Read(data, 0, data.Length)) > 0)
            {
                sum += read;
                this.Progress = Progress = (int)((sum * 100) / totalValue);
                //App.DownloadDispatcher.BeginInvoke(new Action(() => Progress = (int)((sum * 100) / totalValue)));
                memoryStream.Write(data, 0, read);
                Debug.WriteLine("Progress : " + this.Progress);
            }

            Debug.WriteLine("Progress : " + this.Progress);
            memoryStream.Position = 0;
        }

    }
}
