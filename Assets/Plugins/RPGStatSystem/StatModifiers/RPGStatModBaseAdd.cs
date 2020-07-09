
namespace RPGStatSystem {
	/// <summary>
	/// Modifier that adds the value to the stat value
	/// </summary>
	[System.Serializable]
	public class RPGStatModBaseAdd : RPGStatModifier {
		/// <summary>
		/// The order in which the modifier is applied to the stat
		/// </summary>
		public override int Order { get { return 2; } }

		/// <summary>
		/// Calculates the amount to apply to the stat based off the 
		/// sum of all the stat modifier's value and the current value of
		/// the stat.
		/// </summary>
		public override float ApplyModifier(float statValue, float modValue) {
			return modValue;
		}

		/// <summary>
		/// Constructor that sets the Value and sets Stacks to true
		/// </summary>
		public RPGStatModBaseAdd(float value) : base(value) { }

		/// <summary>
		/// Constructor that sets the Value and Stacks
		/// </summary>
		public RPGStatModBaseAdd(float value, bool stacks) : base(value, stacks) { }
	}
}
