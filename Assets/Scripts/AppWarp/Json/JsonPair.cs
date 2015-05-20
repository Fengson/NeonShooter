using NeonShooter.Utils;
using System.Text;

namespace NeonShooter.AppWarp.Json
{
    public class JsonPair : StringBuilderExtensions.IAppendable
    {
        public string Key { get; private set; }
        public IJsonObject Value { get; private set; }

        public bool IsNull { get { return Value == null || Value is JsonNull; } }

        public JsonPair(string key, IJsonObject value)
        {
            Key = key;
            Value = value;
        }

        public JsonPair(string key, object value)
            : this(key, new JsonValue(value))
        {
        }

        public StringBuilder AppendTo(StringBuilder sb)
        {
            return sb
                .Append(Key)
                .Append(" : ")
                .AppendOne(Value);
        }
    }
}
