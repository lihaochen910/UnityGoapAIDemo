// Decompiled with JetBrains decompiler
// Type: DamageReceiverChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E20EE8E7-4E23-4A85-927C-65F4005A4EC2
// Assembly location: Cuphead\Cuphead_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class DamageReceiverChild : AbstractMonoBehaviour
{
    [SerializeField]
    private DamageReceiver receiver;

    public DamageReceiver Receiver => receiver;

    protected override void Start()
    {
        base.Start();
        //this.tag = this.receiver.tag;
    }
}
