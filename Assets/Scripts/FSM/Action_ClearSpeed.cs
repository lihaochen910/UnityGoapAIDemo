using Enemy;
using HutongGames.PlayMaker;
using UnityEngine;
/// <summary>
/// 清除怪的速度
/// </summary>
public class Action_ClearSpeed : FsmStateAction
{
    public bool ClearOnEnable = true;
    public bool ClearOnExit = false;

    private Monster controller;
    public override void Awake()
    {
        controller = Owner.GetComponent<Monster>();
    }

    public override void OnEnter()
    {
        if (ClearOnEnable)
            controller.velocity = Vector2.zero;
    }

    public override void OnExit()
    {
        if (ClearOnExit)
            controller.velocity = Vector2.zero;
    }
}
