using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk9 : FsmStateAction {

	public float  Duration = 1f;
	public float  DamageInterval = 0.3f;
	public string DamageKey     = "Atk_9";

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent       faceDirectionComponent;
	
	public override void Awake () {
		eventHandler           = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
	}

	public override void OnEnter () {
		// eventHandler.OnCustomAnimationEvent += OnCustomEvent;
		OnCustomEvent ( string.Empty );
	}

	public override void OnExit () {
		// eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
	}

	private void OnCustomEvent ( string evt ) {

		var obj = Object.Instantiate ( Resources.Load<GameObject> ( "Prefabs/Object/Boss08_Atk09_Tentacle" ) );
		obj.transform.position = Owner.transform.position;
		
		var comp = obj.GetComponent<Boss08_Atk09_Tentacle> ();
		comp.DamageSource   = Owner.name;
		comp.DamageKey      = DamageKey;
		comp.Duration       = Duration;
		comp.DamageInterval = DamageInterval;
	}

}
