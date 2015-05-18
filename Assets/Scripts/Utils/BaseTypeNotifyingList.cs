namespace NeonShooter.Utils
{
    public class BaseTypeNotifyingList<TBase, TDerived> : ProxyTypeNotifyingList<TBase, TDerived>
        where TDerived : TBase
    {
        public BaseTypeNotifyingList(INotifyingList<TDerived> originalList)
            : base(originalList, d => (TBase)d, b => (TDerived)b)
        {
        }
    }
}
