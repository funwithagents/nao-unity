using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageWakeUp : NaoMessage
    {
        private const string MESSAGE_ID = "WakeUp";

        public NaoMessageWakeUp()
            : base(MESSAGE_ID)
        {
        }
    }
}
