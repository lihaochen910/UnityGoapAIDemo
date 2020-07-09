using UnityEditor;
using UnityEngine;

namespace Nez.AI.GOAP {
	
	[CustomEditor ( typeof ( AIScenarioAgentComponent ) )]
	public class AIScenarioAgentComponentInspector : Editor {

		private AIScenarioAgentComponent self;
		private void OnEnable () {
			self = target as AIScenarioAgentComponent;
		}

		public override void OnInspectorGUI () {
			base.OnInspectorGUI ();

			if ( Application.isPlaying ) {
				
				if ( GUILayout.Button ( "AI Debugger" ) ) {
					GoapAIDebuggerWindow.OpenWindow ( self.agent );
				}
				
				if ( GUILayout.Button ( "Reload Scenario" ) ) {
					( target as AIScenarioAgentComponent ).Load ();
				}

				if ( self.agent != null ) {
					GUILayout.Space ( 10f );
					GUILayout.Label ( $"当前计划: {(self.agent.hasActionPlan () ? self.agent.actions?.Peek ()?.name : string.Empty)}" );
					GUILayout.Space ( 10f );
					GUILayout.Label ( $"当前目标: {(self.agent.GetGoal () != null ? self.agent.GetGoal ().name : string.Empty)}" );
					GUILayout.Space ( 10f );
					GUILayout.Label ( $"条件: " );
					foreach ( var condition in self.agent.GetConditions () ) {
						GUILayout.Label ( $"  {condition.Key} = {condition.Value?.OnCheck ()}" );
					}
					GUILayout.Space ( 10f );
				}
			}
		}
	}
}
