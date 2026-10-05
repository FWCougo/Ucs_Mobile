using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ============================================================
//  ATRIBUTO: mostra o campo só se a flag estiver marcada
//  no enum [Flags] indicado.
// ============================================================
[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ShowIfFlagAttribute : PropertyAttribute
{
    public readonly string enumFieldName;
    public readonly int flagValue;

    /// <param name="enumFieldName">Nome do campo enum [Flags] (use nameof).</param>
    /// <param name="flag">Valor(es) do enum que fazem o campo aparecer.</param>
    public ShowIfFlagAttribute(string enumFieldName, object flag)
    {
        this.enumFieldName = enumFieldName;
        this.flagValue = Convert.ToInt32(flag);
    }
}

#if UNITY_EDITOR
// ============================================================
//  DRAWER: decide se desenha o campo ou o esconde.
// ============================================================
[CustomPropertyDrawer(typeof(ShowIfFlagAttribute))]
public class ShowIfFlagDrawer : PropertyDrawer
{
    private bool ShouldShow(SerializedProperty property)
    {
        var attr = (ShowIfFlagAttribute)attribute;

        // Troca o último trecho do caminho pelo nome do enum
        // (funciona também para campos dentro de classes/listas serializadas).
        string path = property.propertyPath;
        int lastDot = path.LastIndexOf('.');
        string enumPath = lastDot >= 0
            ? path.Substring(0, lastDot + 1) + attr.enumFieldName
            : attr.enumFieldName;

        SerializedProperty enumProp = property.serializedObject.FindProperty(enumPath);
        if (enumProp == null)
        {
            Debug.LogWarning($"ShowIfFlag: campo '{attr.enumFieldName}' não encontrado.");
            return true;
        }

        return (enumProp.enumValueFlag & attr.flagValue) != 0;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return -EditorGUIUtility.standardVerticalSpacing; // remove o espaço vazio

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            EditorGUI.PropertyField(position, property, label, true);
    }
}
#endif