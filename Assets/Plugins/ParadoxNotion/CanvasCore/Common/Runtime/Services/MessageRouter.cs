using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using Logger = ParadoxNotion.Services.Logger;

namespace ParadoxNotion.Services
{

    ///Automaticaly added to a gameobject when needed.
    ///Handles forwarding Unity event messages to listeners that need them as well as Custom event forwarding.
    ///Notice: this is a partial class. Add your own methods to forward events as you please.
    public partial class MessageRouter : MonoBehaviour
            , IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
            IDragHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler
    {

        ///Dispatched messages are encapsulated within this data class if the target method uses a MessageData parameter type.
        public class MessageData
        {
            public GameObject receiver { get; private set; }
            public object sender { get; private set; }
            public MessageData(GameObject receiver, object sender) {
                this.receiver = receiver;
                this.sender = sender;
            }
        }

        ///Dispatched messages are encapsulated within this data class if the target method uses a MessageData parameter type.
        public class MessageData<T> : MessageData
        {
            public T value { get; private set; }
            public MessageData(T value, GameObject receiver, object sender) : base(receiver, sender) {
                this.value = value;
            }
        }

        //The flags used to find methods. Remark: Declared Only
        private const BindingFlags METHOD_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        //The event names and the objects subscribed to them
        private Dictionary<string, List<object>> listeners = new Dictionary<string, List<object>>(System.StringComparer.OrdinalIgnoreCase);

        //Animator reference required to handle OnAnimatorMove correctly.
        private object _animator;
        private Animator animator {
            get
            {
                if ( _animator == null ) {
                    _animator = GetComponent<Animator>();
                    if ( _animator == null ) {
                        _animator = new object();
                    }
                }
                return _animator as Animator;
            }
        }

        //-------------------------------------------------
        public void OnPointerEnter(PointerEventData eventData) {
            Dispatch("OnPointerEnter", eventData);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Dispatch("OnPointerExit", eventData);
        }

        public void OnPointerDown(PointerEventData eventData) {
            Dispatch("OnPointerDown", eventData);
        }

        public void OnPointerUp(PointerEventData eventData) {
            Dispatch("OnPointerUp", eventData);
        }

        public void OnPointerClick(PointerEventData eventData) {
            Dispatch("OnPointerClick", eventData);
        }

        public void OnDrag(PointerEventData eventData) {
            Dispatch("OnDrag", eventData);
        }

        public void OnDrop(BaseEventData eventData) {
            Dispatch("OnDrop", eventData);
        }

        public void OnScroll(PointerEventData eventData) {
            Dispatch("OnScroll", eventData);
        }

        public void OnUpdateSelected(BaseEventData eventData) {
            Dispatch("OnUpdateSelected", eventData);
        }

        public void OnSelect(BaseEventData eventData) {
            Dispatch("OnSelect", eventData);
        }

        public void OnDeselect(BaseEventData eventData) {
            Dispatch("OnDeselect", eventData);
        }

        public void OnMove(AxisEventData eventData) {
            Dispatch("OnMove", eventData);
        }

        public void OnSubmit(BaseEventData eventData) {
            Dispatch("OnSubmit", eventData);
        }

        //-------------------------------------------------
        void OnMouseDown() {
            Dispatch("OnMouseDown");
        }

        void OnMouseDrag() {
            Dispatch("OnMouseDrag");
        }

        void OnMouseEnter() {
            Dispatch("OnMouseEnter");
        }

        void OnMouseExit() {
            Dispatch("OnMouseExit");
        }

        void OnMouseOver() {
            Dispatch("OnMouseOver");
        }

        void OnMouseUp() {
            Dispatch("OnMouseUp");
        }

        //-------------------------------------------------
        void OnEnable() {
            Dispatch("OnEnable");
        }

        void OnDisable() {
            Dispatch("OnDisable");
        }

        void OnDestroy() {
            Dispatch("OnDestroy");
        }

        //-------------------------------------------------
        void OnTransformChildrenChanged() {
            Dispatch("OnTransformChildrenChanged");
        }

        void OnTransformParentChanged() {
            Dispatch("OnTransformParentChanged");
        }

        //-------------------------------------------------
        void OnAnimatorIK(int layerIndex) {
            Dispatch("OnAnimatorIK", layerIndex);
        }

        void OnAnimatorMove() {
            if ( !Dispatch("OnAnimatorMove") && animator.applyRootMotion ) {
                animator.ApplyBuiltinRootMotion();
            }
        }

        //-------------------------------------------------
        void OnBecameInvisible() {
            Dispatch("OnBecameInvisible");
        }

        void OnBecameVisible() {
            Dispatch("OnBecameVisible");
        }

        //-------------------------------------------------
        void OnControllerColliderHit(ControllerColliderHit hit) {
            Dispatch("OnControllerColliderHit", hit);
        }

        void OnParticleCollision(GameObject other) {
            Dispatch("OnParticleCollision", other);
        }

        //-------------------------------------------------
        void OnCollisionEnter(Collision collisionInfo) {
            Dispatch("OnCollisionEnter", collisionInfo);
        }

        void OnCollisionExit(Collision collisionInfo) {
            Dispatch("OnCollisionExit", collisionInfo);
        }

        void OnCollisionStay(Collision collisionInfo) {
            Dispatch("OnCollisionStay", collisionInfo);
        }

        void OnCollisionEnter2D(Collision2D collisionInfo) {
            Dispatch("OnCollisionEnter2D", collisionInfo);
        }

        void OnCollisionExit2D(Collision2D collisionInfo) {
            Dispatch("OnCollisionExit2D", collisionInfo);
        }

        void OnCollisionStay2D(Collision2D collisionInfo) {
            Dispatch("OnCollisionStay2D", collisionInfo);
        }

        //-------------------------------------------------
        void OnTriggerEnter(Collider other) {
            Dispatch("OnTriggerEnter", other);
        }

        void OnTriggerExit(Collider other) {
            Dispatch("OnTriggerExit", other);
        }

        void OnTriggerStay(Collider other) {
            Dispatch("OnTriggerStay", other);
        }

        void OnTriggerEnter2D(Collider2D other) {
            Dispatch("OnTriggerEnter2D", other);
        }

        void OnTriggerExit2D(Collider2D other) {
            Dispatch("OnTriggerExit2D", other);
        }

        void OnTriggerStay2D(Collider2D other) {
            Dispatch("OnTriggerStay2D", other);
        }

        ///----------------------------------------------------------------------------------------------

        ///Add a listener to several messages
        public void Register(object target, params string[] messages) {

            if ( target == null ) {
                return;
            }

            for ( var i = 0; i < messages.Length; i++ ) {

                var message = messages[i];

                if ( string.IsNullOrEmpty(message) ) {
                    Logger.LogError("Registration message name is null or empty.", "Events", target);
                    continue;
                }

                var method = target.GetType().GetMethod(message, METHOD_FLAGS);
                if ( method == null ) {
                    Logger.LogError(string.Format("Type '{0}' does not implement a method named '{1}', for the registered event to use.", target.GetType().FriendlyName(), message), "Events", target);
                    continue;
                }

                List<object> targetObjects = null;
                if ( !listeners.TryGetValue(message, out targetObjects) ) {
                    targetObjects = new List<object>();
                    listeners[message] = targetObjects;
                }

                if ( !targetObjects.Contains(target) ) {
                    targetObjects.Add(target);
                }
            }
        }

        ///Register a delegate callback to a message directly
        public void RegisterCallback(string message, Action callback) { Internal_RegisterCallback(message, callback); }
        public void RegisterCallback<T>(string message, Action<T> callback) { Internal_RegisterCallback(message, callback); }
        void Internal_RegisterCallback(string message, Delegate callback) {

            if ( string.IsNullOrEmpty(message) ) {
                Logger.LogError("Registration message name is null or empty.", "Events");
                return;
            }

            List<object> targetObjects = null;
            if ( !listeners.TryGetValue(message, out targetObjects) ) {
                targetObjects = new List<object>();
                listeners[message] = targetObjects;
            }
            if ( !targetObjects.Contains(callback) ) {
                targetObjects.Add(callback);
            }
        }

        ///Remove a listener completely from all messages
        public void UnRegister(object target) {

            if ( target == null ) {
                return;
            }

            foreach ( var message in listeners.Keys ) {
                foreach ( var o in listeners[message].ToArray() ) {

                    if ( o == target ) {
                        listeners[message].Remove(target);
                        continue;
                    }

                    if ( o is Delegate ) {
                        var delTarget = ( o as Delegate ).Target;
                        if ( delTarget == target ) {
                            listeners[message].Remove(o);
                        }
                    }
                }
            }
        }

        ///Remove a listener from the specified messages
        public void UnRegister(object target, params string[] messages) {

            if ( target == null ) {
                return;
            }

            for ( var i = 0; i < messages.Length; i++ ) {

                var message = messages[i];
                if ( string.IsNullOrEmpty(message) || !listeners.ContainsKey(message) ) {
                    continue;
                }

                foreach ( var o in listeners[message].ToArray() ) {

                    if ( o == target ) {
                        listeners[message].Remove(target);
                        continue;
                    }

                    if ( o is Delegate ) {
                        var delTarget = ( o as Delegate ).Target;
                        if ( delTarget == target ) {
                            listeners[message].Remove(o);
                        }
                    }
                }
            }
        }

        ///Call the functions assigned to the event without argument
        public bool Dispatch(string message, object sender = null) { return Dispatch<object>(message, null, sender); }
        ///Call the functions assigned to the event with argument
        public bool Dispatch<T>(string message, T arg, object sender = null) {

            if ( sender == null ) {
                sender = this;
            }

            if ( string.IsNullOrEmpty(message) ) {
                Logger.LogError("Dispatch message name is null or empty.", "Events", sender);
                return false;
            }

            List<object> targets;
            if ( !listeners.TryGetValue(message, out targets) ) {
                return false;
            }

            for ( var i = 0; i < targets.Count; i++ ) {
                var target = targets[i];
                if ( target == null ) {
                    continue;
                }

                MethodInfo method = null;

                if ( target is Delegate ) {
                    method = ( target as Delegate ).RTGetDelegateMethodInfo();
                } else {
                    method = target.GetType().GetMethod(message, METHOD_FLAGS);
                }

                if ( method == null ) {
                    Logger.LogError(string.Format("Can't resolve method {0}.{1}.", target.GetType().Name, message), "Events", target);
                    continue;
                }

                var parameters = method.GetParameters();
                if ( parameters.Length > 1 ) {
                    Logger.LogError(string.Format("Parameters on method {0}.{1}, are more than 1.", target.GetType().Name, message), "Events", target);
                    continue;
                }

                object[] argsArray = null;
                if ( parameters.Length == 1 ) {
                    object realArg;
                    if ( typeof(MessageData).RTIsAssignableFrom(parameters[0].ParameterType) ) {
                        realArg = new MessageData<T>(arg, this.gameObject, sender);
                    } else {
                        realArg = arg;
                    }
                    argsArray = ReflectionTools.SingleTempArgsArray(realArg);
                }

                if ( target is Delegate ) {

                    ( target as Delegate ).DynamicInvoke(argsArray);

                } else {

                    if ( method.ReturnType == typeof(IEnumerator) ) {
                        MonoManager.current.StartCoroutine((IEnumerator)method.Invoke(target, argsArray));
                    } else {
                        method.Invoke(target, argsArray);
                    }
                }
            }

            return true;
        }
    }
}