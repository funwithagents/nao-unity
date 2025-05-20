using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageGenericNao : NaoMessage
    {
        private const string MESSAGE_ID = "GenericNao";

        [JsonProperty("text")]
        public string m_Text;

        public NaoMessageGenericNao(string text)
            : base(MESSAGE_ID)
        {
            m_Text = text;
        }
    }
}
