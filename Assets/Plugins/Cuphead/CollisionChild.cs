// Decompiled with JetBrains decompiler
// Type: CollisionChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E20EE8E7-4E23-4A85-927C-65F4005A4EC2
// Assembly location: D:\Games\Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CollisionChild : AbstractCollidableObject
{
    [SerializeField]
    [Tooltip("OPTIONAL: Drag collision parent to this slot to register all collision events to this child. If null, no collisions are registered.")]
    private AbstractCollidableObject collisionParent;

    public event CollisionChild.OnCollisionHandler OnAnyCollision;

    public event CollisionChild.OnCollisionHandler OnWallCollision;

    public event CollisionChild.OnCollisionHandler OnGroundCollision;

    public event CollisionChild.OnCollisionHandler OnCeilingCollision;

    public event CollisionChild.OnCollisionHandler OnPlayerCollision;

    public event CollisionChild.OnCollisionHandler OnPlayerProjectileCollision;

    public event CollisionChild.OnCollisionHandler OnEnemyCollision;

    public event CollisionChild.OnCollisionHandler OnEnemyProjectileCollision;

    public event CollisionChild.OnCollisionHandler OnOtherCollision;

    protected override void Start()
    {
        base.Start();
        if (!(this.collisionParent != null))
            return;
        this.collisionParent.RegisterCollisionChild(this);
    }

    protected override void OnCollision(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnAnyCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnAnyCollision(hit, phase);
    }

    protected override void OnCollisionWalls(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnWallCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnWallCollision(hit, phase);
    }

    protected override void OnCollisionGround(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnGroundCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnGroundCollision(hit, phase);
    }

    protected override void OnCollisionCeiling(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnCeilingCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnCeilingCollision(hit, phase);
    }

    protected override void OnCollisionPlayer(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnPlayerCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnPlayerCollision(hit, phase);
    }

    protected override void OnCollisionPlayerProjectile(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnPlayerProjectileCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnPlayerProjectileCollision(hit, phase);
    }

    protected override void OnCollisionEnemy(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnEnemyCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnEnemyCollision(hit, phase);
    }

    protected override void OnCollisionEnemyProjectile(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnEnemyProjectileCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnEnemyProjectileCollision(hit, phase);
    }

    protected override void OnCollisionOther(GameObject hit, CollisionPhase phase)
    {
        // ISSUE: reference to a compiler-generated field
        if (this.OnOtherCollision == null)
            return;
        // ISSUE: reference to a compiler-generated field
        this.OnOtherCollision(hit, phase);
    }

    public delegate void OnCollisionHandler(GameObject hit, CollisionPhase phase);
}
