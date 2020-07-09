using Prime31;

namespace Nez.AI.GOAP {
	
	public class CheckCharacterController2DState : AICondition {
		
		public bool above;
		public bool below;
		
		private CharacterController2D controller;
		public override void Awake () {
			controller = DamageDealer.FindObjectT< CharacterController2D > ( gameObject );
		}

		public override bool OnCheck () {
			
			if ( controller == null ) {
				return false;
			}

			return controller.collisionState.above == above &&
			       controller.collisionState.below == below;
		}
	}
}
