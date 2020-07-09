using ParadoxNotion;
using RPGStatSystem;


namespace Nez.AI.GOAP {

	public class IsHPLessThan : AICondition {
		
		public CompareMethod checkType = CompareMethod.LessThan;
		public float         value;

		private RPGStatCollection statCollection;

		public override void Awake () {
			statCollection = DamageDealer.FindObjectStatCollection ( gameObject );
		}

		public override bool OnCheck () {
			return OperationTools.Compare (
				statCollection.GetStat< RPGVital > ( GlobalSymbol.HP ).StatValueCurrent /
				statCollection.GetStat< RPGVital > ( GlobalSymbol.HP ).StatValue, value,
				checkType, 0 );
		}
	}
	
}
