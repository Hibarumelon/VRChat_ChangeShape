using UnityEngine;

public class OverlayToTPCamera: MonoBehaviour
{
    public Camera replacementCamera;
    public RenderTexture replacementRT;
    public Material overlayMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        overlayMaterial.mainTexture = replacementRT;

        // src に replacementRT を合成
        Graphics.Blit(src, dest, overlayMaterial);
    }
}
