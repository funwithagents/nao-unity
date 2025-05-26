using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NaoUnity
{
    public class NaoMessageSetBasicAwarenessState : NaoMessage
    {
        private const string MESSAGE_ID = "SetBasicAwarenessState";

        [JsonProperty("enabled")]
        public bool m_Enabled;
        [JsonProperty("engagementMode"), JsonConverter(typeof(StringEnumConverter))]
        public BasicAwareness_EngagementMode m_EngagementMode;
        [JsonProperty("trackingMode"), JsonConverter(typeof(StringEnumConverter))]
        public BasicAwareness_TrackingMode m_TrackingMode;

        public NaoMessageSetBasicAwarenessState(bool enabled,
                                                BasicAwareness_EngagementMode engagementMode,
                                                BasicAwareness_TrackingMode trackingMode)
            : base(MESSAGE_ID)
        {
            m_Enabled = enabled;
            m_EngagementMode = engagementMode;
            m_TrackingMode = trackingMode;
        }
    }

    public enum BasicAwareness_EngagementMode
    {
        Unengaged,
        FullyEngaged,
        SemiEngaged
    }

    public enum BasicAwareness_TrackingMode
    {
        Head,
        BodyRotation,
        WholeBody,
        MoveContextually
    }
}
