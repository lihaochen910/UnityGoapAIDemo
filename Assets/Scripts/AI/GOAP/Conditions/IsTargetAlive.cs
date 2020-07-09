using UnityEngine;
using NodeCanvas.Framework;
using RPGStatSystem;


namespace Nez.AI.GOAP {
	
	public class IsTargetAlive : AICondition {
		
		private Blackboard blackboard;
		public override void Awake () {
			blackboard = DamageDealer.FindObjectT< Blackboard > ( gameObject );
		}

		public override bool OnCheck () {

			return true;
		    
			Transform target = blackboard.GetValue< Transform > ( "Target" );

			if ( target == null ) {
				return false;
			}

			RPGStatCollection statCollection = DamageDealer.FindObjectStatCollection ( target.gameObject );
			if ( statCollection == null || !statCollection.ContainStat ( GlobalSymbol.HP ) ) {
				return false;
			}

			return statCollection.GetStat< RPGVital > ( GlobalSymbol.HP ).StatValueCurrent > 0f;
		}
	}
}
