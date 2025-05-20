using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageGetExpressiveReactionTypes : NaoMessage
    {
        private const string MESSAGE_ID = "GetExpressiveReactionTypes";

        public NaoMessageGetExpressiveReactionTypes()
            : base(MESSAGE_ID)
        {
        }
    }
    public class NaoMessageExpressiveReaction : NaoMessage
    {
        private const string MESSAGE_ID = "ExpressiveReaction";

        [JsonProperty("reactionType")]
        public string m_ReactionType;

        public NaoMessageExpressiveReaction(string reactionType)
            : base(MESSAGE_ID)
        {
            m_ReactionType = reactionType;
        }
    }
    public class NaoMessageStopExpressiveReaction : NaoMessage
    {
        private const string MESSAGE_ID = "StopExpressiveReaction";

        [JsonProperty("reactionType")]
        public string m_ReactionType;

        public NaoMessageStopExpressiveReaction(string reactionType)
            : base(MESSAGE_ID)
        {
            m_ReactionType = reactionType;
        }
    }
}
