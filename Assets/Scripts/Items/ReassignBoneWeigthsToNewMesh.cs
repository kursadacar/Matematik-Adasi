using UnityEngine;


[ExecuteInEditMode]
public class ReassignBoneWeigthsToNewMesh : MonoBehaviour
{
    public Transform newArmature;
    public string rootBoneName = "Hips";

    private SkinnedMeshRenderer rend;
    private Transform[] targetChildren;

    public void Reassign()
    {
        if (newArmature == null)
        {
            Debug.Log("No new armature assigned");
            return;
        }

        if (newArmature.Find(rootBoneName) == null)
        {
            Debug.Log("Root bone not found");
            return;
        }

        targetChildren = newArmature.GetComponentsInChildren<Transform>();

        //Debug.Log("Reassingning bones");
        rend = gameObject.GetComponent<SkinnedMeshRenderer>();

        Transform[] bones = rend.bones;

        rend.rootBone = newArmature.Find(rootBoneName);

        for (int i = 0; i < bones.Length; i++)
            for (int a = 0; a < targetChildren.Length; a++)
                if (bones[i].name == targetChildren[a].name)
                {
                    bones[i] = targetChildren[a];
                    break;
                }

        rend.bones = bones;
    }

}