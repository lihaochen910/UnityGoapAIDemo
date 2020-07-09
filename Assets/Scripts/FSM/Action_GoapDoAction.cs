using HutongGames.PlayMaker;
using UnityEngine;
using Nez.AI.GOAP;

/// <summary>
/// Goap
/// </summary>
[ActionCategory(ActionCategory.CharacterController)]
public class Action_GoapDoAction : FsmStateAction {
    
    [Title ( "Goap状态列表" )]
    public GoapParams[] list;
    
    private AIScenarioAgentComponent agentComponent;
    
    public override void Awake () {
        agentComponent = Owner.GetComponent< AIScenarioAgentComponent > ();
    }

    public override void OnEnter () {
        agentComponent.agent.plan ();
    }

    public override void OnUpdate () {

        if ( !agentComponent.enabled ) {
            return;
        }

        if ( agentComponent.agent.hasActionPlan () && hasAction ( agentComponent.agent.actions.Peek ().name ) ) {
            
            Fsm.Event ( findEvent ( agentComponent.agent.actions.Peek ().name ) );
            
            if ( Fsm.ActiveState != State ) {
                Debug.Log ( $"Action_GoapDoAction 正在执行Action: {agentComponent.agent.actions.Peek ().name}" );
            }
        }
        else {
            agentComponent.agent.plan ();
        }
    }

    private string findEvent ( string action ) {
        foreach ( var param in list ) {
            if ( param.action.Equals ( action ) ) {
                return param.sendEvent;
            }
        }

        return null;
    }

    private bool hasAction ( string action ) {
        foreach ( var param in list ) {
            if ( param.action.Equals ( action ) ) {
                return true;
            }
        }

        return false;
    }
    
    public class GoapParams {
        /// <summary>
        /// 目标状态
        /// </summary>
        [Title("行为"), RequiredField]
        public string action;
        /// <summary>
        /// 响应的事件
        /// </summary>
        [Title("事件"), RequiredField]
        public string sendEvent;
    }
}
