using System;

namespace NeonShooter.Utils
{
    public class InvokableAction<T>
    {
        public event Action<T> Action;

        public void Invoke(T args)
        {
            if (Action != null) Action.Invoke(args);
        }
    }
}
