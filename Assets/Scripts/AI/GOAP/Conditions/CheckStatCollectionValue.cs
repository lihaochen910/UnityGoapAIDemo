using ParadoxNotion;
using RPGStatSystem;

namespace Nez.AI.GOAP {
	
	public class CheckStatCollectionValue : AICondition {
		
		public string        key;
		public CompareMethod checkType = CompareMethod.GreaterOrEqualTo;
		public float         value;
		
		private RPGStatCollection statCollection;
		public override void Awake () {
			statCollection = DamageDealer.FindObjectStatCollection ( gameObject );
		}

		public override bool OnCheck () {
			
			if ( statCollection == null || statCollection.ContainStat ( key ) ) {
				return false;
			}

			return OperationTools.Compare ( statCollection.GetStatValue ( key ), value, checkType, 0 );
		}
	}
}
