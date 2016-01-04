using NeonShooter.AppWarp.Json;
using System.IO;

namespace NeonShooter.AppWarp.States
{
    public interface IState
    {
        bool Changed { get; }
        IJsonObject RelativeJson { get; }
        IJsonObject AbsoluteJson { get; }

        //int AbsoluteBinarySize { get; }

        //void WriteRelativeBinaryTo(BinaryWriter bw);
        //void WriteAbsoluteBinaryTo(BinaryWriter bw);

        void ClearChanges();
        void ApplyTo(object o);
    }
}
