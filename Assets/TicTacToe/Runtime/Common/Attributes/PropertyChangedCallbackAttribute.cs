using System;
using UnityEngine;

namespace TicTacToe
{
    public class PropertyChangedCallbackAttribute : PropertyAttribute
    {
        public string MethodName { get; }
        public Type ArgumentType { get; set; }
        public PropertyChangedCallbackAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}