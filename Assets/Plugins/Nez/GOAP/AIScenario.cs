using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;


namespace Nez.AI.GOAP {

	[CreateAssetMenuAttribute ( fileName = "Scenario", menuName = "New AI Scenario", order = 1 )]
	public class AIScenario : ScriptableObject {
		public                   AIScenarioCondition conditions;
		[HideInInspector] public AIScenarioAction[]  actions = new AIScenarioAction[0];
		[HideInInspector] public AIScenarioGoal[]    goals   = new AIScenarioGoal[0];

//		private void OnEnable () {
//			foreach ( var cond in conditions.list ) {
//				Debug.Log ( cond.type.GetType () );
//			}
//		}
	}

	[Serializable]
	public class AIScenarioGoal {
		public string           name;
		public bool             isOpened;
		public AIScenarioItem[] conditions;

		public AIScenarioGoal () {
			name       = "<Unnamed>";
			isOpened   = true;
			conditions = new AIScenarioItem[0];
		}
	}

	[Serializable]
	public class AIScenarioAction {
		public string           name;
//		public string           state;
		public int              cost;
		public bool             isOpened;
		public bool             isActived = true;
		public AIScenarioItem[] pre;
		public AIScenarioItem[] post;

		public AIScenarioAction () {
			name     = "<Unnamed>";
//			state    = name;
			cost     = 0;
			isOpened = true;
			pre      = new AIScenarioItem[0];
			post     = new AIScenarioItem[0];
		}
	}

	[Serializable]
	public struct AIScenarioItem {
		public int  id;
		public bool value;
	}

	[Serializable]
	public class AIScenarioCondition {
		public AIScenarioConditionItem[] list     = new AIScenarioConditionItem[0];
//		public int                       serialId = -1;

		public AIScenarioCondition Clone () {
			var clone = new AIScenarioCondition ();
			clone.list     = new AIScenarioConditionItem[list.Length];
//			clone.serialId = serialId;
			for ( int i = 0, n = list.Length; i < n; i++ ) {
				clone.list[ i ] = list[ i ];
			}

			return clone;
		}

		public string GetName ( int id ) {
			int index = Array.FindIndex ( list, x => x.id == id );
			return ( index >= 0 && index < list.Length ) ? list[ index ].name : null;
		}
//
		public int GetID ( string aConditionName ) {
			int index = Array.FindIndex ( list, x => x.name.Equals ( aConditionName ) );
			return ( index >= 0 && index < list.Length ) ? list[ index ].id : -1;
		}
		
		public string this [ int aIndex ] {
			get { return ( aIndex >= 0 && aIndex < list.Length ) ? list[ aIndex ].name : null; }
		}

		public int Count {
			get { return list.Length; }
		}
	}

	[Serializable]
	public class AIScenarioConditionItem {
		public int id;
		public string name;

		[ClassExtends ( typeof ( AICondition ), AllowAbstract = false )]
		public ClassTypeReference type;
		[NonSerialized]  public object      typeInstance;
		[NonSerialized]  public FieldInfo[] publicFields;
		[SerializeField] public string      typeData;

		public void Save () {
			if ( typeInstance != null ) {
				JsonSerializerSettings settings = new JsonSerializerSettings ();
				settings.Culture = CultureInfo.InvariantCulture;
				settings.Formatting = Formatting.Indented;
				settings.DefaultValueHandling = DefaultValueHandling.Include;
				settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
				typeData = JsonConvert.SerializeObject ( typeInstance, settings );
			}
		}

		public object Load () {
			
			if ( type == null || type.Type == null ) {
				return null;
			}
			
			var instance = JsonConvert.DeserializeObject ( typeData, type.Type );

			if ( instance == null ) {
				return type.Type.Assembly.CreateInstance ( type.Type.ToString () );
			}
			
			return instance;
		}
	}
}
