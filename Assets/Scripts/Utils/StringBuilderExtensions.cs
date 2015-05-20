using System.Collections.Generic;
using System.Text;

namespace NeonShooter.Utils
{
    public static class StringBuilderExtensions
    {
        public interface IAppendable
        {
            StringBuilder AppendTo(StringBuilder sb);
        }

        public static StringBuilder Build(this IAppendable appendable)
        {
            return new StringBuilder().AppendOne(appendable);
        }

        public static string BuildString(this IAppendable appendable)
        {
            return appendable.Build().ToString();
        }

        public static StringBuilder AppendOne(this StringBuilder sb, IAppendable appendable)
        {
            return appendable.AppendTo(sb);
        }

        public static StringBuilder AppendMany(this StringBuilder sb, IEnumerable<IAppendable> enumerable)
        {
            foreach (var s in enumerable) s.AppendTo(sb);
            return sb;
        }

        public static StringBuilder AppendJoin(this StringBuilder sb, string separator, IEnumerable<IAppendable> enumerable)
        {
            var enumerator = enumerable.GetExtendedEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.AppendTo(sb);
                if (enumerator.HasNext) sb.Append(separator);
            }
            return sb;
        }
    }
}
