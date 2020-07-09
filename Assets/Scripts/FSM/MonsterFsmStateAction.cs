using HutongGames.PlayMaker;
using UnityEngine;
using RPGStatSystem;

namespace Enemy
{
    public class MonsterFsmStateAction : FsmStateAction
    {
        protected Transform transform;
        // protected Player.CharacterController2D m_Player;
        protected Monster m_Monster;
        // protected EnemyHealth m_Health;
        protected SpriteRenderer m_SpriteRenderer;
        protected RPGStatCollection m_StatCollection;
        protected PlayMakerFSM pmFSM;
        protected NodeCanvas.BehaviourTrees.BehaviourTreeOwner behaviourTreeOwner;
        protected readonly CupheadTime.Layer timeLayer = CupheadTime.Layer.Enemy;
        public override void Awake()
        {
            base.Awake();
            transform = Owner.transform;
            m_Monster = Owner.GetComponent<Monster>();
            // m_Player = GameManager.Player?.GetComponent<Player.CharacterController2D>();
            // m_Health = Owner.GetComponent<EnemyHealth>();
            m_SpriteRenderer = transform.Find("ActionAnimator")?.GetComponent<SpriteRenderer>();
            m_StatCollection = Owner.GetComponent < RPGStatCollection > ();
            pmFSM = Owner.GetComponent<PlayMakerFSM>();
            behaviourTreeOwner = Owner.GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>();

            if (m_StatCollection == null)
            {
                Debug.LogError($"MonsterFsmStateAction::Awake() {Owner.name} 物体上没有找到<RPGStatCollection>组件或字段没有赋值，无法初始化");
            }
        }

        protected Coroutine DelayTask(float time, System.Action callback)
        {
            return CupheadTime.WaitForSeconds(pmFSM, time, timeLayer, callback);
        }

        protected void CancelDelayTask(Coroutine task)
        {
            if (task != null)
                pmFSM.StopCoroutine(task);
        }
    }
}
