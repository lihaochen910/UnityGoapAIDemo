using HutongGames.PlayMaker;
using NodeCanvas.Framework;

public class Action_WriteEnemyLayerTime : FsmStateAction {

	public string key;
	public bool   writeOnEnter;
	public bool   writeOnExit       = true;
	public bool   cdFirst           = false;
	public bool   random            = false;
	public float   randomProbability = 0f;

	private Blackboard blackboard;
	public override void Awake () {
		blackboard = Owner.GetComponent< Blackboard > ();
		if ( blackboard.GetVariable< float > ( key ) == null ) {
			blackboard.AddVariable ( key, typeof ( float ) );
		}

		if ( !cdFirst ) {
			blackboard.SetValue ( key, float.MinValue );
		}
	}

	public override void OnEnter () {
		if ( writeOnEnter ) {
			if ( random ) {
				if ( MathUtils.RandomProbability ( randomProbability ) ) {
					blackboard.SetValue ( key, TimeManager.EnemyTime );
				}
			}
			else {
				blackboard.SetValue ( key, TimeManager.EnemyTime );
			}
		}
	}
	
	public override void OnExit () {
		if ( writeOnExit ) {
			if ( random ) {
				if ( MathUtils.RandomProbability ( randomProbability ) ) {
					blackboard.SetValue ( key, TimeManager.EnemyTime );
				}
			}
			else {
				blackboard.SetValue ( key, TimeManager.EnemyTime );
			}
		}
	}
}
