using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AllRenderFeature : ScriptableRendererFeature
{
    AllRenderPass m_CopyTexturePass;
    public Texture m_Tex;
    public Shader m_Shader;
    public Color m_color;


    /// <inheritdoc/>
    public override void Create()
    {
        if(m_Tex)
        Shader.SetGlobalTexture("_BaseColortexture",m_Tex);
        if(m_Shader)
        m_CopyTexturePass = new AllRenderPass(m_Shader);

        Shader.SetGlobalColor("_ShadowColor",m_color);

        m_CopyTexturePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents+2;
            
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(!renderingData.cameraData.isSceneViewCamera&&m_Shader)
        {
            m_CopyTexturePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(m_CopyTexturePass);

        }

    }
}


