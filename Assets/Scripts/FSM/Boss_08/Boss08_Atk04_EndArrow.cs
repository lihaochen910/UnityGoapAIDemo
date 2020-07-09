using UnityEngine;
using Nez.AI.FSM;


public class Boss08_Atk04_EndArrow : SimpleStateMachine< Boss08_Atk04_EndArrow.State > {
    public enum State {
        Fly,
        Landed,
        Fade
    }
    
    public string DamageSource = "Boss08_a";
    public float flySpeed;
    public float flySpeedAdd;
    public float degreeSpeed;
    public float FlyTrackWaitTime;
    public float FlyTrackWaitTimeRandomRange;
    public Transform target;

    private Animator                      _animator;
    private Prime31.CharacterController2D _platformController;
    private DamageDealer                  _damageDealer;
    private float                          _timer;
    private bool                          _flyTrackEnable;
    private bool                          _flyTrackActived;
    private float _flyTrackWaitTimeRandomRange;

    public Vector3 flyDir;
    public Vector2 velocity;
    public Vector3 targetPosition;

    private void Awake () {
        _animator           = transform.Find ( "renderer" ).GetComponent< Animator > ();
        _platformController = GetComponent< Prime31.CharacterController2D > ();
    }

    private void Start () {
        
        _damageDealer = DamageDealer.NewEnemy ();
        _damageDealer.SetDirection ( DamageDirection.Neutral, transform );
        _damageDealer.ignoreReceiverOnDealDamage = true;
        _damageDealer.monsterID                  = DamageSource;
        _damageDealer.attackType                 = AttackType.Heavy;
        // _damageDealer.damage =
        //     LevelMonsterStrengthManager.Instance.GetDamage ( _damageDealer.monsterID, "Atk_4" );
        
        _flyTrackEnable = DamageSource != "Boss08_a";
        _flyTrackEnable = true;
        _flyTrackActived = false;
        _flyTrackWaitTimeRandomRange = Random.Range ( FlyTrackWaitTime - FlyTrackWaitTimeRandomRange,
            FlyTrackWaitTime + FlyTrackWaitTimeRandomRange );
        
        initialState = State.Fly;
    }

    void Fly_Enter () {
        _animator.gameObject.SetActive ( false );
    }

    void Fly_Tick () {
        
        _timer += CupheadTime.Delta[ CupheadTime.Layer.Enemy ];
        if ( _flyTrackEnable && _timer >= _flyTrackWaitTimeRandomRange ) {
            velocity = ( target.position - transform.position ).normalized;
            _timer          = 0f;
            _flyTrackEnable = false;
            _flyTrackActived = true;
        }

        if ( !_flyTrackActived ) {
            
            flyDir = ( targetPosition - transform.position ).normalized;

            var sign = Vector3.Cross ( flyDir, velocity.normalized ).z;

            if ( sign > 0 ) {
                velocity = MathUtils.Rotate ( velocity, -degreeSpeed );
            }
            else {
                velocity = MathUtils.Rotate ( velocity, +degreeSpeed );
            }
        }
        
        flySpeed += flySpeedAdd * CupheadTime.Delta[ CupheadTime.Layer.Enemy ];

        _platformController.Move ( velocity * flySpeed * CupheadTime.Delta[ CupheadTime.Layer.Enemy ] );

        if ( _platformController.collisionState.hasCollision () )
            currentState = State.Landed;
    }

    void Landed_Enter () {
        
        transform.Find ( "pse_effect_10706_tuowei" )?.gameObject.SetActive ( false );
        _animator.gameObject.SetActive ( true );
        _animator.Play ( "Boss_08_Atk04texiao" );

        Collider2D hitCollider2D =
            Physics2D.OverlapBox ( _animator.transform.position + new Vector3 ( -0.1f, -0.34f ), new Vector2 ( 1.7f, 3.17f ), LayerMask.GetMask ( "PlayerBody" ) );

        if ( hitCollider2D != null ) {
            _damageDealer.DealDamage ( hitCollider2D.gameObject, hitCollider2D.transform.position );
        }
    }

    void Landed_Tick () {
        if ( _animator.GetCurrentAnimationProgress () >= 1f ) {
            currentState = State.Fade;
        }
    } 
    
    void Fade_Enter () {
        Object.Destroy ( gameObject );
    }
}
