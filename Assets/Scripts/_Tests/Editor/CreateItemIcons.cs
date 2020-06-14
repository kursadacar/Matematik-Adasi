using UnityEditor;
using UnityEngine;

public class CreateItemIcons : EditorWindow
{
    string mainPath = "Assets/Model_Texture_Materials/Textures/Icons/Items/";
    [MenuItem("Window/Item Icon Creator")]
    static void Init()
    {
        CreateItemIcons window = (CreateItemIcons)GetWindow(typeof(CreateItemIcons));
        window.Show();
    }

    void OnGUI()
    {
        AssetPreview.SetPreviewTextureCacheSize(256);
        if (GUILayout.Button("Recreate Icon For Every Item"))
        {
            var snapshotCamera = SnapshotCamera.MakeSnapshotCamera(5, "Snapshot Camera");
            var guids = AssetDatabase.FindAssets("t:GameObject");
            foreach(var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var item = obj.GetComponent<Item>();
                if(item != null && item.type != Item.ItemType.Rod)
                {
                    var texture = snapshotCamera.TakePrefabSnapshot(obj, 512, 512);

                    string savePath = mainPath;

                    switch (item.type)
                    {
                        case Item.ItemType.Fish:
                            {
                                savePath += "Fishes/";
                            }
                            break;
                        case Item.ItemType.Hat:
                            {
                                savePath += "Hats/";
                            }
                            break;
                        case Item.ItemType.Shoe:
                            {
                                savePath += "Shoes/";
                            }
                            break;
                        case Item.ItemType.Tshirt:
                            {
                                savePath += "Tshirts/";
                            }
                            break;
                        case Item.ItemType.Pants:
                            {
                                savePath += "Pants/";
                            }
                            break;
                    }

                    AssetDatabase.CreateAsset(texture, savePath + item.name + "_iconTexture.asset");
                    var loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath + item.name + "_iconTexture.asset");
                    var icon = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), new Vector2(0.5f, 0.5f));
                    AssetDatabase.CreateAsset(icon, savePath + item.name + "_icon.asset");
                    var loadedIcon = AssetDatabase.LoadAssetAtPath<Sprite>(savePath + item.name + "_icon.asset");

                    item.SetIcon(loadedIcon);
                }
                EditorUtility.SetDirty(obj);
            }
            AssetDatabase.Refresh();
            DestroyImmediate(snapshotCamera);
        }
    }
}

