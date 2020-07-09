using System.Collections.Generic;

namespace Nez.AI.GOAP {
	
	/// <summary>
	/// convenince Action subclass with a typed context. This is useful when an Action requires validation so that it has some way to get
	/// the data it needs to do the validation.
	/// </summary>
	public class ActionList : Action {
		
		private List < ActionImpl > _actionList = new List < ActionImpl >();

		public ActionList ( string name ) : base ( name ) {
		}
		
		public ActionList ( string name, int cost ) : base ( name, cost ) {
		}


		public void addAction ( ActionImpl action ) {
			if ( action == null ) {
				return;
			}
			
			_actionList.Add ( action );
		}


		public override void onEnter () {
			foreach ( var action in _actionList ) {
				action._finished = false;
				action.onEnter ();
			}
		}


		public override void onTick () {
			foreach ( var action in _actionList ) {
				if ( !action._finished ) {
					action.onTick ();
				}
			}
		}


		public override void onExit () {
			foreach ( var action in _actionList ) {
				action.onExit ();
			}
		}
	}
}
