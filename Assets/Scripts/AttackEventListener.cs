using UnityEngine;
/// <summary>
/// 攻击事件监听器
/// </summary>
public class AttackEventListener : AbstractCollidableObject
{
    public bool ClearCallbackOnDisable = false;
    /// event不可以通过=传递，所以使用委托方式
    public delegate void OnAttackHitEvent(GameObject hitObject, Vector3 hitPoint);

    public OnAttackHitEvent OnHitEnemyEnterEvent;
    public OnAttackHitEvent OnHitEnemyStayEvent;
    public OnAttackHitEvent OnHitEnemyExitEvent;

    public OnAttackHitEvent OnHitPlayerEnterEvent;
    public OnAttackHitEvent OnHitPlayerStayEvent;
    public OnAttackHitEvent OnHitPlayerExitEvent;

    public OnAttackHitEvent OnHitOtherEnterEvent;

    protected override void OnDisable()
    {
        if ( ClearCallbackOnDisable ) {
	        ClearCallback ();
        }
    }

    protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
    {
		if ( !enabled ) {
			return;
		}

        //Debug.Log("AttackEventListener".Colored(Colors.green) + " OnCollisionPlayer : " + hit.ToString().Colored(Colors.cyan));
        if (phase == CollisionPhase.Enter)
            OnHitPlayerEnterEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
        else if (phase == CollisionPhase.Stay)
            OnHitPlayerStayEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
        else OnHitPlayerExitEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
    }

    protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
    {
		if ( !enabled ) {
			return;
		}

		if (phase == CollisionPhase.Enter)
            OnHitEnemyEnterEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
        else if (phase == CollisionPhase.Stay)
            OnHitEnemyStayEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
        else OnHitEnemyExitEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
    }

    protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
    {
		if ( !enabled ) {
			return;
		}

		if (phase == CollisionPhase.Enter)
            OnHitOtherEnterEvent?.Invoke(hit, hit.GetComponent<Collider2D>().bounds.center);
    }

    public void ClearCallback () {
	    OnHitEnemyEnterEvent = OnHitEnemyStayEvent = OnHitEnemyExitEvent =
		    OnHitPlayerEnterEvent = OnHitPlayerStayEvent = OnHitPlayerExitEvent =
			    OnHitOtherEnterEvent = null;
    }
}
