using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk7_Up : FsmStateAction {

	public Vector2 UpVelocity;

	private MonsterAnimationEventHandler eventHandler;
	private Enemy.Monster                controller;
	
	public override void Awake () {
		eventHandler = Owner.transform.Find ( "ActionAnimator" ).GetComponent< MonsterAnimationEventHandler > ();
		controller   = Owner.GetComponent< Enemy.Monster > ();
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
		controller.ClearVelocity ();
		controller.ActiveGravity = false;
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
		controller.ClearVelocity ();
		controller.ActiveGravity = true;
	}

	private void OnCustomEvent ( string evt ) {
		if ( evt.Equals ( "Up" ) ) {
			controller.AddForce ( UpVelocity, ForceMode2D.Force );
		}
		if ( evt.Equals ( "UpEnd" ) ) {
			controller.ClearVelocity ();
		}
	}

}
