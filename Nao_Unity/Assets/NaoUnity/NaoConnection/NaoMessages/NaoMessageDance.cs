using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;

namespace NaoUnity
{
    public class NaoMessageGetDanceBehaviors : NaoMessage
    {
        private const string MESSAGE_ID = "GetDanceBehaviors";

        public NaoMessageGetDanceBehaviors()
            : base(MESSAGE_ID)
        {
        }
    }
    public class NaoMessageDance : NaoMessage
    {
        private const string MESSAGE_ID = "Dance";

        [JsonProperty("danceId")]
        public string m_DanceId;

        public NaoMessageDance(string danceId)
            : base(MESSAGE_ID)
        {
            m_DanceId = danceId;
        }
    }
    public class NaoMessageStopDance : NaoMessage
    {
        private const string MESSAGE_ID = "StopDance";

        [JsonProperty("danceId")]
        public string m_DanceId;

        public NaoMessageStopDance(string danceId)
            : base(MESSAGE_ID)
        {
            m_DanceId = danceId;
        }
    }
}
