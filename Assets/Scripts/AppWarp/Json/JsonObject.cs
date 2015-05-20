using NeonShooter.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeonShooter.AppWarp.Json
{
    public class JsonObject : IJsonObject
    {
        Dictionary<string, JsonPair> pairs;
        List<JsonPair> arrayPairs;

        public bool Empty { get { return pairs.Count == 0; } }

        public JsonObject(params JsonPair[] pairs)
        {
            this.pairs = new Dictionary<string, JsonPair>();
            foreach (var p in pairs)
                if (!(p == null || p.IsNull))
                    this.pairs[p.Key] = p;
            arrayPairs = new List<JsonPair>(pairs);
        }

        public void Append(JsonPair pair)
        {
            if (pair == null || pair.IsNull) return;
            pairs[pair.Key] = pair;
            arrayPairs.Add(pair);
        }

        public StringBuilder AppendTo(StringBuilder sb)
        {
            return sb
                .Append("{ ")
                .AppendJoin(", ", from kv in pairs select kv.Value as StringBuilderExtensions.IAppendable)
                .Append("}");
        }
    }
}
