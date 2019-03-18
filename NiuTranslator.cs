using System;
using System.Net;
using System.Net.Security;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace NiuTransDemo
{
    public class NiuTranslator
    {
        private const String host = "http://niutrans1.market.alicloudapi.com";
        private const String path = "/NiuTransServer/translation";
        private const String method = "GET";
        private const String appcode = "d334b690309d4fc4beed3579747b29a4";

        public static String ChineseToEnglish(String chineseContent)
        {
            NiuTranslator translator = new NiuTranslator();
            return translator.Translate(chineseContent,"zh","en");
        }

        public static String EnglishToChinese(String englishContent)
        {
            NiuTranslator translator = new NiuTranslator();
            return translator.Translate(englishContent, "en", "zh");
        }

        private String Translate(String content,String from,String to)
        {
            String querys = "from=" + from
                + "&src_text=" + content
                + "&to=" + to;
            String bodys = "";
            String url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (querys.Length > 0)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            if (bodys.Length > 0)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            if (httpResponse.StatusCode.ToString() != "OK")
                return null;

            //Console.WriteLine("StatusCode:" + httpResponse.StatusCode);
            //Console.WriteLine("Method:" + httpResponse.Method);
            //Console.WriteLine("Headers:" + httpResponse.Headers);
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));

            String jsonTexts = reader.ReadToEnd();
            if (!jsonTexts.Contains("tgt_text"))
                return null;
            int startIndex = jsonTexts.IndexOf("tgt_text")+11;
            int endIndex = jsonTexts.IndexOf("\",\"to\"");
            String translateResult = jsonTexts.Substring(startIndex, endIndex - startIndex);
            translateResult = translateResult.Replace("\\n", "\n");
            return translateResult;
        }
        

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
