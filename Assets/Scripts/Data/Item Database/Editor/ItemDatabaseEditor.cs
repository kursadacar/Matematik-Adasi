using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    ItemDatabase database;
    public override void OnInspectorGUI()
    {
        database = (ItemDatabase)target;

        GUILayout.Label("Editor Properties");

        if (GUILayout.Button("Refresh Database"))
        {
            Refresh();
        }

        GUILayout.Space(10f);
        GUILayout.Box(Texture2D.whiteTexture, GUILayout.Height(3f),GUILayout.ExpandWidth(true));
        GUILayout.Space(10f);

        DrawDefaultInspector();
    }

    private void Refresh()
    {
        database.items.Clear();
        var guids = AssetDatabase.FindAssets("t:GameObject");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var item = go.GetComponent<Item>();

            if (item == null)
                continue;

            if (database.type == Item.ItemType.None)
            {
                item.data.ID = database.items.Count;
                database.items.Add(item);
            }
            else
            {
                if (item.type == database.type)
                    database.items.Add(item);
            }
        }
    }
}
