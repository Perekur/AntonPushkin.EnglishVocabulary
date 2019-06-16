using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PushkinA.EnglishVocabulary.Services
{

    //see src https://www.codeproject.com/Articles/12711/Google-Translator
    public interface ITranslationService
    {
        string Translate(string text, string fromCulture, string toCulture);
    }

    public class TranslationService : ITranslationService
    {
        private readonly Regex _regex;
        public TranslationService()
        {
            _regex = new Regex("([^[,^\\\",^\\]]+)");
        }

        /// Translates the text.///
        /// The input.
        /// The language pair.
        ///
        public string Translate(string text, string fromCulture, string toCulture)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return TranslateText(text, string.Format("{0}|{1}", fromCulture, toCulture));
        }

        ///
        /// Translate Text using Google Translate///
        /// The string you want translated
        /// 2 letter Language Pair, delimited by "|".
        /// e.g. "en|da" language pair means to translate from English to Danish
        /// The encoding.
        /// Translated to String
        private string TranslateText(string input, string langPair)
        {
            var langs = langPair.Split(new []{"|"}, StringSplitOptions.RemoveEmptyEntries);


            string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={langs[0]}&tl={langs[1]}&dt=t&q={HttpUtility.UrlEncode(input)}";

            //var url = $"https://translate.google.com/?hl=en&text={input}&langpair={langPair}#view=home&op=translate&sl={langs[0]}&tl=langs[1]]&text={input}";
            //string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", input, langPair);
            string result = String.Empty;

            using (var webClient = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
            {
                //webClient.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 " +
                                             "(KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                webClient.Headers.Add("content-type", "application/json");

                result = webClient.DownloadString(url);                
            }

            var matches = _regex.Matches(result);
            foreach (Match match in matches)
            {
                return match.Value;
            }

            return string.Empty;
        }


        private string ConvertUTF8ToDefault(string data)
        {
            var defEnc = System.Text.Encoding.ASCII;
            var utf8 = Encoding.UTF8;

            byte[] utfBytes = utf8.GetBytes(data);
            byte[] defBytes = Encoding.Convert(utf8, defEnc, utfBytes);
            return defEnc.GetString(defBytes);
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
