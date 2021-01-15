using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.OLE.Interop;
/// <summary>
/// CMoney爬蟲練習 NBA Player Career Data
/// 1. Get First Name Char List
/// 2. Get Player Name And Connection Url
/// 3. Get Player Career Data
/// 4. Output Data To CSV.
/// 2021-01-13
/// ALN
/// 直接硬幹
/// </summary>
namespace CMoneyCraw
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. A~Z 爬不出來偷懶
            string[] arrays = new string[26];
            for (int i = 0; i < 26; i++)
            {
                arrays[i] = Convert.ToChar(97 + i).ToString();
            }
            try
            {
                //WebClient webClient = new WebClient();
                //MemoryStream ms = new MemoryStream(webClient.DownloadData("https://www.basketball-reference.com/players/"));
                //HtmlDocument doc = new HtmlDocument();
                // 2. 爬出姓名和連結頁
                foreach (string ch in arrays)
                {
                    List<Player> players = new List<Player>();
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument document = web.Load("https://www.basketball-reference.com/players/" + ch + "/");
                    HtmlNodeCollection name = document.DocumentNode.SelectNodes($"//*[@id='players']/tbody/tr");
                    if (name != null)
                    {
                        int row = name.Count();
                        for (int i = 1; i <= row; i++)
                        {
                            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes($"//*[@id='players']/tbody/tr[{i}]/th/a");
                            if (nodes != null)
                            {
                                Player player = new Player();
                                player.Name = nodes[0].InnerText;
                                string url = nodes[0].Attributes["href"].Value;
                                // 3. Get Player Career Data
                                Console.WriteLine(player.Name + "    " + url);
                                HtmlWeb web_detail = new HtmlWeb();
                                HtmlDocument doc_detail = web_detail.Load("https://www.basketball-reference.com/" + url);
                                //紀錄div欄位
                                HtmlNodeCollection data_detail = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div");
                                //紀錄div欄位個數
                                int count = data_detail.Count();
                                //[1]標題忽略
                                //HtmlNodeCollection data_1 = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[1]");
                                HtmlNodeCollection data1 = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[2]");
                                HtmlNodeCollection data2 = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[3]");
                                HtmlNodeCollection data3 = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[4]");
                                var json_data = JsonConvert.SerializeObject(data_detail[0].InnerHtml);
                            }
                        }
                    }
                    // 4. Output Char CSV.
                    var ll = players;
                    FileStream fileStream = new FileStream("./" + ch, System.IO.FileMode.Open, System.IO.FileAccess.Write);
                }

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
    //Model-球員資料
    public class Player
    {
        public string Name { get; set; }
        public double G { get; set; }
        public double PTS { get; set; }
        public double TRB { get; set; }
        public double AST { get; set; }
        public double FG { get; set; }
        public double FG3 { get; set; }
        public double FT { get; set; }
        public double eFG { get; set; }
        public double PER { get; set; }
        public double WS { get; set; }
    }
}
