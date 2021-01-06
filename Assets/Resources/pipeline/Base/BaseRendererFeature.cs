/********************************************************
 * Copyright(C) 2020 by RoyApp All rights reserved.
 * FileName:    BaseRendererFeature.cs
 * Author:      Roy Hu
 * Version:     2.0
 * Date:        2020-12-15 18:20
 * Description: 
 *******************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace RoyUnity
{
    public interface IFeatureSetting { }

    public class BaseRenderPass<S> : ScriptableRenderPass
        where S : IFeatureSetting
    {
        protected S m_Setting;
        private int m_DownScale;
        protected Material m_Material = null;
        protected RenderTargetIdentifier m_RTId_Source;
        private List<int> m_TmpIds = new List<int>(2);
        protected RenderTargetIdentifier m_RTId_Tmp;
        private int m_RTWidth;
        private int m_RTHeight;

        public void Init(S setting, Shader shader, RenderPassEvent passEvent, int downScale)
        {
            if (shader != null)
            {
                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            renderPassEvent = passEvent;
            m_DownScale = downScale;
            SetSetting(setting);
        }

        public virtual void SetSetting() { }

        public void SetSetting(S setting)
        {
            m_Setting = setting;
            SetSetting();
        }

        public void Setup(RenderTargetIdentifier src) => m_RTId_Source = src;

        protected RenderTargetIdentifier GetTmpRTId(CommandBuffer cmd, string name)
        {
            int id = Shader.PropertyToID(name);
            m_TmpIds.Add(id);

            cmd.GetTemporaryRT(id, m_RTWidth, m_RTHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
            var rtId = new RenderTargetIdentifier(id);
            ConfigureTarget(rtId);

            return rtId;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);

            m_RTWidth = cameraTextureDescriptor.width / m_DownScale;
            m_RTHeight = cameraTextureDescriptor.height / m_DownScale;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_Material == null) return;

            var name = GetType().Name;
            CommandBuffer cmd = CommandBufferPool.Get(name);
            cmd.BeginSample(name);

            m_RTId_Tmp = GetTmpRTId(cmd, "tmpId");
            cmd.Blit(m_RTId_Source, m_RTId_Tmp);
            SetCommand(cmd);
            cmd.Blit(m_RTId_Tmp, m_RTId_Source, m_Material);
            context.ExecuteCommandBuffer(cmd);

            foreach (int id in m_TmpIds) cmd.ReleaseTemporaryRT(id);
            m_TmpIds.Clear();

            cmd.EndSample(name);
            CommandBufferPool.Release(cmd);
        }

        public virtual void SetCommand(CommandBuffer command) { }
    }

    public abstract class ABS_BaseRendererFeature : ScriptableRendererFeature
    {
        [HideInInspector]
        public RenderPassEvent m_PassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Shader m_Shader = null;
        [HideInInspector]
        public int m_DownScale = 2;
        public IFeatureSetting m_Setting;

        public virtual void SetSetting(IFeatureSetting setting) => m_Setting = setting;
    }

    public abstract class BaseRendererFeature<S, P> : ABS_BaseRendererFeature
        where S : IFeatureSetting
        where P : BaseRenderPass<S>, new()
    {
        public new S m_Setting;
        protected P m_RenderPass;

        public override void Create()
        {
            m_RenderPass = new P();
            m_RenderPass.Init(m_Setting, m_Shader, m_PassEvent, m_DownScale);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_RenderPass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(m_RenderPass);
        }
        public override void SetSetting(IFeatureSetting setting) => m_RenderPass.SetSetting(m_Setting = (S)setting);
    }
}