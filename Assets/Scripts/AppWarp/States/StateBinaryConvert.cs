using System.Collections.Generic;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public static class StateBinaryConvert
    {
        public static void WriteRelative(this BinaryWriter bw, IState state)
        {
            state.WriteRelativeBinaryTo(bw);
        }

        public static void WriteRelative<T>(this BinaryWriter bw, IEnumerable<T> enumerable)
            where T : IState
        {
            foreach (var bc in enumerable)
                bw.WriteRelative(bc);
        }

        public static void WriteAbsolute(this BinaryWriter bw, IState state)
        {
            state.WriteAbsoluteBinaryTo(bw);
        }

        public static void WriteAbsolute<T>(this BinaryWriter bw, IEnumerable<T> enumerable)
            where T : IState
        {
            foreach (var bc in enumerable)
                bw.WriteAbsolute(bc);
        }
    }
}
