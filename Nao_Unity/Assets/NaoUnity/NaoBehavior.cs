using Newtonsoft.Json;

namespace NaoUnity
{
    public class BehaviorInfos
    {
        [JsonProperty("id")]
        public string m_Id;
        [JsonProperty("behaviorName")]
        public string m_BehaviorName;
        [JsonProperty("localizedName")]
        public LocalizedName m_LocalizedName;
        [JsonProperty("description")]
        public string m_Description;
    }

    public class LocalizedName
    {
        public string en_US;
        public string fr_FR;
    }
}
