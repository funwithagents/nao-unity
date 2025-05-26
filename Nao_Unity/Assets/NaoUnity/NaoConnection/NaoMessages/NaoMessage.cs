using Newtonsoft.Json;

namespace NaoUnity
{
    public abstract class NaoMessage
    {
        [JsonProperty("id")]
        public string m_Id;

        public NaoMessage(string id)
        {
            m_Id = id;
        }
    }
}
