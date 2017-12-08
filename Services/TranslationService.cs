using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace PushkinA.EnglishVocabulary.Services
{
    public interface ITranslationService
    {
        string Translate(string text, string fromCulture, string toCulture);
    }

    public class TranslationService: ITranslationService
    {
        public string Translate(string text, string fromCulture, string toCulture)
        {
            fromCulture = fromCulture.ToLower();
            toCulture = toCulture.ToLower();

            // normalize the culture in case something like en-us was passed 
            // retrieve only en since Google doesn't support sub-locales
            string[] tokens = fromCulture.Split('-');
            if (tokens.Length > 1)
                fromCulture = tokens[0];

            // normalize ToCulture
            tokens = toCulture.Split('-');
            if (tokens.Length > 1)
                toCulture = tokens[0];

            string url = string.Format( //@"http://translate.google.com/translate_a/t?client=j&q={0}&hl=en&sl={1}&tl={2}",
                                        @"http://translate.google.com/translate_a/single?client=t&sl=en&tl=uk&hl=uk&dt=at&dt=bd&dt=ex&dt=ld&dt=md&dt=qca&dt=rw&dt=rm&dt=ss&dt=t&ie=UTF-8&oe=UTF-8&source=btn&ssel=6&tsel=3&kc=0&tk=835042.709783&q=father",
                                       HttpUtility.UrlEncode(text), fromCulture, toCulture);            
            // Retrieve Translation with HTTP GET call
            string html = null;
            try
            {
                WebClient web = new WebClient();

                // MUST add a known browser user agent or else response encoding doen't return UTF-8 (WTF Google?)
                web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

                // Make sure we have response encoding to UTF-8
                web.Encoding = Encoding.UTF8;
                html = web.DownloadString(url);
            }
            catch (Exception ex)
            {
                return null;
            }

            return html;
        }
    }
}
