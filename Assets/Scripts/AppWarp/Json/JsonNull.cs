using System.Text;
namespace NeonShooter.AppWarp.Json
{
    public class JsonNull : IJsonObject
    {
        public StringBuilder AppendTo(StringBuilder sb)
        {
            return sb;
        }
    }
}
