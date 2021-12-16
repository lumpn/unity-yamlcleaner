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
        var prefab = Selection.activeGameObject;
        ShowModifications(prefab);
    }

    [MenuItem("Assets/Remove obsolete modifications")]
    [MenuItem("GameObject/Remove obsolete modifications")]
    public static void RemoveObsoleteModifications()
    {
        var prefab = Selection.activeGameObject;
        RemoveObsoleteModifications(prefab);
    }

    private static void ShowModifications(GameObject prefab)
    {
        var mods = PrefabUtility.GetPropertyModifications(prefab);
        if (mods != null)
        {
            foreach (var mod in mods)
            {
                var so = new SerializedObject(mod.target);
                var prop = so.FindProperty(mod.propertyPath);
                var exists = (prop != null);

                Debug.LogFormat(mod.target, "target {0}, path {1}, value {2}, exists {3}", mod.target, mod.propertyPath, mod.value, exists);
            }
        }
    }

    private static void RemoveObsoleteModifications(GameObject prefab)
    {
        var modifications = PrefabUtility.GetPropertyModifications(prefab);
        if (modifications != null)
        {
            var validModifications = modifications.Where(IsValidModification).ToArray();
            if (validModifications.Length < modifications.Length)
            {
                PrefabUtility.SetPropertyModifications(prefab, validModifications);
                EditorUtility.SetDirty(prefab);

                var obsoleteModifications = modifications.Except(validModifications);
                foreach (var mod in obsoleteModifications)
                {
                    Debug.LogFormat(mod.target, "Removed obsolete modification of target '{0}', path '{1}', value '{2}'",
                                    mod.target, mod.propertyPath, mod.value);
                }
            }
        }
    }

    private static bool IsValidModification(PropertyModification modification)
    {
        var obj = new SerializedObject(modification.target);
        var property = obj.FindProperty(modification.propertyPath);
        return (property != null);
    }
}
