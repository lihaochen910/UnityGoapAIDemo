using UnityEngine;

namespace RPGStatSystem {
	public class RPGStatScalerLinear : RPGStatScaler {
		private float _slope = 0f;
		public float _offset = 0f;

		public RPGStatScalerLinear(float slope, float offset) {
			this._slope = slope;
			this._offset = offset;
		}

		public override float GetValue(int level) {
			return Mathf.RoundToInt((_slope * (level - 1)) + _offset);
		}
	}
}
