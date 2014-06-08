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
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://nswc2014.azurewebsites.net/Home/UpdateOdds");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
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
