using System;

namespace NeonShooter.PlayerControl
{
    public class InvokableAction<T>
    {
        private object access;

        public event Action<T> Action;

        public InvokableAction()
            : this(null)
        {
        }

        public InvokableAction(object access)
        {
            this.access = access;
        }

        public void Invoke(T args, object access = null)
        {
            if (this.access != null && this.access != access)
                throw new InvalidOperationException("Cannot Invoke action - it has restricted access and given access object does not match the required one.");
            if (Action != null)
                Action.Invoke(args);
        }
    }
}
