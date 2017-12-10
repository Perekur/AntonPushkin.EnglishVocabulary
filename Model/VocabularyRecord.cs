using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PushkinA.EnglishVocabulary.Model
{        
    public class VocabularyRecord: ObservableObject
    {
        private DateTime showDateStart;        
        [XmlAttribute]
        public DateTime ShowDateStart {
            get { return showDateStart; }
            set
            {
                if (showDateStart!=value)
                {
                    showDateStart = value;
                    RaisePropertyChanged(() => ShowDateStart);
                }
            }
        }
        
        private DateTime showDateEnd;
        [XmlAttribute]
        public DateTime ShowDateEnd
        {
            get { return showDateEnd; }
            set
            {
                if (showDateEnd != value)
                {
                    showDateEnd = value;
                    RaisePropertyChanged(() => ShowDateEnd);
                }
            }
        }
      
        private string foreignText;
        [XmlAttribute]
        public string ForeignText
        {
            get { return foreignText; }
            set {
                if (foreignText != value)
                {
                    foreignText = value;
                    RaisePropertyChanged(() => ForeignText);
                }
            }
        }
        
        private string nativeText;
        [XmlAttribute]
        public string NativeText
        {
            get { return nativeText; }
            set
            {
                if (nativeText != value)
                {
                    nativeText = value;
                    RaisePropertyChanged(() => NativeText);
                }
            }
        }

        public VocabularyRecord()
        {
            ShowDateStart = DateTime.Now;
            ShowDateEnd = DateTime.Now.Date.AddDays(7);
        }

        public VocabularyRecord(VocabularyRecord item):base()
        {
            ForeignText = item.ForeignText;
            NativeText = item.NativeText;
            ShowDateStart = item.ShowDateStart;
            ShowDateEnd = item.ShowDateEnd;
        }
    }

    public class VocabularyRecordViewModel:VocabularyRecord
    {
        private bool isSelected = false;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                }
            }
        }

        public VocabularyRecordViewModel(VocabularyRecord item) : base(item) { }

        public VocabularyRecordViewModel(VocabularyRecordViewModel item) : base(item)
        {
            IsSelected = item.IsSelected;
        }
    }
}
