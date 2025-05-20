using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageRest : NaoMessage
    {
        private const string MESSAGE_ID = "Rest";

        public NaoMessageRest()
            : base(MESSAGE_ID)
        {
        }
    }
}
