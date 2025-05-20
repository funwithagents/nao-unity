using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NaoUnity
{
    public class NaoMessageSetTTSLanguage : NaoMessage
    {
        private const string MESSAGE_ID = "SetTTSLanguage";

        [JsonProperty("language"), JsonConverter(typeof(StringEnumConverter))]
        public NaoTTSLanguage m_Language;

        public NaoMessageSetTTSLanguage(NaoTTSLanguage language)
            : base(MESSAGE_ID)
        {
            m_Language = language;
        }
    }

    public enum NaoTTSLanguage
    {
        French,
        English
    }
}
