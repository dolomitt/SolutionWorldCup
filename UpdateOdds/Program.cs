using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UpdateOdds
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = 0;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nswc2014.azurewebsites.net/Home/UpdateOdds?password=orange05!");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = 1;
                    }
                }
            }
            catch
            {
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nswc2014.azurewebsites.net/Home/UpdateResults?password=orange05!");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return result;
                    }
                    else
                    {
                        return result^1;
                    }
                }
            }
            catch
            {
                return 2;
            }
        }
    }
}
