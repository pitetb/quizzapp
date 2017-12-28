using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuizzApp.Core.Helpers
{
    public class TaskHelpers
    {

        public static void RunDelayed(int millisecondsDelay, Action func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }

            TaskEx.Delay(millisecondsDelay).ContinueWith(_ => func.Invoke());
        }

        public static T RunDelayed<T>(int millisecondsDelay, Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }



            Task<T> task = TaskEx.Delay(millisecondsDelay, new CancellationToken()).ContinueWith(_ => func.Invoke());
            task.Wait();
            return task.Result;            
        }


        public static async Task RunDelayedAsync(int millisecondsDelay, Action func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }

            
            await TaskEx.Delay(millisecondsDelay).ContinueWith(_ => func.Invoke());            
        }

        public static async Task<T> RunDelayedAsync<T>(int millisecondsDelay, Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }



            Task<T> task = TaskEx.Delay(millisecondsDelay, new CancellationToken()).ContinueWith(_ => func.Invoke());
            await task;
            return task.Result;
            
            //var taskCompletionSource = new TaskCompletionSource<T>();
            //taskCompletionSource.SetResult(t);


            //var timer = new Timer(self =>
            //{
            //    ((Timer)self).Dispose();
            //    try
            //    {
            //        var result = func();
            //        taskCompletionSource.SetResult(result);
            //    }
            //    catch (Exception exception)
            //    {
            //        taskCompletionSource.SetException(exception);
            //    }
            //});
            //timer.Change(millisecondsDelay, millisecondsDelay);

            //return taskCompletionSource.Task;
        }
    }

         
}
