using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AutoBallifyOnJoin : UdonSharpBehaviour
{
    public Camera replacementCamera;
    public RenderTexture replacementRT;
    public float boxifyStrength = 1f;
    public Shader boxifyShader;

    void Start()
    {
        // Shader を取得
        if (boxifyShader == null)
        {
            Debug.LogError("Boxify Shader not found!");
            return;
        }

        // RenderTexture 作成
        //replacementRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        //replacementCamera.targetTexture = replacementRT;

        // ReplacementShader を Camera に設定
        replacementCamera.SetReplacementShader(boxifyShader, "");
    }

    // Strength を変更する例
    /*    public void SetStrength(float s)
        {
            boxifyStrength = s;
            Shader.SetGlobalFloat("_Strength", boxifyStrength);
        }*/
}
