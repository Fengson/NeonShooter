using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            set
            {
                if (value == null || value is JsonNull) objects.RemoveAt(index);
                else objects[index] = value;
            }
        }

        public JsonArray(params IJsonObject[] objects)
            : this((IEnumerable<IJsonObject>)objects)
        {
        }

        public JsonArray(IEnumerable<IJsonObject> objects)
        {
            this.objects = new List<IJsonObject>(
                from js in objects
                where !(js == null || js is JsonNull) 
                select js);
        }

        public void Add(IJsonObject o)
        {
            if (o == null || o is JsonNull) return;
            objects.Add(o);
        }

        public void Remove(IJsonObject o)
        {
            if (o == null || o is JsonNull) return;
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
