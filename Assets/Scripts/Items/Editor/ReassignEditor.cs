using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReassignBoneWeigthsToNewMesh))]
public class ReassignEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var reassigner = (ReassignBoneWeigthsToNewMesh)target;

        if (GUILayout.Button("Reassign!"))
            reassigner.Reassign();
        DrawDefaultInspector();
    }
}