using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NaoUnity
{
    public class NaoMessageChangeEyesColor : NaoMessage
    {
        private const string MESSAGE_ID = "ChangeEyesColor";

        [JsonProperty("color"), JsonConverter(typeof(StringEnumConverter))]
        public NaoLedColor m_Color;

        public NaoMessageChangeEyesColor(NaoLedColor color)
            : base(MESSAGE_ID)
        {
            m_Color = color;
        }
    }

    public enum NaoLedColor
    {
        white,
        red,
        green,
        blue,
        yellow,
        magenta,
        cyan
    }
}
