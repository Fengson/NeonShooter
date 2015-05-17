using System.Collections.Generic;
using System.Text;

namespace NeonShooter.AppWarp.Json
{
    public class JsonObject : IJsonObject
    {
        Dictionary<string, JsonPair> pairs;

        public bool Empty { get { return pairs.Count == 0; } }

        public JsonObject(params JsonPair[] pairs)
        {
            this.pairs = new Dictionary<string, JsonPair>();
            foreach (var p in pairs) this.pairs[p.Key] = p;
        }

        public void Append(JsonPair pair)
        {
            pairs[pair.Key] = pair;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{ ");
            int i = 0;
            foreach (var pair in pairs.Values)
            {
                sb.AppendFormat("{0}", pair);
                if (i < pairs.Count - 1) sb.Append(',');
                sb.Append(' ');
                i++;
            }
            sb.Append("}");
            return sb.ToString();
        }
    }
}
