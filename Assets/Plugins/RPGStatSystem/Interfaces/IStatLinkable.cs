namespace RPGStatSystem {
    /// <summary>
    /// Allows the stat to use stat linkers
    /// </summary>
    public interface IStatLinkable {
        float StatLinkerValue { get; }

        void AddLinker(RPGStatLinker linker);
        void RemoveLinker(RPGStatLinker linker);
        void ClearLinkers();
        void UpdateLinkerValue();
    }
}