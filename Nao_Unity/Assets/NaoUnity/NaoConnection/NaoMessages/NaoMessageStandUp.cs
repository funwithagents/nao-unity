using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageStandUp : NaoMessage
    {
        private const string MESSAGE_ID = "StandUp";

        public NaoMessageStandUp()
            : base(MESSAGE_ID)
        {
        }
    }
}
