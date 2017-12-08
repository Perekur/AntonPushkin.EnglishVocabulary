using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace PushkinA.EnglishVocabulary.Services
{
    public interface ISpeachService
    {
        void SpeachAsync(string word);
    }
    public class SpeachService : ISpeachService, IDisposable
    {
        private SpeechSynthesizer synth = new SpeechSynthesizer();

        public SpeachService()
        {
            synth.SelectVoiceByHints(VoiceGender.Female);
        }

        public void Dispose()
        {
            synth.Dispose();
        }

        public void SpeachAsync(string sentence)
        {
            if (string.IsNullOrEmpty(sentence)) return;
            synth.SpeakAsync(sentence);            
        }
    }
}
