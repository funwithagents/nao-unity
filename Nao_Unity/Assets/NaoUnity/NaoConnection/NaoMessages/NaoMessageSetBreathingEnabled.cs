using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NaoUnity
{
    public class NaoMessageSetBreathingEnabled : NaoMessage
    {
        private const string MESSAGE_ID = "SetBreathingEnabled";

        [JsonProperty("enabled")]
        public bool m_Enabled;
        [JsonProperty("chainName"), JsonConverter(typeof(StringEnumConverter))]
        public Breathing_ChainName m_ChainName;

        public NaoMessageSetBreathingEnabled(bool enabled, Breathing_ChainName chainName)
            : base(MESSAGE_ID)
        {
            m_Enabled = enabled;
            m_ChainName = chainName;
        }
    }
    public enum Breathing_ChainName
    {
        Body,
        Legs,
        Arms,
        Head
    }
}
