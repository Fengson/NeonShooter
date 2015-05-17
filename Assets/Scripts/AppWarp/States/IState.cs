using NeonShooter.AppWarp.Json;

namespace NeonShooter.AppWarp.States
{
    public interface IState
    {
        bool Changed { get; }

        void Clear();
        JsonObject ToJson();
    }
}
