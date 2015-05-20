using System.Collections.Generic;
using System.Text;

namespace NeonShooter.AppWarp.Json
{
    public class JsonValue : IJsonObject
    {
        public string Value { get; set; }

        public JsonValue(object value)
        {
            Value = value.ToString();
        }

        public StringBuilder AppendTo(StringBuilder sb)
        {
            return sb.Append(Value);
        }
    }
}
