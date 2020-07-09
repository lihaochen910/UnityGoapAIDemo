using System;
using UnityEngine;

[RequireComponent ( typeof ( Animator ) )]
public class SimpleAnimationEventHandler : MonoBehaviour {

	public event Action< string > OnStringEvent;

	public void FireEvent ( string evt ) {
		OnStringEvent?.Invoke ( evt );
	}
}
