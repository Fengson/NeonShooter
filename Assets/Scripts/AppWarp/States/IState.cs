using NeonShooter.AppWarp.Json;

namespace NeonShooter.AppWarp.States
{
    public interface IState
    {
        bool Changed { get; }
        IJsonObject RelativeJson { get; }
        IJsonObject AbsoluteJson { get; }

        void ClearChanges();
        void ApplyTo(object o);
    }
}
