using UnityEngine;

namespace Nez.AI.GOAP {
	
	public class TestICondition : AICondition {

		public int     a;
		public float   b;
		public string  c;
		public Vector2 d;
		public Color   e;
		public TestIConditionEnum f;
		
		public override bool OnCheck () {
			return true;
		}
	}

	public enum TestIConditionEnum {
		A,
		B,
		C
	}
}