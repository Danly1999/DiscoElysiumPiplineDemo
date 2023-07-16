using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class AllRenderPass : ScriptableRenderPass
    {

        RenderTargetIdentifier m_Source;
        RenderTargetHandle m_Target;
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
            cmd.GetTemporaryRT(m_Target.id,Descriptor,FilterMode.Point);
            //ConfigureClear(ClearFlag.None,Color.black);
            ConfigureTarget(m_Source,m_Target.id);
        }
        public AllRenderPass(Shader shader)
        {
            //Shader shader = Shader.Find("Unlit/AllRender");
            m_Material = new Material(shader);
            m_Target.Init("_OverTexture");

            
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

            var cmd = CommandBufferPool.Get("AllRenderPass");
            cmd.Blit(m_Source,m_Target.id);
            cmd.SetGlobalTexture("_BlitTexture",m_Target.id);
            cmd.SetRenderTarget(m_Source);
            cmd.Blit(m_Target.id,m_Source,m_Material,0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

        }


        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }