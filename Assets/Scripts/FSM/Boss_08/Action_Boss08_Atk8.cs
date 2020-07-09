using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk8 : FsmStateAction {

	public string DamageKey = "Atk_8";

	private MonsterAnimationEventHandler eventHandler;
	private FaceDirectionComponent faceDirectionComponent;
	private DamageDealer _damageDealer;

	private Vector2[] frameInfos = new [] {
		new Vector2 ( 2.59f, 0.64f ), new Vector2 ( 4.37f, 1.52f ),
		new Vector2 ( 1.95f, 0.56f ), new Vector2 ( 2.85f, 2.12f ),
		new Vector2 ( 2.15f, 0.207f ), new Vector2 ( 3.24f, 1.48f )
	};

	private int current;
	
	public override void Awake () {
		eventHandler = Owner.transform.Find ( "ActionAnimator" ).GetComponent<MonsterAnimationEventHandler> ();
		faceDirectionComponent = Owner.GetComponent<FaceDirectionComponent> ();
		
		_damageDealer = DamageDealer.NewEnemy ();
		_damageDealer.SetDamageFlags ( true, false, true );
		_damageDealer.SetDirection ( DamageDirection.Neutral, Owner.transform );
		_damageDealer.ignoreReceiverOnDealDamage = false;
		_damageDealer.attackType = AttackType.Hit;
		// _damageDealer.damage     = LevelMonsterStrengthManager.Instance.GetDamage ( Owner.name, DamageKey );
	}

	public override void OnEnter () {
		eventHandler.OnCustomAnimationEvent += OnCustomEvent;
		_damageDealer.SetDirection ( (DamageDirection)faceDirectionComponent.FaceDirection, Owner.transform );
		_damageDealer.ClearIgnoredReceiver ();

		current = 0;
	}

	public override void OnExit () {
		eventHandler.OnCustomAnimationEvent -= OnCustomEvent;
	}

	private void OnCustomEvent ( string evt ) {

		var offset = frameInfos[ current ];
		var size = frameInfos[ current + 1 ];

		if ( faceDirectionComponent.FaceDirection == -1 ) {
			offset.x = -offset.x;
		}
		
		Collider2D[] hitCollider2Ds =
			Physics2D.OverlapBoxAll ( Owner.transform.position + (Vector3)offset, size, LayerMask.GetMask ( "PlayerBody" ) );

		foreach ( var hit in hitCollider2Ds ) {
			if ( hit != null ) {
				_damageDealer.DealDamage ( hit.gameObject, hit.transform.position );
			}
		}
		
		// DebugExtensions.DrawBox ( Owner.transform.position + (Vector3)offset, size, 0f, Color.red );

		current += 2;
	}

}
