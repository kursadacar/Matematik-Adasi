#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.SkinnedMeshCombiner
{
    /*
     *  This class is responsible for the functioning of the "Skinned Mesh Bones Manager" component, and all its functions.
     */
    /*
     * The Skinned Mesh Combiner was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
     */

    public class SkinnedMeshBonesManager : MonoBehaviour
    {
        //Public variables of script
        [HideInInspector]
        public SkinnedMeshRenderer anotherBonesHierarchyCurrentInUse = null;

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;

        //Classes of this script, only disponible in Editor
        private class VerticeData
        {
            //This class store all data about a vertice influenced by a bone
            public BoneInfo influencerBone;
            public float weightOfInfluencer;
            public int indexOfThisVerticeInMesh;

            public VerticeData(BoneInfo boneInfo, float weightOfInfluencer, int indexOfThisVerticeInMesh)
            {
                this.influencerBone = boneInfo;
                this.weightOfInfluencer = weightOfInfluencer;
                this.indexOfThisVerticeInMesh = indexOfThisVerticeInMesh;
            }
        }
        private class BoneInfo
        {
            //This class store all data about a bone
            public GameObject gameObject;
            public Transform transform;
            public string name;
            public string transformPath;
            public int hierarchyIndex;
            public List<VerticeData> verticesOfThis = new List<VerticeData>();
        }

        //Public variables of editor 
        [HideInInspector]
        private BoneInfo[] bones = new BoneInfo[0];
        [HideInInspector]
        private BoneInfo boneInfoToShowVertices = null;
        [HideInInspector]
        public string currentBoneNameRendering = "";
        [HideInInspector]
        public float gizmosSizeInterface = 0.01f;
        [HideInInspector]
        public bool renderGizmoOfBone = true;
        [HideInInspector]
        public bool renderLabelOfBone = true;
        [HideInInspector]
        public bool pingBoneOnShowVertices = false;
        [HideInInspector]
        public SkinnedMeshRenderer meshRendererBonesToAnimateThis = null;
        [HideInInspector]
        public bool useRootBoneToo = true;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(SkinnedMeshBonesManager))]
        public class CustomInspector : UnityEditor.Editor
        {
            //Private variables of editor
            private Vector2 bonesListScroll = Vector2.zero;
            private int framesUpdate = 10;
            private Vector3 currentPostionOfVerticesText = Vector3.zero;
            private string currentTextOfVerticesText = "";
            private int currentSelectedVertice = -1;

            public bool DisableGizmosInSceneView(string scriptClassNameToDisable, bool isGizmosDisabled)
            {
                /*
                *  This method disables Gizmos in scene view, for this component
                */

                if (isGizmosDisabled == true)
                    return true;

                //Try to disable
                try
                {
                    //Get all data of Unity Gizmos manager window
                    var Annotation = System.Type.GetType("UnityEditor.Annotation, UnityEditor");
                    var ClassId = Annotation.GetField("classID");
                    var ScriptClass = Annotation.GetField("scriptClass");
                    var Flags = Annotation.GetField("flags");
                    var IconEnabled = Annotation.GetField("iconEnabled");

                    System.Type AnnotationUtility = System.Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
                    var GetAnnotations = AnnotationUtility.GetMethod("GetAnnotations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    var SetIconEnabled = AnnotationUtility.GetMethod("SetIconEnabled", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                    //Scann all Gizmos of Unity, of this project
                    System.Array annotations = (System.Array)GetAnnotations.Invoke(null, null);
                    foreach (var a in annotations)
                    {
                        int classId = (int)ClassId.GetValue(a);
                        string scriptClass = (string)ScriptClass.GetValue(a);
                        int flags = (int)Flags.GetValue(a);
                        int iconEnabled = (int)IconEnabled.GetValue(a);

                        // this is done to ignore any built in types
                        if (string.IsNullOrEmpty(scriptClass))
                        {
                            continue;
                        }

                        const int HasIcon = 1;
                        bool hasIconFlag = (flags & HasIcon) == HasIcon;

                        //If the current gizmo is of the class desired, disable the gizmo in scene
                        if (scriptClass == scriptClassNameToDisable)
                        {
                            if (hasIconFlag && (iconEnabled != 0))
                            {
                                /*UnityEngine.Debug.LogWarning(string.Format("Script:'{0}' is not ment to show its icon in the scene view and will auto hide now. " +
                                    "Icon auto hide is checked on script recompile, if you'd like to change this please remove it from the config", scriptClass));*/
                                SetIconEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
                            }
                        }
                    }

                    return true;
                }
                //Catch any error
                catch (System.Exception exception)
                {
                    string exceptionOcurred = "";
                    exceptionOcurred = exception.Message;
                    if (exceptionOcurred != null)
                        exceptionOcurred = "";
                    return false;
                }
            }

            public Rect GetInspectorWindowSize()
            {
                //Returns the current size of inspector window
                return EditorGUILayout.GetControlRect(true, 0f);
            }

            public override void OnInspectorGUI()
            {
                //Start the undo event support, draw default inspector and monitor of changes
                SkinnedMeshBonesManager script = (SkinnedMeshBonesManager)target;
                script.gizmosOfThisComponentIsDisabled = DisableGizmosInSceneView("SkinnedMeshBonesManager", script.gizmosOfThisComponentIsDisabled);

                //Try to load needed assets
                Texture selectedBone = (Texture)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Skinned Mesh Combiner/Editor/Images/SelectedBone.png", typeof(Texture));
                Texture unselectedBone = (Texture)AssetDatabase.LoadAssetAtPath("Assets/MT Assets/Skinned Mesh Combiner/Editor/Images/UnselectedBone.png", typeof(Texture));
                //If fails on load needed assets, locks ui
                if (selectedBone == null || unselectedBone == null)
                {
                    EditorGUILayout.HelpBox("Unable to load required files. Please reinstall Skinned Mesh Combiner to correct this problem.", MessageType.Error);
                    return;
                }

                //Description
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("Remember to read the Skinned Mesh Combiner documentation to understand how to use it.\nGet support at: mtassets@windsoft.xyz", MessageType.None);
                GUILayout.Space(10);

                //If not exists a skinned mesh renderer or null mesh, stop this interface
                SkinnedMeshRenderer meshRenderer = script.GetComponent<SkinnedMeshRenderer>();
                if (meshRenderer == null)
                {
                    EditorGUILayout.HelpBox("A \"Skinned Mesh Renderer\" component could not be found in this GameObject. Please insert this manager into a Skinned Renderer.", MessageType.Error);
                    return;
                }
                if (meshRenderer != null && meshRenderer.sharedMesh == null)
                {
                    EditorGUILayout.HelpBox("It was not possible to find a mesh associated with the Skinned Mesh Renderer component of this GameObject. Please associate a valid mesh with this Skinned Mesh Renderer, so that you can manage the Bones.", MessageType.Error);
                    return;
                }

                //Verify if is playing
                if (Application.isPlaying == true)
                {
                    EditorGUILayout.HelpBox("The bone management interface is not available while the application is running, only the API for this component works during execution.", MessageType.Info);
                    return;
                }

                //Bones list
                EditorGUILayout.LabelField("All Bones Of This Mesh (" + meshRenderer.sharedMesh.name + ")", EditorStyles.boldLabel);
                GUILayout.Space(10);

                //Get all bones after 10 updates of frames
                if (framesUpdate >= 10)
                    script.bones = script.GetAllBonesAndDataInList(); framesUpdate = 0;
                framesUpdate += 1;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("All Bones Found In This Skinned Mesh Renderer", GUILayout.Width(280));
                GUILayout.Space(GetInspectorWindowSize().x - 280);
                EditorGUILayout.LabelField("Size", GUILayout.Width(30));
                EditorGUILayout.IntField(script.bones.Length, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                bonesListScroll = EditorGUILayout.BeginScrollView(bonesListScroll, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(GetInspectorWindowSize().x), GUILayout.Height(250));
                if (script.bones != null)
                {
                    //If is using another bones hierarchy
                    if (script.anotherBonesHierarchyCurrentInUse != null)
                    {
                        EditorGUILayout.HelpBox("This bone hierarchy belongs to the Skinned Mesh Renderer \"" + script.anotherBonesHierarchyCurrentInUse.transform.name + "\", however, it is the bone hierarchy of \"" + script.anotherBonesHierarchyCurrentInUse.transform.name + "\" that is animating this mesh. You can still see which bones control which vertices and so on.", MessageType.Warning);
                    }

                    //Create style of icon
                    GUIStyle estiloIcone = new GUIStyle();
                    estiloIcone.border = new RectOffset(0, 0, 0, 0);
                    estiloIcone.margin = new RectOffset(4, 0, 6, 0);

                    foreach (BoneInfo bone in script.bones)
                    {
                        //List each bone
                        if (bone.hierarchyIndex > 0)
                            GUILayout.Space(8);
                        EditorGUILayout.BeginHorizontal();
                        if (script.boneInfoToShowVertices == null || bone.hierarchyIndex != script.boneInfoToShowVertices.hierarchyIndex)
                            GUILayout.Box(unselectedBone, estiloIcone, GUILayout.Width(24), GUILayout.Height(24));
                        if (script.boneInfoToShowVertices != null && bone.hierarchyIndex == script.boneInfoToShowVertices.hierarchyIndex)
                            GUILayout.Box(selectedBone, estiloIcone, GUILayout.Width(24), GUILayout.Height(24));
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("(" + bone.hierarchyIndex.ToString() + ") " + bone.name, EditorStyles.boldLabel);
                        GUILayout.Space(-3);
                        EditorGUILayout.LabelField("Influencing " + bone.verticesOfThis.Count + " vertices in this mesh.");
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(6);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Path", GUILayout.Height(20)))
                        {
                            if (bone.gameObject != null)
                                EditorUtility.DisplayDialog("Ping to \"" + bone.name + "\" bone of this mesh", "The path of GameObject/Transform of this bone is...\n\n" + bone.transformPath, "Ok");
                            if (bone.gameObject == null)
                                EditorUtility.DisplayDialog("Bone Error", "This bone transform, not found in this scene.", "Ok");
                            EditorGUIUtility.PingObject(bone.gameObject);
                        }
                        if (script.boneInfoToShowVertices == null || bone.hierarchyIndex != script.boneInfoToShowVertices.hierarchyIndex)
                            if (GUILayout.Button("Vertices", GUILayout.Height(20)))
                            {
                                //Chjange the bone info to view vertices, and reset editor data
                                script.boneInfoToShowVertices = bone;
                                script.currentBoneNameRendering = "Showing vertices influenceds by bone\n\"" + bone.name + "\"\nVertices Influenceds: " + bone.verticesOfThis.Count;
                                currentSelectedVertice = -1;
                                currentPostionOfVerticesText = Vector3.zero;
                                currentTextOfVerticesText = "";
                                if (script.pingBoneOnShowVertices)
                                    EditorGUIUtility.PingObject(bone.gameObject);
                            }
                        if (script.boneInfoToShowVertices != null && bone.hierarchyIndex == script.boneInfoToShowVertices.hierarchyIndex)
                            if (GUILayout.Button("---------", GUILayout.Height(20)))
                            {
                                script.boneInfoToShowVertices = null;
                                script.currentBoneNameRendering = "";
                                currentSelectedVertice = -1;
                                currentPostionOfVerticesText = Vector3.zero;
                                currentTextOfVerticesText = "";
                            }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(2);
                    }
                }
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();

                EditorGUILayout.HelpBox("" +
                   "In the list above, you can see all the bones that are linked to the mesh of this Skinned Mesh Renderer. You can also see the bone hierarchy that this mesh is using. All bones listed below are linked to this mesh and may or may not deform vertices of this mesh.", MessageType.Info);

                script.gizmosSizeInterface = EditorGUILayout.Slider(new GUIContent("Gizmos Size In Interface",
                         "The size that the Gizmos will be rendered in interface of this component."),
                         script.gizmosSizeInterface, 0.001f, 0.1f);

                script.renderGizmoOfBone = (bool)EditorGUILayout.Toggle(new GUIContent("Render Gizmo Of Bone",
                        "Render gizmo of bone selected to show vertices?"),
                        script.renderGizmoOfBone);

                script.renderLabelOfBone = (bool)EditorGUILayout.Toggle(new GUIContent("Render Label Of Bone",
                       "Render label of bone selected to show vertices?"),
                       script.renderLabelOfBone);

                script.pingBoneOnShowVertices = (bool)EditorGUILayout.Toggle(new GUIContent("Ping Bone On Show Vert.",
                      "Ping/Highlight bone transform in scene, everytime that you show vertices of the bone?"),
                      script.pingBoneOnShowVertices);

                //Bones mangement
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Use Another Bone Hierarchy To Animate This", EditorStyles.boldLabel);
                GUILayout.Space(10);

                //If not forneced the skinned mesh renderer
                if (script.meshRendererBonesToAnimateThis == null)
                    EditorGUILayout.HelpBox("Provide a Skinned Mesh Renderer so that your bones hierarchy is used instead.", MessageType.Info);

                //If forneced the skinned mesh renderer
                if (script.meshRendererBonesToAnimateThis != null)
                {
                    //Prepare the message
                    string errorMessage = "It is not possible for this Skinned Mesh Renderer to use the Skinned Mesh Renderer \"" + script.meshRendererBonesToAnimateThis.gameObject.name + "\" bone hierarchy, as the two hierarchies are not identical. Both Skinned Mesh Renderers must have an identical bone hierarchy to make it possible for this mesh to be animated by the bones of the desired Skinned Mesh Renderer.";

                    //Validate the mesh renderer
                    if (script.isValidTargetSkinnedMeshRendererBonesHierarchy(script.meshRendererBonesToAnimateThis, false) == false)
                        EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                }

                script.meshRendererBonesToAnimateThis = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(new GUIContent("Bones Hierarchy To Use",
                    "This custom material will have its properties copied and will be associated with the merged mesh."),
                    script.meshRendererBonesToAnimateThis, typeof(SkinnedMeshRenderer), true, GUILayout.Height(16));

                //If forneced the skinned mesh renderer
                if (script.meshRendererBonesToAnimateThis != null)
                {
                    if (script.isValidTargetSkinnedMeshRendererBonesHierarchy(script.meshRendererBonesToAnimateThis, false) == true)
                    {
                        script.useRootBoneToo = (bool)EditorGUILayout.Toggle(new GUIContent("Use Root Bone Too",
                          "Use the same root bone of the Skinned Mesh Renderer too?"),
                          script.useRootBoneToo);

                        GUILayout.Space(10);

                        if (GUILayout.Button("Use Bones Hierarchy From That Skinned Mesh Renderer", GUILayout.Height(40)))
                        {
                            if (EditorUtility.DisplayDialog("Continue?", "You will no longer be able to undo this change. To obtain the Skinned Mesh Renderer and all its original information, it will be necessary to re-add this mesh to your scene again."
                                + "\n\nAfter performing this action, the mesh of this Skinned Mesh Renderer will be animated using the bones of the Skinned Mesh Renderer you provided.", "Continue", "Cancel") == true)
                                script.UseAnotherBoneHierarchyForAnimateThis(script.meshRendererBonesToAnimateThis, script.useRootBoneToo);
                        }
                    }
                }
            }

            protected virtual void OnSceneGUI()
            {
                SkinnedMeshBonesManager script = (SkinnedMeshBonesManager)target;

                //Get this mesh renderer
                SkinnedMeshRenderer meshRenderer = script.GetComponent<SkinnedMeshRenderer>();

                //If not have components to worl
                if (meshRenderer == null || meshRenderer.sharedMesh == null)
                    return;

                //Render only if have a boneinfo to render
                if (meshRenderer != null && meshRenderer.sharedMesh != null && script.boneInfoToShowVertices == null)
                    return;

                EditorGUI.BeginChangeCheck();
                Undo.RecordObject(target, "Undo Event");

                //Set the base color of gizmos
                Handles.color = Color.green;

                //Render each vertice
                foreach (VerticeData vertice in script.boneInfoToShowVertices.verticesOfThis)
                {
                    //Color the gizmo according the weight
                    Handles.color = Color.Lerp(Color.green, Color.red, vertice.weightOfInfluencer);

                    //If is the selected vertice
                    if (vertice.indexOfThisVerticeInMesh == currentSelectedVertice)
                        Handles.color = Color.white;

                    //Draw current vertice
                    Vector3 currentVertice = script.transform.TransformPoint(meshRenderer.sharedMesh.vertices[vertice.indexOfThisVerticeInMesh]);
                    if (Handles.Button(currentVertice, Quaternion.identity, script.gizmosSizeInterface, script.gizmosSizeInterface, Handles.SphereHandleCap))
                    {
                        currentPostionOfVerticesText = currentVertice;
                        currentTextOfVerticesText = "Vertice Index: " + vertice.indexOfThisVerticeInMesh + "/" + meshRenderer.sharedMesh.vertices.Length + "\nInfluencer Bone: " + vertice.influencerBone.name + "\nWeight of Influence: " + vertice.weightOfInfluencer.ToString("F2");
                        currentSelectedVertice = vertice.indexOfThisVerticeInMesh;
                    }
                }

                //Prepare the text
                GUIStyle styleVerticeDetail = new GUIStyle();
                styleVerticeDetail.normal.textColor = Color.white;
                styleVerticeDetail.alignment = TextAnchor.MiddleCenter;
                styleVerticeDetail.fontStyle = FontStyle.Bold;
                styleVerticeDetail.contentOffset = new Vector2(-currentTextOfVerticesText.Substring(0, currentTextOfVerticesText.IndexOf("\n") + 1).Length * 1.8f, 30);

                //Draw the vertice text, if is desired
                if (currentPostionOfVerticesText != Vector3.zero)
                    Handles.Label(currentPostionOfVerticesText, currentTextOfVerticesText, styleVerticeDetail);

                //Render the bone, if is desired
                if (script.renderGizmoOfBone)
                {
                    Handles.color = Color.gray;
                    Handles.ArrowHandleCap(0, script.boneInfoToShowVertices.transform.position, Quaternion.identity * script.boneInfoToShowVertices.transform.rotation * Quaternion.AngleAxis(90, Vector3.left), script.gizmosSizeInterface * 18f, EventType.Repaint);
                }

                //Render the bone name, if is desired
                if (script.renderLabelOfBone)
                {
                    GUIStyle styleBoneName = new GUIStyle();
                    styleBoneName.normal.textColor = Color.white;
                    styleBoneName.alignment = TextAnchor.MiddleCenter;
                    styleBoneName.fontStyle = FontStyle.Bold;
                    styleBoneName.contentOffset = new Vector2(-script.currentBoneNameRendering.Substring(0, script.currentBoneNameRendering.IndexOf("\n") + 1).Length * 1.5f, 30);
                    if (string.IsNullOrEmpty(script.currentBoneNameRendering) == false)
                        Handles.Label(script.boneInfoToShowVertices.transform.position, script.currentBoneNameRendering, styleBoneName);
                }

                //Apply changes on script, case is not playing in editor
                if (GUI.changed == true && Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(script);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                }
                if (EditorGUI.EndChangeCheck() == true)
                {
                    //Apply the change, if moved the handle
                    //script.transform.position = teste;
                }
                Repaint();
            }
        }

        private string GetGameObjectPath(Transform transform)
        {
            //Return the full path of a GameObject
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }

        private BoneInfo[] GetAllBonesAndDataInList()
        {
            //Create list of bones
            List<BoneInfo> bonesList = new List<BoneInfo>();

            //Get the skinned mesh renderer
            SkinnedMeshRenderer meshRender = GetComponent<SkinnedMeshRenderer>();

            //Start the scan
            if (meshRender != null && meshRender.sharedMesh != null)
            {
                //Get all bones
                Transform[] allBonesTransform = meshRender.bones;

                //Create all boneinfo
                for (int i = 0; i < allBonesTransform.Length; i++)
                {
                    BoneInfo boneInfo = new BoneInfo();
                    boneInfo.transform = allBonesTransform[i];
                    boneInfo.name = allBonesTransform[i].name;
                    boneInfo.gameObject = allBonesTransform[i].transform.gameObject;
                    boneInfo.transformPath = GetGameObjectPath(allBonesTransform[i]);
                    boneInfo.hierarchyIndex = i;

                    bonesList.Add(boneInfo);
                }

                //Associate each vertice influenced by each bone to respective key
                for (int i = 0; i < meshRender.sharedMesh.vertexCount; i++)
                {
                    //Verify if exists a weight of a possible bone X influencing this vertice. Create a vertice data that stores and link this vertice inside your respective BoneInfo
                    if (meshRender.sharedMesh.boneWeights[i].weight0 > 0)
                    {
                        int boneIndexOfInfluencerBoneOfThisVertice = meshRender.sharedMesh.boneWeights[i].boneIndex0;
                        bonesList[boneIndexOfInfluencerBoneOfThisVertice].verticesOfThis.Add(new VerticeData(bonesList[boneIndexOfInfluencerBoneOfThisVertice], meshRender.sharedMesh.boneWeights[i].weight0, i));
                    }
                    if (meshRender.sharedMesh.boneWeights[i].weight1 > 0)
                    {
                        int boneIndexOfInfluencerBoneOfThisVertice = meshRender.sharedMesh.boneWeights[i].boneIndex1;
                        bonesList[boneIndexOfInfluencerBoneOfThisVertice].verticesOfThis.Add(new VerticeData(bonesList[boneIndexOfInfluencerBoneOfThisVertice], meshRender.sharedMesh.boneWeights[i].weight1, i));
                    }
                    if (meshRender.sharedMesh.boneWeights[i].weight2 > 0)
                    {
                        int boneIndexOfInfluencerBoneOfThisVertice = meshRender.sharedMesh.boneWeights[i].boneIndex2;
                        bonesList[boneIndexOfInfluencerBoneOfThisVertice].verticesOfThis.Add(new VerticeData(bonesList[boneIndexOfInfluencerBoneOfThisVertice], meshRender.sharedMesh.boneWeights[i].weight2, i));
                    }
                    if (meshRender.sharedMesh.boneWeights[i].weight3 > 0)
                    {
                        int boneIndexOfInfluencerBoneOfThisVertice = meshRender.sharedMesh.boneWeights[i].boneIndex3;
                        bonesList[boneIndexOfInfluencerBoneOfThisVertice].verticesOfThis.Add(new VerticeData(bonesList[boneIndexOfInfluencerBoneOfThisVertice], meshRender.sharedMesh.boneWeights[i].weight3, i));
                    }
                }
            }

            //Return the list
            return bonesList.ToArray();
        }
        #endregion
#endif

        //Private methods for this component Interface and API.

        private bool isValidTargetSkinnedMeshRendererBonesHierarchy(SkinnedMeshRenderer targetMeshRenderer, bool launchLogs)
        {
            //Prepare the value
            bool isValid = true;

            //Get this mesh renderer
            SkinnedMeshRenderer thisMeshRenderer = GetComponent<SkinnedMeshRenderer>();

            //Validate
            if (thisMeshRenderer == null)
                isValid = false;
            if (targetMeshRenderer == null)
                isValid = false;
            if (thisMeshRenderer.bones.Length != targetMeshRenderer.bones.Length)
                isValid = false;

            //Validate if all bones of target is equal of bones of this, by name
            /*if (isValid == true)
            {
                for (int i = 0; i < thisMeshRenderer.bones.Length; i++)
                {
                    if (thisMeshRenderer.bones[i].name != targetMeshRenderer.bones[i].name)
                        isValid = false;
                }
            }*/

            //Launch logs if is desired
            if (launchLogs == true)
            {
                string errorMessage = "It is not possible for this Skinned Mesh Renderer to use the Skinned Mesh Renderer \"" + targetMeshRenderer.gameObject.name + "\" bone hierarchy, as the two hierarchies are not identical. Both Skinned Mesh Renderers must have an identical bone hierarchy to make it possible for this mesh to be animated by the bones of the desired Skinned Mesh Renderer.";

#if !UNITY_EDITOR
                if (isValid == false)
                    Debug.Log(errorMessage);
#endif
#if UNITY_EDITOR
                if (isValid == false && Application.isPlaying == false)
                    EditorUtility.DisplayDialog("Error", errorMessage, "Ok");
                if (isValid == false && Application.isPlaying == true)
                    Debug.Log(errorMessage);
#endif
            }

            //Return
            return isValid;
        }

        //Public API methods

        public bool UseAnotherBoneHierarchyForAnimateThis(SkinnedMeshRenderer meshRendererBonesToUse, bool useRootBoneToo)
        {
            //First validate the target skinned mesh renderer
            if (isValidTargetSkinnedMeshRendererBonesHierarchy(meshRendererBonesToUse, true) == false)
                return false;

            //Set a another bone hierarchy to animate this
            SkinnedMeshRenderer thisMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            thisMeshRenderer.bones = meshRendererBonesToUse.bones;
            if (useRootBoneToo == true)
                thisMeshRenderer.rootBone = meshRendererBonesToUse.rootBone;

            //Move this gameobject to be parent of meshRendererToBonesUse
            this.gameObject.transform.parent = meshRendererBonesToUse.transform.parent;

            //Set the stats
            anotherBonesHierarchyCurrentInUse = meshRendererBonesToUse;

            //Return true for success
            return true;
        }
    }
}