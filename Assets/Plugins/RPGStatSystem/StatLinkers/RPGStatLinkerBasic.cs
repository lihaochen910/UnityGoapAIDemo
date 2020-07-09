using UnityEngine;

namespace RPGStatSystem {
	/// <summary>
	/// Basic implementation of a RPGStatLinker. Returns a percentage 
	/// of the Linked Stat
	/// </summary>
	public class RPGStatLinkerBasic : RPGStatLinker {
		/// <summary>
		/// The Ratio of the linked stat to use
		/// </summary>
		private float _ratio;

		/// <summary>
		/// Gets the Ratio value. Read Only
		/// </summary>
		public float Ratio {
			get { return _ratio; }
		}

		/// <summary>
		/// returns the ratio of the linked stat as the linker's value
		/// </summary>
		public override float GetValue() {
			return LinkedStat.StatValue * _ratio;
		}

		public RPGStatLinkerBasic(float ratio) {
			this._ratio = ratio;
		}

		public RPGStatLinkerBasic(float ratio, RPGStat linkedStat) : base(linkedStat) {
			this._ratio = ratio;
		}
	}
}