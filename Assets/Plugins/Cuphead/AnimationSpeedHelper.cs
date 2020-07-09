// Decompiled with JetBrains decompiler
// Type: AnimationSpeedHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E20EE8E7-4E23-4A85-927C-65F4005A4EC2
// Assembly location: D:\Games\Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class AnimationSpeedHelper : AbstractMonoBehaviour
{
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private CupheadTime.Layer layer;
    [SerializeField]
    private bool ignoreGlobal;
    [SerializeField]
    private bool autoUpdate;

    private Animator animator;

    public CupheadTime.Layer Layer
    {
        get
        {
            return this.layer;
        }
        set
        {
            this.layer = value;
            this.Set();
        }
    }

    public float LayerSpeed
    {
        get
        {
            return CupheadTime.GetLayerSpeed(this.Layer);
        }
        set
        {
            CupheadTime.SetLayerSpeed(this.Layer, value);
            this.Set();
        }
    }

    public float Speed
    {
        get
        {
            return this.speed;
        }
        set
        {
            this.speed = value;
            this.Set();
        }
    }

    public bool IgnoreGlobal
    {
        get
        {
            return this.ignoreGlobal;
        }
        set
        {
            this.ignoreGlobal = value;
            this.Set();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        if (this.animator == null)
        {
            Debug.LogError("AnimationSpeedHelper needs Animator component");
            Object.Destroy(this);
        }
        else
        {
            CupheadTime.OnChangedEvent += this.Set;
            this.Set();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!this.autoUpdate)
            return;
        this.Set();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CupheadTime.OnChangedEvent -= this.Set;
    }

    protected void Set()
    {
        if (this.IgnoreGlobal)
            this.animator.speed = this.Speed * this.LayerSpeed;
        else
            this.animator.speed = this.Speed * this.LayerSpeed * CupheadTime.GlobalSpeed;
    }
}
