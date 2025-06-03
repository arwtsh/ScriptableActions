using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Assertions;

namespace ScriptableActions.Editor
{
    [CustomEditor(typeof(ScriptableAction))]
    internal class ScriptableActionInspector : UnityEditor.Editor
    {

        public override VisualElement CreateInspectorGUI()
        {
            //ScriptableAction scriptableAction = (ScriptableAction)target;
            //Assert.IsNotNull(scriptableAction, "Somehow, the ScriptableAction reference for it's inspector was null.");

            VisualElement container = new VisualElement();

            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            //Helpful toolpit. Basically in-editor documentation.
            HelpBox infoBox = new HelpBox("Some of the logic is handled by the InputAction system. The different event types are not consistent depending on the ActionType of the InputAction assigned. " +
                "For example, if the ActionType is Button, then the \"Cancled\" event is called if the action doesn't fire, not when releasing the button. " +
                "Only the \"Performed\" event is listened to by default, but both the \"Started\" and \"Cancled\" can be opted-in with the above toggles.", HelpBoxMessageType.Info);
            container.Add(infoBox);

            //Only display the info box while there is a input action assigned.
            PropertyField inputActionField = container.Query<PropertyField>("PropertyField:inputAction");
            Assert.IsNotNull(inputActionField, "The ScriptableAction interface tried to get a reference to property playerInput, but was unable to find it.");
            inputActionField.RegisterValueChangeCallback((args) =>
            {
                infoBox.style.display = args.changedProperty.objectReferenceValue != null ? DisplayStyle.Flex : DisplayStyle.None;
            });
            //Set the initial visibility of the help box
            SerializedProperty inputActionProperty = serializedObject.FindProperty("inputAction");
            infoBox.style.display = inputActionProperty.objectReferenceValue != null ? DisplayStyle.Flex : DisplayStyle.None;

            return container;
        }
    }
}
