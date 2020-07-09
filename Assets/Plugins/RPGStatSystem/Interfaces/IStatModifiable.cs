namespace RPGStatSystem {
    /// <summary>
    /// Allows the stat to use modifiers
    /// </summary>
    public interface IStatModifiable {
        float StatModifierValue { get; }

        int GetModifierCount();
        RPGStatModifier GetModifierAt(int index);

        void AddModifier(RPGStatModifier mod);
        void RemoveModifier(RPGStatModifier mod);
        void ClearModifiers();
        void UpdateModifiers();
    }
}
