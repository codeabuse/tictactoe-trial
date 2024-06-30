using System;
using UnityEngine;

namespace TicTacToe
{
    public class InstantiateAttribute : PropertyAttribute
    {
        public Type BaseType { get; }

        public InstantiateAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}