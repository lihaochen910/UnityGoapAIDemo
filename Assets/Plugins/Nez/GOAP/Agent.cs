using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Nez.AI.GOAP
{
	/// <summary>
	/// Agent provides a simple and concise way to use the planner. It is not necessary to use at all since it is just a convenince wrapper
	/// around the ActionPlanner making it easier to get plans and store the results.
	/// </summary>
	public abstract class Agent
	{
		public Stack<Action> actions;
		public ActionPlanner _planner;


		public Agent () {
			_planner = new ActionPlanner ();
		}


		public bool plan( bool debugPlan = false )
		{
			List<AStarNode> nodes = null;
			if( debugPlan )
				nodes = new List<AStarNode>();
			
			actions = _planner.plan( getWorldState(), getGoalState(), nodes );

			if( nodes != null && nodes.Count > 0 )
			{
				Debug.Log( "---- ActionPlanner plan ----" );
				Debug.LogFormat( "plan cost = {0}\n", nodes[nodes.Count - 1].costSoFar );
				Debug.LogFormat( "{0}\t{1}", "start".PadRight( 15 ), getWorldState().describe( _planner ) );
				for( var i = 0; i < nodes.Count; i++ )
				{
					Debug.LogFormat( "{0}: {1}\t{2}", i, nodes[i].action.GetType().Name.PadRight( 15 ), nodes[i].worldState.describe( _planner ) );
					Pool<AStarNode>.free( nodes[i] );
				}
			}

			return hasActionPlan();
		}


		public bool hasActionPlan()
		{
			return actions != null && actions.Count > 0;
		}


		public void resetPlanner () {
			_planner = new ActionPlanner ();
		}


		/// <summary>
		/// current WorldState
		/// </summary>
		/// <returns>The world state.</returns>
		abstract public WorldState getWorldState();


		/// <summary>
		/// the goal state that the agent wants to achieve
		/// </summary>
		/// <returns>The goal state.</returns>
		abstract public WorldState getGoalState();

	}

	[Serializable]
	public abstract class AICondition {
		[HideInInspector] public GameObject gameObject;
		[HideInInspector] public Transform transform;
		public Agent agent;
		public abstract bool OnCheck ();
		public virtual void Awake () {}
	}

	public class AIScenarioAgent : Agent {

		private Dictionary<string, AICondition> conditions = new Dictionary< string , AICondition >();
		private AIScenarioCondition cond;
		private AIScenarioGoal[] goals;
		private AIScenarioGoal goal;
		public override WorldState getWorldState () {
		
			var worldState = _planner.createWorldState ();

			foreach ( var condition in conditions ) {
				if ( condition.Value != null ) {
					worldState.set ( condition.Key, condition.Value.OnCheck () );
				}
			}
		
			return worldState;
		}

		public override WorldState getGoalState () {
		
			var goalState  = _planner.createWorldState ();

			if ( goal != null ) {
				
				foreach ( var condition in goal.conditions ) {
					goalState.set ( cond.GetName ( condition.id ), condition.value );
				}
			}
			
			return goalState;
		}

		public void Setup ( AIScenario scenario, GameObject target ) {
			
//			foreach ( var condition in conditions ) {
//				if ( condition.Value != null ) {
//					if ( condition.Value is MonoBehaviour ) {
//						Object.DestroyImmediate ( condition.Value as MonoBehaviour );
//					}
//				}
//			}
			
			resetPlanner ();
			conditions.Clear ();
			
			foreach ( var condition in scenario.conditions.list ) {
				if ( condition.type != null ) {
					conditions.Add ( condition.name, (AICondition)condition.Load() );
					AICondition cond = conditions[ condition.name ];
					if ( cond == null ) {
						Debug.LogError ( $"AIScenarioAgent::Setup AICondition:{condition.name} == null" );
						continue;
					} 
					cond.agent = this;
					cond.gameObject = target;
					cond.transform = target.transform;
					cond.Awake ();
				}
			}

			goals = scenario.goals;
			cond = scenario.conditions;

			foreach ( var action in scenario.actions ) {

				if ( !action.isActived ) {
					continue;
				}
				
				var actionIns = new Action ( action.name, action.cost );

				foreach ( var pre in action.pre ) {
					actionIns.setPrecondition ( cond.GetName ( pre.id ), pre.value );
//					Debug.Log ( $"setPrecondition: {cond.GetName ( pre.id )} {pre.value}" );
				}
				foreach ( var post in action.post ) {
					actionIns.setPostcondition ( cond.GetName ( post.id ), post.value );
//					Debug.Log ( $"setPostcondition: {cond.GetName ( post.id )} {post.value}" );
				}
				
				_planner.addAction ( actionIns );
			}

			if ( goals != null && goals.Length != 0 ) {
				SetupGoal ( goals[ 0 ] );
			}
		}

		public void SetupGoal ( AIScenarioGoal goal ) {
			this.goal = goal;
		}

		public AIScenarioGoal GetGoal () {
			return goal;
		}
		
		public AIScenarioGoal[] GetGoals () {
			return goals;
		}

		public AIScenarioCondition GetAIScenarioCondition () {
			return cond;
		}

		public AICondition GetCondition ( string key ) {
			if ( conditions.ContainsKey ( key ) ) {
				return conditions[ key ];
			}
			return null;
		}

		public bool CheckCondition ( string key ) {
			
			if ( conditions.ContainsKey ( key ) ) {
				if ( conditions[ key ] != null ) {
					return conditions[ key ].OnCheck ();
				}
				return false;
			}
			
			return false;
		}

		public Dictionary< string, AICondition > GetConditions () => conditions;
	}
}
