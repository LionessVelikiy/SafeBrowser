using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp12
{
    class CheckerData
    {

        public static List<Query> querys = new List<Query>();

        //загрузка фильтров
        public static void UploadFilters()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                string filters = wc.DownloadString(String.Format("http://test.tasty-catalog.ru/history/getFilter/?type=watch&number={0}", User.ID));
              

                string[] querysStr = filters.Split('|');
                foreach (string q in querysStr)
                {
                    Console.WriteLine(q);
                    querys.Add(new Query(q));
                }
            }


            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                string status = wc.DownloadString(String.Format("http://test.tasty-catalog.ru/users/getStatusChildren/?type=watch&number={0}", User.ID));


                User.STATUS = status == "True";
            }


            using (WebClient wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                string filters = wc.DownloadString(String.Format("http://test.tasty-catalog.ru/history/getFilter/?type=white&number={0}", User.ID));


                string[] querysStr = filters.Split('|');
                if(querysStr.Length > 1)
                {
                    querys = new List<Query>();
                    User.IS_WHITE = true;
                }
                foreach (string q in querysStr) { 

                    querys.Add(new Query(q));
                }
            }
        }

        //проверка по фильтрам
        public static string Parsing(string url, string content)
        {
            if (!User.STATUS)
            {
                return "--";
            }
            //В НИЖНИЙ РЕГИСТР
            content = content.ToLower();
          
            //ПЕРЕБИРАЕМ ПРИЗЩЗНАКИ
            foreach(Query q in querys)
            {
                if (q.value == "") continue;
                Console.WriteLine(User.IS_WHITE);
                if (User.IS_WHITE)
                {
                    if (url.IndexOf(q.value) == -1 )
                    {


                        string id = q.id;
                  
                        //В ПРОТИВНОМ СЛУЧАЕ ВЗВРАЩАЕМ ID ПРИЗНАКА, ЧТОБЫ ОТМЕТИТЬ ЕГО В ИСТОРИИ 
                        return id;
                    }
                }

              else
                {
                    Console.WriteLine(q.value);
                    //ИЩЕМ ПРИЗНАК В URL ИЛИ КОНТЕНТЕ
                    if(url.IndexOf(q.value) != -1 || content.IndexOf(q.value) != -1)
                    {
                       
                       
                        string id = q.id;
                        //ЕСЛИ ЭТО НАБЛЮДЕНИЕ, ТО ТОЛЬКО ПОМЕЧАЕМ
                        if (!q.isBlock)
                        {
                            id += "_watch";
                        }
                        //В ПРОТИВНОМ СЛУЧАЕ ВЗВРАЩАЕМ ID ПРИЗНАКА, ЧТОБЫ ОТМЕТИТЬ ЕГО В ИСТОРИИ 
                        return id;
                    }
                }
            }

            return "-";
        }



    }
}
