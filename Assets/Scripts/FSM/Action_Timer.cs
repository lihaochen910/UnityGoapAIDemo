using HutongGames.PlayMaker;

namespace Assets.Scripts.FSM
{
    /// <summary>
    /// 定时发送消息
    /// </summary>
    [ActionCategory(ActionCategory.Time)]
    public class Action_Timer : FsmStateAction
    {
        public float Time;
        public CupheadTime.Layer TimeLayer;
        public FsmEvent OnTimeElapsedEvent;

        private float _timer;

		public override void OnEnter () {

			_timer = 0f;
		}

		public override void OnUpdate()
        {
            _timer += CupheadTime.Delta[TimeLayer];

            if (_timer >= Time)
                Fsm.Event(OnTimeElapsedEvent);
        }
    }
}
