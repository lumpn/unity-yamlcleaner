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
        ShowAddedComponents(prefab);
        ShowRemovedComponents(prefab);

        //PrefabUtility.RevertObjectOverride(prefab, InteractionMode.AutomatedAction);
        //foreach (var comp in prefab.GetComponentsInChildren(typeof(Component)))
        //{
        //    //if (PrefabUtility.IsAddedComponentOverride(comp)) continue;
        //    //PrefabUtility.RevertObjectOverride(comp, InteractionMode.AutomatedAction);

        //    var mods = PrefabUtility.GetPropertyModifications(comp);
        //    if (mods != null)
        //    {
        //        foreach (var mod in mods)
        //        {
        //            Debug.LogFormat(mod.target, "target {0}, path {1}, value {2}", mod.target, mod.propertyPath, mod.value);
        //        }
        //    }
        //}
        //EditorUtility.SetDirty(prefab);


        //var builtInModifications = modifications.Where(p => p.propertyPath.StartsWith("m_", System.StringComparison.OrdinalIgnoreCase)).ToArray();
        //PrefabUtility.SetPropertyModifications(prefab, builtInModifications);
        //EditorUtility.SetDirty(prefab);
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

    private static void ShowRemovedComponents(GameObject prefab)
    {
        var comps = PrefabUtility.GetRemovedComponents(prefab);
        if (comps != null)
        {
            foreach (var comp in comps)
            {
                var go = comp.containingInstanceGameObject;
                Debug.LogFormat(go, "removed {0}", comp.assetComponent);
            }
        }
    }

    private static void ShowAddedComponents(GameObject prefab)
    {
        var comps = PrefabUtility.GetAddedComponents(prefab);
        if (comps != null)
        {
            foreach (var comp in comps)
            {
                var go = comp.instanceComponent.gameObject;
                Debug.LogFormat(go, "added {0}", comp.instanceComponent);
            }
        }
    }
}
