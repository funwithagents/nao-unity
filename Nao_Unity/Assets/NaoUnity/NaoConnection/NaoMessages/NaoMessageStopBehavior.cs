using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageStopBehavior : NaoMessage
    {
        private const string MESSAGE_ID = "StopBehavior";

        [JsonProperty("name")]
        public string m_Name;

        public NaoMessageStopBehavior(string name)
            : base(MESSAGE_ID)
        {
            m_Name = name;
        }
    }
}
