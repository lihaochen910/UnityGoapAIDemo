using UnityEngine;
using RPGStatSystem;
using NodeCanvas.Framework;

namespace Nez.AI.GOAP {
	
	public class IsPlayerInRange : AICondition {

		public string KEY_RANGE_X;
		public string KEY_RANGE_Y;
		
		private RPGStatCollection statCollection;
		private Blackboard blackboard;
		private Transform target;

		public override void Awake () {
			statCollection = DamageDealer.FindObjectStatCollection ( gameObject );
			blackboard = DamageDealer.FindObjectT< Blackboard > ( gameObject );
		}

		public override bool OnCheck () {

			target = blackboard.GetValue< Transform > ( "Target" );

			if ( target == null ) {
				return false;
			}

			Vector3 centerOffset = Vector3.one;
			if ( statCollection.ContainStat ( GlobalSymbol.CENTER_OFFSET_X ) ) {
				centerOffset.x = statCollection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_X );
			}

			if ( statCollection.ContainStat ( GlobalSymbol.CENTER_OFFSET_Y ) ) {
				centerOffset.y = statCollection.GetStatValue ( GlobalSymbol.CENTER_OFFSET_Y );
			}

			bool xOK = string.IsNullOrEmpty ( KEY_RANGE_X ) || Mathf.Abs ( target.position.x - ( transform.position.x + centerOffset.x ) ) <
			           statCollection.GetStatValue ( KEY_RANGE_X );
			bool yOK = string.IsNullOrEmpty ( KEY_RANGE_Y ) || Mathf.Abs ( target.position.y - ( transform.position.y + centerOffset.y ) ) <
				  statCollection.GetStatValue ( KEY_RANGE_Y );

			return xOK && yOK;
		}
	}
}
