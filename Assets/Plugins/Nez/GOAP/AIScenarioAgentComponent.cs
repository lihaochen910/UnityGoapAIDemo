using UnityEngine;

namespace Nez.AI.GOAP {
	
	public class AIScenarioAgentComponent : MonoBehaviour {

		public AIScenario scenario;
		public AIScenarioAgent agent;
		
		private void Awake () {
			agent = new AIScenarioAgent ();
		}

		private void Start () {
			Load ();
		}

		public void Load () {
			agent.Setup ( scenario, gameObject );
		}
	}
}
