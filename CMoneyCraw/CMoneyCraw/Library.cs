using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMoneyCraw
{
    public class Library
    {
        public Library()
        {

        }
        public static string CallWebApi(string url, object para, string token = null)
        {
            try
            {
                string parameters = JsonConvert.SerializeObject(para);
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);

                #region Token

                if (token != null)
                {
                    req.PreAuthenticate = true;
                    req.Headers.Add("Authorization", $"Bearer {token}");
                }

                #endregion Token
                req.ContentType = "application/json";
                req.Method = "POST";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameters);
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length);
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null)
                    return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                var result = sr.ReadToEnd().Trim();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
