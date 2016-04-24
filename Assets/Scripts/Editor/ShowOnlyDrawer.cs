using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowOnly))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string valueStr;

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = prop.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                valueStr = prop.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                valueStr = prop.floatValue.ToString("0.00000");
                break;
            case SerializedPropertyType.String:
                if (string.IsNullOrEmpty(prop.stringValue))
                    valueStr = "(NULL)";
                else
                    valueStr = prop.stringValue;
                break;
            case SerializedPropertyType.ObjectReference:
                if (prop.objectReferenceValue)
                {
                    valueStr = prop.objectReferenceValue.ToString();
                }
                else
                {
                    valueStr = "(NULL)";
                }
                break;
            case SerializedPropertyType.Enum:
                try
                {
                    valueStr = prop.enumDisplayNames[prop.enumValueIndex];
                }
                catch (System.IndexOutOfRangeException)
                {
                    valueStr = "0x" + string.Format("{0,10:X}", prop.enumValueIndex.ToString());
                }
                break;
            case SerializedPropertyType.Vector2:
                valueStr = prop.vector2Value.ToString();
                break;
            case SerializedPropertyType.Vector3:
                valueStr = prop.vector3Value.ToString();
                break;
            case SerializedPropertyType.Vector4:
                valueStr = prop.vector4Value.ToString();
                break;
            default:
                valueStr = prop.objectReferenceValue.ToString();
                break;
        }

        EditorGUI.LabelField(position, label.text, valueStr);
    }
}