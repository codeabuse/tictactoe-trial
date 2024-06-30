using System;
using UnityEngine;

namespace TicTacToe
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RestrictWithInterfaceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }
        
        public RestrictWithInterfaceAttribute(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"{interfaceType.Name} is not an interface!");
            InterfaceType = interfaceType;
        }
    }
}