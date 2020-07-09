using ParadoxNotion;


namespace Nez.AI.GOAP {
	
	public class ConditionList : AICondition {
		
		public NodeCanvas.Framework.ConditionList.ConditionsCheckMode checkMode;
		public string[] conditions;

		private AIScenarioAgent _agent;
		public override void Awake () {
			_agent = agent as AIScenarioAgent;
		}

		public override bool OnCheck () {

			if ( checkMode == NodeCanvas.Framework.ConditionList.ConditionsCheckMode.AnyTrueSuffice ) {
				foreach ( var condition in conditions ) {
					if ( _agent.CheckCondition ( condition ) ) {
						return true;
					}
				}

				return false;
			}
			
			foreach ( var condition in conditions ) {
				if ( _agent.CheckCondition ( condition ) ) {
					continue;
				}

				return false;
			}

			return true;
		}
	}
}
