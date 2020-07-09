using NodeCanvas.Framework;
using ParadoxNotion;


namespace Nez.AI.GOAP {
	
	public class CheckEnemyTimeLayerInterval : AICondition {
		
		public string        key;
		public CompareMethod checkType = CompareMethod.GreaterOrEqualTo;
		public float         value;
		
		private Blackboard blackboard;
		public override void Awake () {
			blackboard = DamageDealer.FindObjectT< Blackboard > ( gameObject );
		}

		public override bool OnCheck () {
			return OperationTools.Compare ( TimeManager.EnemyTime - blackboard.GetValue<float> ( key ), value, checkType, 0 );
		}
	}
}
