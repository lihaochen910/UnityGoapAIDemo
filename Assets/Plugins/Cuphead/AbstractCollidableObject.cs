// Decompiled with JetBrains decompiler
// Type: AbstractCollidableObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E20EE8E7-4E23-4A85-927C-65F4005A4EC2
// Assembly location: Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class AbstractCollidableObject : AbstractPausableComponent
{
    private List<CollisionChild> collisionChildren = new List<CollisionChild>();

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.UnregisterAllCollisionChildren();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
        this.checkCollision(col, CollisionPhase.Enter);
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        this.checkCollision(col.collider, CollisionPhase.Enter);
    }

    protected override void OnTriggerStay2D(Collider2D col)
    {
        base.OnTriggerStay2D(col);
        this.checkCollision(col, CollisionPhase.Stay);
    }

    protected override void OnCollisionStay2D(Collision2D col)
    {
        base.OnCollisionStay2D(col);
        this.checkCollision(col.collider, CollisionPhase.Stay);
    }

    protected override void OnTriggerExit2D(Collider2D col)
    {
        base.OnTriggerExit2D(col);
        this.checkCollision(col, CollisionPhase.Exit);
    }

    protected override void OnCollisionExit2D(Collision2D col)
    {
        base.OnCollisionExit2D(col);
        this.checkCollision(col.collider, CollisionPhase.Exit);
    }

    protected virtual void checkCollision(Collider2D col, CollisionPhase phase)
    {
        GameObject gameObject = col.gameObject;
        this.OnCollision(gameObject, phase);

        string tag = gameObject.tag;
        string layer = LayerMask.LayerToName(gameObject.layer);

        //Debug.Log("AttackEventListener checkCollision() + " + col.gameObject.ToString().Colored(Colors.cyan) + " " + layer.Colored(Colors.yellow));

        if (tag == Tags.Monster.ToString() || tag == Tags.MonsterBody.ToString())
        {
            if (!this.allowCollisionEnemy)
                return;
            this.OnCollisionEnemy(gameObject, phase);
        }
        else if (tag == Tags.Player.ToString() || tag == Tags.PlayerBody.ToString())
        {
            if (!this.allowCollisionPlayer)
                return;
            this.OnCollisionPlayer(gameObject, phase);
        }
        else if (layer == Tags.DefenseBody.ToString())
        {
            //this.OnCollisionEnemy(gameObject, phase);
            this.OnCollisionPlayer(gameObject, phase);
        }
        else
            this.OnCollisionOther(gameObject, phase);
    }

    protected virtual bool allowCollisionPlayer
    {
        get
        {
            return true;
        }
    }

    protected virtual bool allowCollisionEnemy
    {
        get
        {
            return true;
        }
    }

    protected virtual void OnCollision(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionWalls(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionCeiling(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionGround(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionEnemy(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionPlayer(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase) { }

    protected virtual void OnCollisionOther(GameObject hit, CollisionPhase phase) { }

    protected void RegisterCollisionChild(GameObject go)
    {
        CollisionChild component = go.GetComponent<CollisionChild>();
        if (component == null)
            Debug.LogWarning("GameObject " + go.name + " does not contain a CollisionSwitch component");
        else
            this.RegisterCollisionChild(component);
    }

    public void RegisterCollisionChild(CollisionChild s)
    {
        this.collisionChildren.Add(s);
        s.OnAnyCollision += new CollisionChild.OnCollisionHandler(this.OnCollision);
        s.OnWallCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionWalls);
        s.OnGroundCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionGround);
        s.OnCeilingCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionCeiling);
        s.OnPlayerCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionPlayer);
        s.OnPlayerProjectileCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionPlayerProjectile);
        s.OnEnemyCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionEnemy);
        s.OnEnemyProjectileCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionEnemyProjectile);
        s.OnOtherCollision += new CollisionChild.OnCollisionHandler(this.OnCollisionOther);
    }

    protected void UnregisterCollisionChild(CollisionChild s)
    {
        if (!this.collisionChildren.Contains(s))
            return;
        s.OnAnyCollision -= new CollisionChild.OnCollisionHandler(this.OnCollision);
        s.OnWallCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionWalls);
        s.OnGroundCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionGround);
        s.OnCeilingCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionCeiling);
        s.OnPlayerCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionPlayer);
        s.OnPlayerProjectileCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionPlayerProjectile);
        s.OnEnemyCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionEnemy);
        s.OnEnemyProjectileCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionEnemyProjectile);
        s.OnOtherCollision -= new CollisionChild.OnCollisionHandler(this.OnCollisionOther);
        this.collisionChildren.Remove(s);
    }

    protected void UnregisterAllCollisionChildren()
    {
        for (int index = this.collisionChildren.Count - 1; index >= 0; --index)
            this.UnregisterCollisionChild(this.collisionChildren[index]);
    }
}

public enum CollisionPhase
{
    Enter,
    Stay,
    Exit,
}
