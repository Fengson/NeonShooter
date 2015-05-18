using System;

namespace NeonShooter.Utils
{
    public class InvokableAction<T>
    {
        public static InvokableAction<T> Public()
        {
            return new InvokableAction<T>();
        }

        public static InvokableAction<T> Private(object access)
        {
            return new InvokableAction<T>(access);
        }

        private object access;

        public event Action<T> Action;

        InvokableAction(object access = null)
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
