using System;

namespace Nez.AI.GOAP
{
	public abstract class SimpleGOAPAgent : Agent {
		
		public float elapsedTimeInAction;
		public Action previousAction;
		Action _currentAction;
		public Action currentAction
		{
			get
			{
				return _currentAction;
			}
			set
			{
				// dont change to the current state
				if ( _currentAction == value ) {
					return;
				}
				
				
				// swap previous/current
				previousAction = _currentAction;
				_currentAction = value;

				// exit the state, fetch the next cached state methods then enter that state
				if ( previousAction != null ) {
					previousAction.onExit ();
				}

				elapsedTimeInAction = 0f;
				
				_currentAction.onEnter ();
			}
		}

		
		public virtual void init () {
		}

		
		public void tick ( float deltaTime ) {

			if ( _currentAction != null ) {
				
				elapsedTimeInAction += deltaTime;

				currentAction.onTick ();
			}
			else {
				if ( hasActionPlan () ) {
					_currentAction = base.actions.Peek ();
				}
				else {
					plan ();
				}
			}
		}
	}
}
