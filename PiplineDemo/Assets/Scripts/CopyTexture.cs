using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

    public enum SetRender
    {
        BaseColor,
        Outline,
        Depth
    }
[ExecuteInEditMode]
public class CopyTexture : MonoBehaviour
{
    public RenderTexture renderTexture;
    RenderTexture nullTexture;
    Material mat;
    public SetRender mySet;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void Copy()
    {
        if(renderTexture != null)
        savePng(renderTexture,"Assets/test.png");
    }

    public void Render()
    {
        if(renderTexture != null)
        {
            Shader shader;
            if(mySet == SetRender.Depth)
            {
                shader = Shader.Find("Unlit/URPCopyDepth");

            }else if(mySet == SetRender.BaseColor)
            {
                shader = Shader.Find("Unlit/URPCopyColorShader");
            }else if(mySet == SetRender.Outline)
            {
                shader = Shader.Find("Unlit/OutlineShader");
            }else
            {
                shader = Shader.Find("Unlit/URPUnlitShader");
            }
            mat = new Material(shader);
            nullTexture = new RenderTexture(8,8,0);

            Graphics.Blit(nullTexture,renderTexture,mat);

        }
    }
    public void savePng(RenderTexture rt, string savePath) 
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        var directory = Path.GetDirectoryName(savePath);
        var fileName = Path.GetFileName(savePath);

        if (!string.IsNullOrEmpty(directory)) {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }
        else {
            Debug.LogErrorFormat("savePath directory no exist {0}", savePath);
            return;
        }
            File.WriteAllBytes(savePath, tex.EncodeToPNG());

            Debug.LogFormat("save png: {0}", savePath);
    }
}
