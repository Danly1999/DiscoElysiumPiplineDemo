using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

public class ScenceSpaceShadowFeature : ScriptableRendererFeature
{
    class ScenceSpaceShadowPass : ScriptableRenderPass
    {

        RenderTargetIdentifier m_Source;
        RenderTargetHandle m_TargetHandle;
        Material m_Material;




        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var Descriptor = renderingData.cameraData.cameraTargetDescriptor;
            Descriptor.depthBufferBits = 0;
            Descriptor.enableRandomWrite = true;
            cmd.GetTemporaryRT(m_TargetHandle.id,Descriptor,FilterMode.Point);

        }
        public ScenceSpaceShadowPass(Shader shader)
        {

            m_Material = CoreUtils.CreateEngineMaterial(shader);
            m_TargetHandle.Init("_ShadowMaskTexture");
        }
        public void Setup( RenderTargetIdentifier source)
        {
            this.m_Source = source;
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

                var cmd = CommandBufferPool.Get("ScenceSpaceShadowPass");
                int w = renderingData.cameraData.cameraTargetDescriptor.width;
                int h = renderingData.cameraData.cameraTargetDescriptor.height;
                Matrix4x4 proj = renderingData.cameraData.GetProjectionMatrix();
                Matrix4x4 view = renderingData.cameraData.GetViewMatrix();
                Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(proj, false);
                Matrix4x4 toScreen = new Matrix4x4(
                new Vector4(0.5f * w, 0.0f, 0.0f, 0.0f),
                new Vector4(0.0f, 0.5f * h, 0.0f, 0.0f),
                new Vector4(0.0f, 0.0f, 1.0f, 0.0f),
                new Vector4(0.5f * w, 0.5f * h, 0.0f, 1.0f)
                );
                Matrix4x4 zScaleBias = Matrix4x4.identity;

                Matrix4x4 screenToWorld = Matrix4x4.Inverse(toScreen * zScaleBias * gpuProj * view);
                cmd.SetGlobalMatrix("_ScreenToWorld",screenToWorld);
                // cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);

                // cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_Material,0,0);
                // cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);

                cmd.Blit(m_Source,m_TargetHandle.id,m_Material,0,0);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);

            
        }


        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            
        }
    }

    ScenceSpaceShadowPass m_ScriptablePass;
    public  Shader m_InputShader;
    //public ComputeShader computeShader;

    /// <inheritdoc/>
    public override void Create()
    {
        if(m_InputShader)
        {
        m_ScriptablePass = new ScenceSpaceShadowPass(m_InputShader);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents+1;

        }
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
            if(m_InputShader)
            {
                m_ScriptablePass.Setup(renderer.cameraColorTarget);
                renderer.EnqueuePass(m_ScriptablePass);

            }

        
    }
}


