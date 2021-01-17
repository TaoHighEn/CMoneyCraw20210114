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
                            //部分有Strong,偷懶不判斷
                            HtmlNodeCollection node1 = document.DocumentNode.SelectNodes($"//*[@id='players']/tbody/tr[{i}]/th/a");
                            HtmlNodeCollection node2 = document.DocumentNode.SelectNodes($"//*[@id='players']/tbody/tr[{i}]/th/strong/a");
                            HtmlNodeCollection nodes = node1==null?node2:node1;
                            //*[@id="players"]/tbody/tr[10]/th/strong/a
                            //表結構為Master-Detail
                            if (nodes != null)
                            {
                                Player player = new Player();
                                //取得Master
                                player.Name = nodes[0].InnerText;
                                //取得Detail
                                string url = nodes[0].Attributes["href"].Value;
                                // 3. Get Player Career Data
                                HtmlWeb web_detail = new HtmlWeb();
                                HtmlDocument doc_detail = web_detail.Load("https://www.basketball-reference.com/" + url);

                                //[2]G、PTS、TRB、AST、FG、FG3、FT、eFG、PER、WS
                                //*[@id='info']/div[4]/div[{2}]/div[{1}]/p[2] {row} {col}
                                for (int row_data = 2; row_data <= 4; row_data++)
                                {
                                    for (int col_data = 1; col_data <= 4; col_data++)
                                    {
                                        //var data = doc_detail.DocumentNode.ChildNodes.Where(x => x.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/h4")[0].InnerText == "G").SelectMany(x => x.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[col_data]/p[2]"));
                                        if (doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/h4") != null)
                                        {
                                            switch (doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/h4")[0].InnerText)
                                            {
                                                case "G":
                                                    player.G = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "PTS":
                                                    player.PTS = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "TRB":
                                                    player.TRB = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "AST":
                                                    player.AST = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "FG%":
                                                    player.FG = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "FG3%":
                                                    player.FG3 = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "FT%":
                                                    player.FT = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "eFG%":
                                                    player.eFG = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "PER":
                                                    player.PER = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                case "WS":
                                                    player.WS = doc_detail.DocumentNode.SelectNodes($"//*[@id='info']/div[4]/div[{row_data}]/div[{col_data}]/p[2]")[0].InnerText;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                players.Add(player);
                                //Testing
                                Console.WriteLine(i + "///" + player.Name + " " + player.G + " " + player.PTS + " " + player.TRB + " " + player.AST + " " + player.FG + " " + player.FG3 + " " + player.FT + " " + player.eFG + " " + player.PER + " " + player.WS);
                            }
                            //驗證為空的節點
                            else 
                            {
                                Console.WriteLine(i + "///" + nodes[0].InnerText+" "+ nodes[0].Attributes["href"].Value);
                            }
                        }
                    }
                    //升冪排序
                    players.Sort((x, y) => x.Name.CompareTo(y.Name));
                    // 4. Output To Char CSV.
                    FileStream fileStream = new FileStream("./" + ch.ToUpper(), System.IO.FileMode.Open, System.IO.FileAccess.Write);
                    
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
        public string G { get; set; }
        public string PTS { get; set; }
        public string TRB { get; set; }
        public string AST { get; set; }
        public string FG { get; set; }
        public string FG3 { get; set; }
        public string FT { get; set; }
        public string eFG { get; set; }
        public string PER { get; set; }
        public string WS { get; set; }
    }
}
