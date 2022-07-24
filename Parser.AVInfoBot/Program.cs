using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using DAL.DatabaseAccess;
using DAL.Models;
using Newtonsoft.Json;

namespace Parser.AVInfoBot
{
    internal class Program
    {
        private static int counterPages = 1; 
        
        // if doesn't work - paste absolute path to file "query.json"
        private static readonly string pathToQuery = @"..\..\query.json";
        
        public static void Main(string[] args)
        {
            int num = 0;
            var timerCallback = new TimerCallback(Parse);
            // таймер на 10 минут
            var timer = new Timer(timerCallback, num, 0, 600000);

            Console.ReadLine();
        }

        public static void Parse(object obj)
        {
            while (true)
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.lesegais.ru/open-area/graphql");
                request.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36";
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/json";

                var requestStream = request.GetRequestStream();
                
                var querySample = File.ReadAllText(pathToQuery);

                requestStream.Write(Encoding.UTF8.GetBytes(querySample), 0, querySample.Length);
                requestStream.Close();

                var webResponse = request.GetResponse();
                byte[] bytes;
                using (var stream = webResponse.GetResponseStream())
                using (var ms = new MemoryStream())
                {
                    int count;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = stream!.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    } while (stream.CanRead && count > 0);

                    bytes = ms.ToArray();
                }

                var result = JsonConvert.DeserializeObject<Root>(Encoding.UTF8.GetString(bytes));

                var deals = Deal.ContentListToDealList(result?.Data.SearchReportWoodDeal.Content);

                if (deals.Count == 0)
                {
                    SetZeroPage();   
                    break;
                }
                
                DealRepository.SaveDeals(deals);
                NextPage();

                Console.WriteLine($"Проверено {(counterPages - 1) * 1000} записей");
                Thread.Sleep(100);
            }

        }
        
        private static void NextPage()
        {
            var querySample = File.ReadAllText(pathToQuery);
        
            dynamic jsonQuery = JsonConvert.DeserializeObject(querySample);
            jsonQuery!["variables"]["number"] = counterPages;
            string query = JsonConvert.SerializeObject(jsonQuery, Formatting.Indented);
            File.WriteAllText(pathToQuery,query);
        
            counterPages++;
        }

        private static void SetZeroPage()
        {
            var querySample = File.ReadAllText(pathToQuery);
        
            dynamic jsonQuery = JsonConvert.DeserializeObject(querySample);
            jsonQuery!["variables"]["number"] = 0;
            string query = JsonConvert.SerializeObject(jsonQuery, Formatting.Indented);
            File.WriteAllText(pathToQuery,query);
        
            counterPages = 1;
        }
    }
}