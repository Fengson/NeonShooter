using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NeonShooter.AppWarp.Json
{
    public class JsonArray : IEnumerable<IJsonObject>, IEnumerable, IJsonObject
    {
        List<IJsonObject> objects;

        public bool Empty { get { return objects.Count == 0; } }

        public IJsonObject this[int index]
        {
            get { return objects[index]; }
            set { objects[index] = value; }
        }

        public JsonArray(params IJsonObject[] pairs)
        {
            this.objects = new List<IJsonObject>(pairs);
        }

        public JsonArray(IEnumerable<IJsonObject> pairs)
        {
            this.objects = new List<IJsonObject>(pairs);
        }

        public void Add(IJsonObject pair)
        {
            objects.Add(pair);
        }

        public void Remove(IJsonObject pair)
        {
            objects.Remove(pair);
        }

        public void Clear(IJsonObject pair)
        {
            objects.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[ ");
            foreach (var o in objects)
            {
                sb.AppendFormat("{0}, ", o);
            }
            sb.Append("]");
            return sb.ToString();
        }

        public IEnumerator<IJsonObject> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
