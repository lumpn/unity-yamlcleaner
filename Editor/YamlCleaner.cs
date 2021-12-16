using UnityEditor;
using UnityEngine;
using System.Linq;

public sealed class YamlCleaner
{
    [MenuItem("Assets/Force reserialization (all files)")]
    public static void ForceReserialization()
    {
        AssetDatabase.ForceReserializeAssets();
    }

    [MenuItem("Assets/Show modifications")]
    [MenuItem("GameObject/Show modifications")]
    public static void ShowModifications()
    {
        var prefab = Selection.activeObject;
        var modifications = PrefabUtility.GetPropertyModifications(prefab);
        if (modifications == null)
        {
            Debug.Log("No modifications");
            return;
        }

        foreach (var modification in modifications)
        {
            Debug.LogFormat(modification.target, "path {0}, value {1}, target {2}", modification.propertyPath, modification.value, modification.target);
        }

        var builtInModifications = modifications.Where(p => p.propertyPath.StartsWith("m_", System.StringComparison.OrdinalIgnoreCase)).ToArray();
        PrefabUtility.SetPropertyModifications(prefab, builtInModifications);
        EditorUtility.SetDirty(prefab);
    }

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
        CleanPrefab(obj);
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
