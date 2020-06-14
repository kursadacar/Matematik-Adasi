using UnityEngine;
using UnityEditor;

class DatabaseRefresher : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var path in importedAssets)
        {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset != null)
            {
                var comp = asset.GetComponent<Item>();
                if (comp != null)
                {
                    foreach (var dbGuid in AssetDatabase.FindAssets("t:ItemDatabase"))
                    {
                        var dbPath = AssetDatabase.GUIDToAssetPath(dbGuid);
                        var database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(dbPath);

                        //Refresh database
                        database.items.Clear();
                        var guids = AssetDatabase.FindAssets("t:GameObject");
                        foreach (var guid in guids)
                        {
                            var itemPath = AssetDatabase.GUIDToAssetPath(guid);
                            var go = AssetDatabase.LoadAssetAtPath<GameObject>(itemPath);
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
                    //Refreshed all item databases, no need to repeat
                    return;
                }
            }
        }

        foreach (var path in deletedAssets)
        {
            foreach (var dbGuid in AssetDatabase.FindAssets("t:ItemDatabase"))
            {
                var dbPath = AssetDatabase.GUIDToAssetPath(dbGuid);
                var database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(dbPath);

                //Refresh database
                database.items.Clear();
                var guids = AssetDatabase.FindAssets("t:GameObject");
                foreach (var guid in guids)
                {
                    var itemPath = AssetDatabase.GUIDToAssetPath(guid);
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(itemPath);
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

    }
}