using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TicTacToe
{
    [CustomPropertyDrawer(typeof(FunctionButtonAttribute))]
    public class FunctionButtonDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = CommonUIElements.RowContainer;
            root.Add(new PropertyField(property){style = { flexGrow = 1}});
            if (attribute is not FunctionButtonAttribute functionButtonAttribute)
            {
                return root;
            }

            var methodName = functionButtonAttribute.MethodName;
            var type = fieldInfo.DeclaringType;
            if (type is null)
            {
                Debug.LogException(new Exception($"Can't retrieve declaring type for {property.displayName} of {property.serializedObject.targetObject}"));
                return root;
            }
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo targetMethod = null;
            var expectedParamsCount = functionButtonAttribute.IsListProperty ? 1 : 0; 
            foreach (var method in methods)
            {
                if (method.Name != methodName)
                    continue;
                var parameters = method.GetParameters();
                var match = parameters.Length == expectedParamsCount;
                if (match && functionButtonAttribute.IsListProperty)
                    match &= parameters[0].ParameterType == typeof(int);
                if (!match) continue;
                targetMethod = method;
                break;
            }
            if (targetMethod is null)
            {
                Debug.LogError($"Can't find method '{methodName}' on class {type.Name}");
                return root;
            }

            var methodParameters = targetMethod.GetParameters();
            var methodParamsCount = methodParameters.Length;
            object parameterIndex = null;
            if (functionButtonAttribute.IsListProperty)
            {
                if (methodParamsCount != 1 && methodParameters[0].ParameterType != typeof(int))
                {
                    Debug.LogError($"{methodName} parameters mismatch. Should be one parameter of type Int32");
                    return root;
                }
                var path = property.propertyPath;
                var indexString = path.Remove(0, path.LastIndexOf('[') + 1);
                indexString = indexString.Remove(indexString.LastIndexOf(']'));
                if (Int32.TryParse(indexString, out var index))
                    parameterIndex = index;
            }
            else if (methodParamsCount != 0)
            {
                Debug.LogError($"{methodName} has more than zero arguments!");
                return root;
            }

            var methodTarget = property.GetPropertyOwnerObject();

            void OnFunctionButtonClicked()
            {
                if (functionButtonAttribute.UpdateSerializedObject)
                {
                    Undo.RecordObject(property.serializedObject.targetObject, functionButtonAttribute.ButtonText);
                }

                targetMethod.Invoke(methodTarget, functionButtonAttribute.IsListProperty ? new[] { parameterIndex } : null);
                
                if (functionButtonAttribute.UpdateSerializedObject)
                {
                    property.serializedObject.Update();
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }

            var functionButton = new Button(OnFunctionButtonClicked)
            {
                text = functionButtonAttribute.ButtonText ?? ObjectNames.NicifyVariableName(functionButtonAttribute.MethodName),
                tooltip = functionButtonAttribute.Tooltip
            };
            root.Add(functionButton);
            return root;
        }
    }
}