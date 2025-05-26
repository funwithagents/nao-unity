using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageSay : NaoMessage
    {
        private const string MESSAGE_ID = "Say";

        [JsonProperty("text")]
        public string m_Text;

        public NaoMessageSay(string text)
            : base(MESSAGE_ID)
        {
            m_Text = text;
        }
    }
}
