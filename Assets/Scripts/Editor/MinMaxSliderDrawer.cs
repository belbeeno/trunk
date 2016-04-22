using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(MinMaxSlider))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 range = property.vector2Value;
            MinMaxSlider castedAttribute = attribute as MinMaxSlider;

            Rect labelRect = position;
            labelRect.width = position.width / 2.75f;
            Rect adjustedRect = position;
            adjustedRect.width = position.width - labelRect.width;
            adjustedRect.x += labelRect.width;

            EditorGUI.LabelField(labelRect, label);

            EditorGUI.BeginChangeCheck();
            {
                float min = range.x;
                float max = range.y;
                Rect leftField = adjustedRect;
                leftField.width = adjustedRect.width / 6f;
                Rect midField = adjustedRect;
                midField.width = 2f * adjustedRect.width / 3f - 4f;
                midField.x = midField.x + leftField.width + 6f;
                Rect rightField = adjustedRect;
                rightField.width = adjustedRect.width / 5f;
                rightField.x = rightField.x + midField.width + rightField.width + 4f;

                min = Mathf.Min(EditorGUI.FloatField(leftField, min), max);
                max = Mathf.Max(EditorGUI.FloatField(rightField, max), min);
                EditorGUI.MinMaxSlider(midField, ref min, ref max, castedAttribute.min, castedAttribute.max);

                range.x = min;
                range.y = max;
            }
            if (EditorGUI.EndChangeCheck())
            {
                property.vector2Value = range;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use MinMax attribute with Vector2!");
        }
    }
}
