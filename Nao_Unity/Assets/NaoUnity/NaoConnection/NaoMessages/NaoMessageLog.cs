using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageLog : NaoMessage
    {
        private const string MESSAGE_ID = "Log";

        [JsonProperty("log")]
        public string m_Log;

        public NaoMessageLog(string log)
            : base(MESSAGE_ID)
        {
            m_Log = log;
        }
    }
}
