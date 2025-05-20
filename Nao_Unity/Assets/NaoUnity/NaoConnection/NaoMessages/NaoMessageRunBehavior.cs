using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageRunBehavior : NaoMessage
    {
        private const string MESSAGE_ID = "RunBehavior";

        [JsonProperty("name")]
        public string m_Name;

        public NaoMessageRunBehavior(string name)
            : base(MESSAGE_ID)
        {
            m_Name = name;
        }
    }
}
