using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;

namespace NaoUnity
{
    public class NaoMessageGetBodyActionBehaviors : NaoMessage
    {
        private const string MESSAGE_ID = "GetBodyActionBehaviors";

        public NaoMessageGetBodyActionBehaviors()
            : base(MESSAGE_ID)
        {
        }
    }
    public class NaoMessageBodyAction : NaoMessage
    {
        private const string MESSAGE_ID = "BodyAction";

        [JsonProperty("bodyActionId")]
        public string m_BodyActionId;

        public NaoMessageBodyAction(string bodyActionId)
            : base(MESSAGE_ID)
        {
            m_BodyActionId = bodyActionId;
        }
    }
    public class NaoMessageStopBodyAction : NaoMessage
    {
        private const string MESSAGE_ID = "StopBodyAction";

        [JsonProperty("bodyActionId")]
        public string m_BodyActionId;

        public NaoMessageStopBodyAction(string bodyActionId)
            : base(MESSAGE_ID)
        {
            m_BodyActionId = bodyActionId;
        }
    }
}
