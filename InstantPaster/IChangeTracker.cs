namespace InstantPaster
{
    internal delegate void CombinationChanged();

    internal interface IChangeTracker
    {
        event CombinationChanged CombinationChanged;
    }
}