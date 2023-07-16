using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature : ScriptableRendererFeature
{
    CopyTexturePass m_CopyTexturePass;
    RenderTexture renderTexture;

    //public  ComputeShader m_ComputeShader;

    /// <inheritdoc/>
    public override void Create()
    {



            m_CopyTexturePass = new CopyTexturePass();
            m_CopyTexturePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        m_CopyTexturePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_CopyTexturePass);


            

        
    }
}


