using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace TicTacToe
{
    /// <summary>
    /// Marker interface for custom generic classes
    /// </summary>
    public interface IAutoPropertyLayout { }
    
    public abstract class PropertyLayoutAttribute : PropertyAttribute
    {
        public abstract VisualElement GetLayoutRoot(VisualElement root);
        public override int GetHashCode() => GetType().GetHashCode();
    }

    public class HorizontalLayoutAttribute : PropertyLayoutAttribute, IEquatable<PropertyLayoutAttribute>
    {
        public override VisualElement GetLayoutRoot(VisualElement root)
        {
            var layoutRoot = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexWrap = Wrap.Wrap,
                    justifyContent = Justify.SpaceBetween
                }
            };
            root.Add(layoutRoot);
            return layoutRoot;
        }

        public override bool Equals(object other)
        {
            return other is HorizontalLayoutAttribute;
        }

        public bool Equals(PropertyLayoutAttribute other)
        {
            return other is HorizontalLayoutAttribute;
        }
    }

    public class VerticalLayoutAttribute : PropertyLayoutAttribute, IEquatable<PropertyLayoutAttribute>
    {
        public override VisualElement GetLayoutRoot(VisualElement root)
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(object other)
        {
            return other is VerticalLayoutAttribute;
        }

        public bool Equals(PropertyLayoutAttribute other)
        {
            return other is VerticalLayoutAttribute;
        }
    }
}