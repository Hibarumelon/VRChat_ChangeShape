#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class CameraReplacementSetup : EditorWindow
{
    Camera targetCamera;
    Shader replacement;
    string replacementTag = "RenderType";
    int avatarLayer = 0; // set to the Avatar layer index you want

    [MenuItem("Tools/Replacement Camera Setup")]
    static void OpenWindow()
    {
        GetWindow<CameraReplacementSetup>("Replacement Camera Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("Setup Replacement Camera for Avatar Ballify", EditorStyles.boldLabel);

        targetCamera = (Camera)EditorGUILayout.ObjectField("Target Camera", targetCamera, typeof(Camera), true);
        replacement = (Shader)EditorGUILayout.ObjectField("Replacement Shader", replacement, typeof(Shader), false);
        avatarLayer = EditorGUILayout.LayerField("Avatar Layer", avatarLayer);
        replacementTag = EditorGUILayout.TextField("Replacement Tag", replacementTag);

        if (GUILayout.Button("Configure Camera"))
        {
            if (targetCamera == null || replacement == null)
            {
                EditorUtility.DisplayDialog("Error", "Camera and Replacement Shader must be assigned.", "OK");
                return;
            }

            Undo.RecordObject(targetCamera, "Configure Replacement Camera");

            // set mask to only avatar layer
            targetCamera.cullingMask = 1 << avatarLayer;
            targetCamera.clearFlags = CameraClearFlags.Depth;
            targetCamera.depth = 1; // ensure higher than main (main usually 0)
            targetCamera.allowHDR = false;

            // set replacement - this call will be serialized in the scene
            targetCamera.SetReplacementShader(replacement, replacementTag);

            EditorUtility.SetDirty(targetCamera);
            EditorSceneManager.MarkSceneDirty(targetCamera.gameObject.scene);
            Debug.Log("Configured camera with replacement shader.");
        }
    }
}
#endif
