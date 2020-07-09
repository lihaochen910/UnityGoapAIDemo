using UnityEngine;
using HutongGames.PlayMaker;

public class Action_Boss08_Atk3_End : FsmStateAction {
	
	public override void OnEnter () {
		Owner.transform.position =
			Owner.GetComponent< NodeCanvas.Framework.Blackboard > ().GetValue< Vector3 > ( Action_Boss08_Atk3_Start.BB_KEY );
	}

}
