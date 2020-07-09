using System;
using UnityEngine;

namespace Nez.AI.GOAP
{
	public class SimpleGOAPComponent : AbstractMonoBehaviour {

		public CupheadTime.Layer tickTimeLayer;
		
		private SimpleGOAPAgent _agent;
		
		protected override void Awake () {
			timeLayer = tickTimeLayer;
			base.Awake ();
		}

		protected override void Update () {
			_agent.tick ( LocalDeltaTime );
		}

		public void SetAgent ( SimpleGOAPAgent agent ) {
			_agent = agent;
			_agent.init ();
		}
	}
}
