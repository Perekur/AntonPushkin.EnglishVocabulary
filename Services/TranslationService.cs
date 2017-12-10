using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PushkinA.EnglishVocabulary.Services
{
    public interface ITranslationService
    {
        string Translate(string text, string fromCulture, string toCulture);
    }

    public class TranslationService : ITranslationService
    {

        /// Translates the text.///
        /// The input.
        /// The language pair.
        ///
        public string Translate(string text, string fromCulture, string toCulture)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return TranslateText(text, string.Format("{0}|{1}", fromCulture, toCulture), System.Text.Encoding.Default);
        }

        ///
        /// Translate Text using Google Translate///
        /// The string you want translated
        /// 2 letter Language Pair, delimited by "|".
        /// e.g. "en|da" language pair means to translate from English to Danish
        /// The encoding.
        /// Translated to String
        private string TranslateText(string input, string languagePair, Encoding encoding)
        {
            string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, languagePair);
            string result = String.Empty;
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = encoding;
                result = webClient.DownloadString(url);
            }

            result = result.Substring(result.IndexOf("<span title=\"") + "<span title=\"".Length);
            result = result.Substring(result.IndexOf(">") + 1);
            result = result.Substring(0, result.IndexOf("</span>"));
            return result.Trim();
        }
    }
}

/*
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

        string url = string.Format( @"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}",
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

        Match m = Regex.Match(result, "");
        if (m.Success) result = m.Value; return result;
        return html;
    }
}
}
*/
