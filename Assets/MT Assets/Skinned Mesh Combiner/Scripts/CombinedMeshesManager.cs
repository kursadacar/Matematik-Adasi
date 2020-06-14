#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.SkinnedMeshCombiner
{
    /*
     *  This class is responsible for the functioning of the "Combined Meshes Manager" component, and all its functions.
     */
    /*
     * The Skinned Mesh Combiner was developed by Marcos Tomaz in 2019.
     * Need help? Contact me (mtassets@windsoft.xyz)
     */

    [ExecuteInEditMode]
    [AddComponentMenu("")] //Hide this script in component menu.
    public class CombinedMeshesManager : MonoBehaviour
    {
        //Enums of script
        public enum MergeMethod
        {
            OneMeshPerMaterial,
            AllInOne,
            JustMaterialColors,
            OnlyAnima2dMeshes
        }

        //Variables of this management
        [HideInInspector]
        public GameObject rootGameObject;
        [HideInInspector]
        public MergeMethod mergeMethodUsed;
        [HideInInspector]
        public GameObject resultOfConversionToStaticMesh;
        [HideInInspector]
        public SkinnedMeshRenderer[] sourceMeshes;

#if UNITY_EDITOR
        //Public variables of Interface
        private bool gizmosOfThisComponentIsDisabled = false;

        //The UI of this component
        #region INTERFACE_CODE
        [UnityEditor.CustomEditor(typeof(CombinedMeshesManager))]
        public class CustomInspector : UnityEditor.Editor
        {
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
                CombinedMeshesManager script = (CombinedMeshesManager)target;
                script.gizmosOfThisComponentIsDisabled = DisableGizmosInSceneView("CombinedMeshesManager", script.gizmosOfThisComponentIsDisabled);

                //Description
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("This component manages the combined mesh, generated by the Skinned Mesh Combiner. If you want to undo the merge, go to the Parent Object of this merge.", MessageType.None);
                GUILayout.Space(10);

                //Anima2D management
                #region ANIMA2D_MANAGEMENT_INTERFACE
#if MTAssets_Anima2D_Available
                if (script.mergeMethodUsed == MergeMethod.OnlyAnima2dMeshes)
                {
                    //Settings for "Anima2D"
                    EditorGUILayout.LabelField("Anima2D Merge", EditorStyles.boldLabel);
                    GUILayout.Space(10);

                    //Show atlas of sprite
                    EditorGUILayout.ObjectField(new GUIContent("Atlas Of This Model",
                        "The sprite to render in this model Anima2D."),
                        script.atlasForRenderInChar, typeof(Texture2D), true, GUILayout.Height(16));

                    GUILayout.Space(10);
                }
#endif
                #endregion

                //Management
                EditorGUILayout.LabelField("General Management", EditorStyles.boldLabel);
                GUILayout.Space(10);

                if (GUILayout.Button("Select Root GameObject", GUILayout.Height(25)))
                {
                    Selection.objects = new Object[] { script.rootGameObject };
                }
            }
        }
        #endregion
#endif

        #region ANIMA2D_MERGE_DATA_AND_RENDERIZATION
#if MTAssets_Anima2D_Available 
        //Renderization of combined Anima2D meshes
        [HideInInspector]
        public Texture2D atlasForRenderInChar;
        [HideInInspector]
        public SkinnedMeshRenderer cachedSkinnedRenderer;
        [HideInInspector]
        public MaterialPropertyBlock materialPropertyBlock;

        void OnWillRenderObject()
        {
            //If the merge method not is Only Anima2d Meshes, ignore this render
            if (mergeMethodUsed != MergeMethod.OnlyAnima2dMeshes)
                return;

            if (materialPropertyBlock != null)
            {
                if (atlasForRenderInChar != null)
                {
                    materialPropertyBlock.SetTexture("_MainTex", atlasForRenderInChar);
                }
                cachedSkinnedRenderer.SetPropertyBlock(materialPropertyBlock);
            }
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }
        }
#endif
        #endregion
    }
}