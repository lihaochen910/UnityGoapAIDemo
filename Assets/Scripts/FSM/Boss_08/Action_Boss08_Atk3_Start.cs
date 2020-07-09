using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk3_Start : FsmStateAction {

	public const string BB_KEY = "Atk03_StartPoint";
	public const string BB_KEY_2 = "Atk03_TargetPoint";
	
	public override void OnEnter () {
		Owner.GetComponent<NodeCanvas.Framework.Blackboard> ().SetValue ( BB_KEY, Owner.transform.position );
		
		Transform target = Owner.GetComponent<NodeCanvas.Framework.Blackboard> ().GetValue<Transform> ( "Target" );
		Owner.GetComponent<NodeCanvas.Framework.Blackboard> ().SetValue ( BB_KEY_2, target.position );
	}

}
