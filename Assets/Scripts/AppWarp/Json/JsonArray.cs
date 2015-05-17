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

        public JsonArray(params IJsonObject[] objects)
        {
            this.objects = new List<IJsonObject>(objects);
        }

        public JsonArray(IEnumerable<IJsonObject> objects)
        {
            this.objects = new List<IJsonObject>(objects);
        }

        public void Add(IJsonObject o)
        {
            objects.Add(o);
        }

        public void Remove(IJsonObject o)
        {
            objects.Remove(o);
        }

        public void Clear()
        {
            objects.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("[ ");
            int i = 0;
            foreach (var o in objects)
            {
                sb.AppendFormat("{0}", o);
                if (i < objects.Count - 1) sb.Append(',');
                sb.Append(' ');
                i++;
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
