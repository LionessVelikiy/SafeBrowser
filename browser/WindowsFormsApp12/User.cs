using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp12
{
    class User
    {
        public static string SERVER = "http://test.tasty-catalog.ru";
        public static string ID = "57619";

        public static bool IS_WHITE = false;
        public static bool STATUS = false;
        public static string getHistoryFilePath()
        {
            return String.Format("history/{0}.history", User.ID);
        }


        public static void SendURL(string URL,string query_id = "-")
        {
            
            Console.WriteLine(URL);
            using (WebClient wc = new WebClient())
            {
                string type = "SITE";

                if( URL.IndexOf("?q=") != -1)
                {
                    type = "Google";
                }
                if (URL.IndexOf("text=") != -1)
                {
                    type = "Yandex";
                }
                User.SaveLocalUrl(URL);
                URL = URL.Replace("&", "$");
                //Console.WriteLine(String.Format("{0}/history/add/?url={1}&typeQuery={2}&user={3}&filter={4}", SERVER, URL, type, ID, query_id));
              string responce = wc.DownloadString(String.Format("{0}/history/add/?url={1}&typeQuery={2}&user={3}&filter={4}", SERVER, URL, type, ID, query_id) );
                //Console.WriteLine(responce);
            }


        }


        public static void SaveLocalUrl(string URL)
        {
            File.AppendAllText( User.getHistoryFilePath(),String.Format("{0}~{1}~{2}\n",URL,"2021-12-01","Описание"));
        }



        public static string[] ReadHistoryUrl()
        {
            return File.ReadAllLines(User.getHistoryFilePath());
     
        }


    }
}
