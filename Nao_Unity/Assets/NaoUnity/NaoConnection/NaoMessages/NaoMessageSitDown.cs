using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoMessageSitDown : NaoMessage
    {
        private const string MESSAGE_ID = "SitDown";

        public NaoMessageSitDown()
            : base(MESSAGE_ID)
        {
        }
    }
}
