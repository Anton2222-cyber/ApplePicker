using UnityEngine;
using UnityEditor;

public class PrefabReplacer : EditorWindow
{
    public GameObject oldPrefab;
    public GameObject newPrefab;

    [MenuItem("Tools/Prefab Replacer")]
    public static void ShowWindow()
    {
        GetWindow<PrefabReplacer>("Prefab Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Prefabs", EditorStyles.boldLabel);
        oldPrefab = (GameObject)EditorGUILayout.ObjectField("Old Prefab", oldPrefab, typeof(GameObject), true);
        newPrefab = (GameObject)EditorGUILayout.ObjectField("New Prefab", newPrefab, typeof(GameObject), true);

        if (GUILayout.Button("Replace"))
        {
            ReplacePrefabs();
        }
    }

    private void ReplacePrefabs()
    {
        if (oldPrefab == null || newPrefab == null)
        {
            Debug.LogWarning("Old Prefab or New Prefab is not set.");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        int count = 0;

        foreach (GameObject obj in allObjects)
        {
            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected && PrefabUtility.GetCorrespondingObjectFromSource(obj) == oldPrefab)
            {
                Debug.Log($"Found instance of old prefab: {obj.name}");

                // Create new prefab instance
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab, obj.transform.parent);
                newObj.transform.position = obj.transform.position;
                newObj.transform.rotation = obj.transform.rotation;
                newObj.transform.localScale = obj.transform.localScale;

                // Copy all components except Transform
                foreach (Component component in obj.GetComponents<Component>())
                {
                    if (!(component is Transform))
                    {
                        Component newComponent = newObj.AddComponent(component.GetType());
                        EditorUtility.CopySerialized(component, newComponent);
                    }
                }

                // Copy children
                foreach (Transform child in obj.transform)
                {
                    Transform newChild = Instantiate(child, newObj.transform);
                    newChild.name = child.name;
                }

                // Destroy old prefab instance
                DestroyImmediate(obj);
                count++;
            }
        }

        Debug.Log($"{count} prefab instances replaced.");
    }
}
