using UnityEngine;

namespace Nez.AI.GOAP {

	public class AIScenarioExecutorComponent : AbstractMonoBehaviour {

		public AIScenarioAgentComponent agentComponent;

		protected override void Awake () {
			agentComponent = GetComponent< AIScenarioAgentComponent > ();
		}

		private void OnGUI () {
			GUI.Label ( new Rect ( 0, 600, 300, 100 ), $"当前Action堆栈:" );
			if ( agentComponent.agent.hasActionPlan () ) {
				int i = 0;
				foreach ( var action in agentComponent.agent.actions ) {
					GUI.Label ( new Rect ( 20, 620 + 20 * ( i + 1 ), 500, 100 ), action.name );
				}
			}
		}
	}
}
