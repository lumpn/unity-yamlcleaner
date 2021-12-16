using UnityEditor;
using UnityEngine;

public sealed class YamlCleaner
{
    [MenuItem("Assets/Debug YAML")]
    public static void RunDebug()
    {
        var guids = Selection.assetGUIDs;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            var obj = new SerializedObject(asset);
            CleanPrefab(obj);
        }
    }

    [MenuItem("Assets/Debug YAML (objects)")]
    public static void RunDebug2()
    {
        var objects = Selection.objects;
        var obj = new SerializedObject(objects);
        Clean(obj);
    }

    public static void CleanPrefab(SerializedObject gameObject)
    {
        var componentsProperty = gameObject.FindProperty("m_Component");
        for (int i = 0; i < componentsProperty.arraySize; i++)
        {
            var elementProperty = componentsProperty.GetArrayElementAtIndex(i);
            var componentProperty = elementProperty.FindPropertyRelative("component");
            var component = componentProperty.objectReferenceValue;
            var componentObj = new SerializedObject(component);
            Clean(componentObj);
        }
    }

    public static void Clean(SerializedObject obj)
    {
        var property = obj.GetIterator();
        while (property.Next(true))
        {
            Debug.LogFormat("name {0}, display {1}, path {2}, type {3}, ptype {4}", property.name, property.displayName, property.propertyPath, property.type, property.propertyType);
        }
    }
}
