using UnityEngine;
using UnityEditor;
using System.Collections.Generic;



public class CreateSSSLUT
{
    [MenuItem("Tools/CreateSSSLUT")]
    static void CreateLUT()
    {
        int width = 512;
        int height = 512;
        Material mat;

        RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        Graphics.SetRenderTarget(rt);

        var lutShader = Shader.Find("SSS/CreateLUT");
        if (lutShader == null)
        {
            Debug.LogError("SSS/CreateLUT shader can not find.");
            return;
        }

        mat = new Material(lutShader);

        mat.SetPass(0);
        Graphics.DrawMeshNow(UnityEngine.Rendering.Universal.RenderingUtils.fullscreenMesh, Vector3.zero, Quaternion.identity);

        Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        System.IO.File.WriteAllBytes("Assets/LUTSSS.png", result.EncodeToPNG());

        Graphics.SetRenderTarget(null);
        rt.Release();
        AssetDatabase.Refresh();
    }
}
