using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerRenderFeature : ScriptableRendererFeature
{
    RenderObjectsPass m_RenderObjectsPass;
    //public Texture m_Tex;


    /// <inheritdoc/>
    public override void Create()
    {
        //if(m_Tex)
        //Shader.SetGlobalTexture("_BaseColortexture",m_Tex);
        m_RenderObjectsPass = new RenderObjectsPass("PlayerRenender",RenderPassEvent.BeforeRenderingTransparents,null,RenderQueueType.Opaque,LayerMask.GetMask("Sphere"));
        m_RenderObjectsPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents+3;
            
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if(!renderingData.cameraData.isSceneViewCamera)
        {
            //m_RenderObjectsPass.Setup(renderer.cameraColorTarget,renderer.cameraDepthTarget);
            renderer.EnqueuePass(m_RenderObjectsPass);

        }

    }
}


