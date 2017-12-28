using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace QuizzApp.Core.Helpers
{

    public class MQHttpWebClient
    {
        internal class DownloadInfos
        {
            public BinaryReader BinaryReader { get; set; }
            public string FileName { get; set; }
            public long ContentLenght { get; set; }
        }

        private async Task<DownloadInfos> DoRequestAsync2(WebRequest req)
        {
            string serverFileName = string.Empty;
            var result = await Task.Factory.FromAsync<WebResponse>(req.BeginGetResponse, req.EndGetResponse, null);
            Debug.WriteLine("End Get Resp :" + result.ContentLength);

            // RegEx to extract file name from headers
            var reFile = new Regex("filename=\"(.*?)\"");

            // The header that contains the filename. Example:
            // Content-Disposition: attachment; filename="pack_4.zip"
            var contentDisposition = result.Headers["Content-Disposition"];

            if (reFile.Match(contentDisposition).Success && reFile.Match(contentDisposition).Groups.Count >= 2)
            {
                serverFileName = reFile.Match(contentDisposition).Groups[1].Value;
            }
            else
            {
                throw new Exception("Can't get file name from server response in http headers, req : " + req.RequestUri);
            }

            WebResponse resp = result;
            var stream = resp.GetResponseStream();
            var sr = new System.IO.BinaryReader(stream);

            return new DownloadInfos() { BinaryReader = sr, FileName = serverFileName, ContentLenght = resp.ContentLength };
        }




        private async Task<DownloadInfos> DoRequestAsync(string url)
        {            
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.AllowReadStreamBuffering = true;

            var tr = await DoRequestAsync2(req);
            return tr;
        }



        public async Task<string> DoDownloadRequestAsync(string uri, string fileFolderPath, Action<int> downloadProgressCallback, CancellationToken cancellationToken)
        {
            Debug.WriteLine("Doing request " + uri);
            var ret = await DoRequestAsync(uri);

            cancellationToken.ThrowIfCancellationRequested();

            await Task.Factory.StartNew(() =>
            {
                byte[] filesBytes = ReadFully(ret.BinaryReader.BaseStream, ret.ContentLenght, downloadProgressCallback);
                IsolatedStorageHelpers.SaveFileAndCreateParentFolders(Path.Combine(fileFolderPath, ret.FileName), filesBytes);
            });

            cancellationToken.ThrowIfCancellationRequested();

            return ret.FileName;
        }



        private static byte[] ReadFully(Stream input, long contentLenght, Action<int> downloadProgressCallback)
        {
            byte[] buffer = new byte[8 * 1024];

            long totalValue = contentLenght;
            long sum = 0;
            int progress = 0;

            if (downloadProgressCallback != null)
                downloadProgressCallback(progress);

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    sum += read;
                    progress = (int)((sum * 100) / totalValue);

                    ms.Write(buffer, 0, read);

                    Debug.WriteLine("Progress : " + progress);
                    if (downloadProgressCallback != null)
                        downloadProgressCallback(progress);
                }
                return ms.ToArray();
            }
        }

    }


    public class JsonWebClient
    {

        public int TimeOutInSecs { get; set; }

        public JsonWebClient()
        {
            this.TimeOutInSecs = 10;
        }


        public JsonWebClient(int timeOutNbSeconds)
        {
            this.TimeOutInSecs = timeOutNbSeconds;
        }

        public async Task<string> DoRequestAsync(WebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
               return sr.ReadToEnd();
            }            
        }
        

        private async Task<string> DoRequestAsync(string url)
        {
            HttpWebRequest req = HttpWebRequest.CreateHttp(url);
            req.AllowReadStreamBuffering = true;
            Task<string> taskReq = DoRequestAsync(req);
            var completeTask = await TaskEx.WhenAny(taskReq, TaskEx.Delay(10000));
            if (completeTask == taskReq)
                return await taskReq;
            else
                return null; // timeout
        }

        private async Task<T> DoRequestJsonAsync<T>(WebRequest req)
        {
            Task<string> taskReq = DoRequestAsync(req);
            var completeTask = await TaskEx.WhenAny(taskReq, TaskEx.Delay(this.TimeOutInSecs * 1000));
            if (completeTask != taskReq)
                return default(T);

            var response = taskReq.Result;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response);
        }

        public async Task<T> DoRequestJsonAsync<T>(string uri)
        {
            Debug.WriteLine("DoRequestJsonAsync to : " + uri);
            var ret = await DoRequestAsync(uri);
            if (ret == null)
                return default(T);
            var response = ret;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response);            
        }












        //private async Task<byte[]> GetURLContentsAsync(string url)
        //{
        //    // The downloaded resource ends up in the variable named content.
        //    var content = new MemoryStream();

        //    // Initialize an HttpWebRequest for the current URL.
        //    var webReq = (HttpWebRequest)WebRequest.Create(url);

        //    // Send the request to the Internet resource and wait for
        //    // the response.                
        //    using (WebResponse response = await webReq.GetResponseAsync())

        //    // The previous statement abbreviates the following two statements.

        //    //Task<WebResponse> responseTask = webReq.GetResponseAsync();
        //    //using (WebResponse response = await responseTask)
        //    {
        //        // Get the data stream that is associated with the specified url.
        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            // Read the bytes in responseStream and copy them to content. 
        //            await responseStream.CopyToAsync(content);

        //            // The previous statement abbreviates the following two statements.

        //            // CopyToAsync returns a Task, not a Task<T>.
        //            //Task copyTask = responseStream.CopyToAsync(content);

        //            // When copyTask is completed, content contains a copy of
        //            // responseStream.
        //            //await copyTask;
        //        }
        //    }
        //    // Return the result as a byte array.
        //    return content.ToArray();
        //}

        //public static class HttpMethod
        //{
        //    public static string Head { get { return "HEAD"; } }
        //    public static string Post { get { return "POST"; } }
        //    public static string Put { get { return "PUT"; } }
        //    public static string Get { get { return "GET"; } }
        //    public static string Delete { get { return "DELETE"; } }
        //    public static string Trace { get { return "TRACE"; } }
        //    public static string Options { get { return "OPTIONS"; } }
        //    public static string Connect { get { return "CONNECT"; } }
        //    public static string Patch { get { return "PATCH"; } }
        //}

        //public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
        //{
        //    var taskComplete = new TaskCompletionSource<Stream>();
        //    request.BeginGetRequestStream(ar =>
        //    {
        //        Stream requestStream = request.EndGetRequestStream(ar);
        //        taskComplete.TrySetResult(requestStream);
        //    }, request);
        //    return taskComplete.Task;
        //}

        //public Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        //{
        //    var taskComplete = new TaskCompletionSource<HttpWebResponse>();
        //    request.BeginGetResponse(asyncResponse =>
        //    {
        //        try
        //        {
        //            HttpWebRequest responseRequest = (HttpWebRequest)asyncResponse.AsyncState;
        //            HttpWebResponse someResponse = (HttpWebResponse)responseRequest.EndGetResponse(asyncResponse);
        //            taskComplete.TrySetResult(someResponse);
        //        }
        //        catch (WebException webExc)
        //        {
        //            HttpWebResponse failedResponse = (HttpWebResponse)webExc.Response;
        //            taskComplete.TrySetResult(failedResponse);
        //        }
        //    }, request);
        //    return taskComplete.Task;
        //}

    }
}
