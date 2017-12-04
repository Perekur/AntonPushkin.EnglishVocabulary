using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PushkinA.EnglishVocabulary.Model
{        
    public class Question
    {

        [XmlAttribute]
        public DateTime ShowDateStart { get; set; }

        [XmlAttribute]
        public DateTime ShowDateEnd { get; set; }

        [XmlAttribute]
        public string ForeignText { get; set; }

        [XmlAttribute]
        public string NativeText { get; set; }

        public Question()
        {
            ShowDateStart = DateTime.Now;
            ShowDateEnd = DateTime.Now.Date.AddDays(7);
        }
    }
}
