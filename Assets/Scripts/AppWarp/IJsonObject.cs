using NeonShooter.Utils;
using System.Text;

namespace NeonShooter.AppWarp
{
    public interface IJsonObject : StringBuilderExtensions.IAppendable
    {
        StringBuilder AppendTo(StringBuilder sb);
    }
}
