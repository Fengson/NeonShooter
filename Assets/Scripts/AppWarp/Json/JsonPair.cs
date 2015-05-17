namespace NeonShooter.AppWarp.Json
{
    public class JsonPair
    {
        public string Key { get; private set; }
        public IJsonObject Value { get; private set; }

        public JsonPair(string key, IJsonObject value)
        {
            Key = key;
            Value = value;
        }

        public JsonPair(string key, object value)
        {
            Key = key;
            Value = new JsonValue(value);
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Key, Value);
        }
    }
}
