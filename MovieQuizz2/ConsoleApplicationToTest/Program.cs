using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationToTest
{
    class Program
    {
        static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        static void Main(string[] args)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("toto", "titi");
            string value = JsonConvert.SerializeObject(values, Formatting.None, settings);

            object o = JsonConvert.DeserializeObject(value, values.GetType(), settings);

            values = (Dictionary<string, object>) o;
            Console.ReadLine();
        }

       
    }
}
