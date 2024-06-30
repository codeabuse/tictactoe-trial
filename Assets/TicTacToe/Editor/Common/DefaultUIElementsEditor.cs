using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TicTacToe
{
    [CustomEditor(typeof(Object), true)]
    public class DefaultUIElementsEditor : Editor
    {
        private static readonly Type context_menu_item_attr_type = typeof(ContextMenu);
        [SerializeField]
        private StyleSheet _editorStyles;

        private const string script_property_name = "m_Script";
        protected virtual bool showScriptField => true;
        
        public override VisualElement CreateInspectorGUI()
        {
            var scriptPropertyPassed = false;
            var root = new VisualElement();
            var layoutRoot = root;
            PropertyLayoutAttribute currentLayout = null;
            var iterator = serializedObject.GetIterator();
            
            if (iterator.NextVisible(true))
            {
                do
                {
                    VisualElement propertyElement;

                    if (!scriptPropertyPassed && 
                        iterator.propertyPath.Equals(script_property_name) && 
                        serializedObject.targetObject)
                    {
                        if (!showScriptField) continue;
                        propertyElement = CommonUIElements.CreateDisabledClickablePropertyField(
                            iterator.Copy(),
                            obj => AssetDatabase.OpenAsset(obj),
                            out _);
                        
                        scriptPropertyPassed = true;
                        layoutRoot.Add(propertyElement);
                    }
                    else
                    {
                        var property = iterator.Copy();
                        var layout = property.GetAttributeFromProperty<PropertyLayoutAttribute>();
                        if (layout is not null && !layout.Equals(currentLayout))
                        {
                            currentLayout = layout;
                            layoutRoot = layout.GetLayoutRoot(root);
                        }
                        propertyElement = CreatePropertyElement(property);
                        if (propertyElement is not null)
                            layoutRoot.Add(propertyElement);
                    }
                }
                while (iterator.NextVisible(false));
            }
            var buttonsRoot = MakeContextActionsButtons();
            root.Insert(0, buttonsRoot);
            
            if (_editorStyles)
                root.styleSheets.Add(_editorStyles);
            return root;
        }

        private VisualElement MakeContextActionsButtons()
        {
            VisualElement buttonsRoot = new();
            var contextMenuActions = target
                   .GetType()
                   .GetMethods()
                   .Select(methodInfo => (methodInfo, contextMenu: methodInfo.GetCustomAttribute<ContextMenu>()) )
                   .Where((input, _) => input.contextMenu is { });

            foreach (var (methodInfo, contextMenu) in contextMenuActions)
            {
                void ClickEvent()
                {
                    BeforeContextActionCall(contextMenu.menuItem);
                    methodInfo.Invoke(target, Array.Empty<object>());
                    AfterContextActionCall();
                }

                var button = new Button(ClickEvent)
                {
                        text = contextMenu.menuItem
                };
                buttonsRoot.Add(button);
            }

            return buttonsRoot;
        }

        protected virtual void BeforeContextActionCall(string action)
        {
            Undo.RecordObject(target, action);
        }

        protected virtual void AfterContextActionCall()
        {
            serializedObject.Update();
        }

        /// <summary>
        /// Override this method to map particular properties to custom UIElements controls
        /// without using PropertyDrawers. Common approach is to use switch statement/expression on property name.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>return null to skip drawing property</returns>
        protected virtual VisualElement CreatePropertyElement(SerializedProperty property)
        {
            return new PropertyField(property){ name = property.name };
        }
    }
}