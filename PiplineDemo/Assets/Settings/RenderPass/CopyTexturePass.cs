using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CopyTexturePass : ScriptableRenderPass
    {

        RenderTargetIdentifier m_Source;
        RenderTargetHandle m_Target;
        RenderTexture m_TargetHandle;
        //ComputeShader m_ComputeShader;
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

            //ConfigureTarget(m_Source,m_Target.id);

            // m_TargetHandle = new RenderTexture(2048, 1024, 24);
            // m_TargetHandle.enableRandomWrite = true;
            // m_TargetHandle.Create();
        }
        public CopyTexturePass()
        {
            Shader shader = Shader.Find("Unlit/OutlineShader");
            m_Material = new Material(shader);
            //m_ComputeShader = computeShader;
            m_Target.Init("_CopyTexture");

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

            var cmd = CommandBufferPool.Get("CopyTexturePass");
            cmd.Blit(m_Source,m_Target.id);
            cmd.SetGlobalTexture("_OutlineTexture",m_Target.id);
            cmd.Blit(m_Target.id,m_Source,m_Material,0);
            cmd.SetGlobalTexture("_OutlineTexture",m_Source);

            // int kernelHandle = m_ComputeShader.FindKernel("CSMain");
            // cmd.SetComputeTextureParam(m_ComputeShader,kernelHandle,"SouceResult",m_Source);
            // cmd.SetComputeTextureParam(m_ComputeShader,kernelHandle,"TargetResult",m_TargetHandle);
            // cmd.DispatchCompute(m_ComputeShader,kernelHandle,2048/8,1024/8,1);
                
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            // if(renderingData.cameraData.cameraType == CameraType.Game)
            // {
            //     if(!System.IO.File.Exists("Assets/test.renderTexture"))
            //     {
            //         AssetDatabase.CreateAsset(m_TargetHandle,"Assets/test.renderTexture"); 
                    
            //     }
            // }


            
        }


        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            //m_TargetHandle.Release();
        }
    }