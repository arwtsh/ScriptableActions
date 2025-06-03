using AdditionalInspectorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace ScriptableActions
{
    public enum ActionType { Performed, Started, Cancled }

    [CreateAssetMenu(fileName = "ScriptableAction", menuName = "Scriptable Objects/ScriptableAction")]
    public class ScriptableAction : ScriptableObject
    {
        private class Listeners
        {
            public Action<object> performed;
            public Action<object> started;
            public Action<object> cancled;

            public void InputPerformed(InputAction.CallbackContext context)
            {
                object value = context.ReadValueAsObject();
                performed?.Invoke(value);
            }

            public void InputStarted(InputAction.CallbackContext context)
            {
                object value = context.ReadValueAsObject();
                started?.Invoke(value);
            }

            public void InputCancled(InputAction.CallbackContext context)
            {
                object value = context.ReadValueAsObject();
                cancled?.Invoke(value);
            }
        }

        private Dictionary<IActionReciever, Listeners> allListeners = new();

#if UNITY_EDITOR
        [SerializeField]
        [TextArea(5, 5)]
        string description;
#endif

        [SerializeField]
        [Header("Player Input")]
        [DisableInInspectorWhilePlaying]
        private InputActionReference inputAction;

        [SerializeField]
        [HideInInspectorIfNull("inputAction")]
        [DisableInInspectorWhilePlaying]
        private bool shouldRegisterStarted = false;

        [SerializeField]
        [HideInInspectorIfNull("inputAction")]
        [DisableInInspectorWhilePlaying]
        private bool shouldRegisterCancled = false;

        public void AddListener(IActionReciever reciever, Action<object> call, ActionType actionType = ActionType.Performed)
        {
            Assert.IsNotNull(reciever);
            Assert.IsNotNull(call);
            Assert.IsNotNull(allListeners);

            Listeners listeners;
            if (!allListeners.TryGetValue(reciever, out listeners))
            {
                listeners = new();
                allListeners.Add(reciever, listeners);
            }
            Assert.IsNotNull(listeners);

            //Find which sub event to add the listener to.
            switch (actionType)
            {
                case ActionType.Performed:
                    listeners.performed += call;
                    break;
                case ActionType.Started:
                    listeners.started += call;
                    break;
                case ActionType.Cancled:
                    listeners.cancled += call;
                    break;
                default:
                    Debug.LogError("Enum out of range. The method AddListener of ScriptableAction " + name + " was passed in an ActionType of " + actionType + " which was not included in the switch statement.");
                    return;
            }
        }

        public void RemoveListener(IActionReciever reciever, Action<object> call, ActionType actionType = ActionType.Performed)
        {
            Assert.IsNotNull(reciever);
            Assert.IsNotNull(call);
            Assert.IsNotNull(allListeners);

            Listeners listeners;
            if (!allListeners.TryGetValue(reciever, out listeners))
            {
                //If the reciever does not have an entry, then it doesn't have any events to remove.
                return;
            }
            Assert.IsNotNull(listeners);

            //Find which sub event to add the listener to.
            switch (actionType)
            {
                case ActionType.Performed:
                    listeners.performed -= call;
                    break;
                case ActionType.Started:
                    listeners.started -= call;
                    break;
                case ActionType.Cancled:
                    listeners.cancled -= call;
                    break;
                default:
                    Debug.LogError("Enum out of range. The method RemoveListener of ScriptableAction " + name + " was passed in an ActionType of " + actionType + " which was not included in the switch statement.");
                    return;
            }
        }

        public void Invoke(IActionReciever reciever, object value = null, ActionType actionType = ActionType.Performed)
        {
            Assert.IsNotNull(reciever);
            Assert.IsNotNull(allListeners);

            Listeners listeners;
            if (!allListeners.TryGetValue(reciever, out listeners))
            {
                //If the reciever does not have an entry, then it doesn't have any events to invoke.
                return;
            }
            Assert.IsNotNull(listeners);

            //Find which sub event to add the listener to.
            switch (actionType)
            {
                case ActionType.Performed:
                    listeners.performed?.Invoke(value);
                    break;
                case ActionType.Started:
                    listeners.started?.Invoke(value);
                    break;
                case ActionType.Cancled:
                    listeners.cancled?.Invoke(value);
                    break;
                default:
                    Debug.LogError("Enum out of range. The method Invoke of ScriptableAction " + name + " was passed in an ActionType of " + actionType + " which was not included in the switch statement.");
                    return;
            }
        }
    

        public void RegisterToInput(IActionReciever reciever)
        {
            Assert.IsNotNull(inputAction, "No InputActionReference was assigned to the ScriptableAction " + name + " but RegisterToInput was called anyway.");
            Assert.IsNotNull(reciever);
            Assert.IsNotNull(allListeners);

            Listeners listeners;
            if (!allListeners.TryGetValue(reciever, out listeners))
            {
                listeners = new();
                allListeners.Add(reciever, listeners);
            }
            Assert.IsNotNull(listeners);

            inputAction.ToInputAction().performed += listeners.InputPerformed;

            if (shouldRegisterStarted)
            {
                inputAction.ToInputAction().started += listeners.InputStarted;

            }

            if (shouldRegisterCancled)
            {
                inputAction.ToInputAction().canceled += listeners.InputCancled;
            }
        }

        public void UnregisterToInput(IActionReciever reciever)
        {
            Assert.IsNotNull(inputAction, "No InputActionReference was assigned to the ScriptableAction " + name + " but UnregisterToInput was called anyway.");
            Assert.IsNotNull(reciever);
            Assert.IsNotNull(allListeners);

            Listeners listeners;
            if (!allListeners.TryGetValue(reciever, out listeners))
            {
                //If the reciever does not have an entry, then it doesn't have any events to remove.
                return;
            }
            Assert.IsNotNull(listeners);

            inputAction.ToInputAction().performed -= listeners.InputPerformed;

            if (shouldRegisterStarted)
            {
                inputAction.ToInputAction().started -= listeners.InputStarted;

            }

            if (shouldRegisterCancled)
            {
                inputAction.ToInputAction().canceled -= listeners.InputCancled;
            }
        }
    }
}
