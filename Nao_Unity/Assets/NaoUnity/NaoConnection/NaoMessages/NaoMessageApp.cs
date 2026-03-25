using Newtonsoft.Json;
using System.Collections.Generic;

namespace NaoUnity
{
    public class NaoMessageGetAppBehaviors : NaoMessage
    {
        private const string MESSAGE_ID = "GetAppBehaviors";

        public NaoMessageGetAppBehaviors()
            : base(MESSAGE_ID)
        {
        }
    }
    public class NaoMessageRunApp : NaoMessage
    {
        private const string MESSAGE_ID = "RunApp";

        [JsonProperty("appId")]
        public string m_AppId;

        public NaoMessageRunApp(string appId)
            : base(MESSAGE_ID)
        {
            m_AppId = appId;
        }
    }
    public class NaoMessageStopApp : NaoMessage
    {
        private const string MESSAGE_ID = "StopApp";

        [JsonProperty("appId")]
        public string m_AppId;

        public NaoMessageStopApp(string appId)
            : base(MESSAGE_ID)
        {
            m_AppId = appId;
        }
    }
}
