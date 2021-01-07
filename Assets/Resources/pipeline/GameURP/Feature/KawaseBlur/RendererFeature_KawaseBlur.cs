/********************************************************
 * Copyright(C) 2020 by RoyApp All rights reserved.
 * FileName:    RendererFeature_KawaseBlur.cs
 * Author:      Roy Hu
 * Version:     2.0
 * Date:        2020-12-14 15:01
 * Description: 
 *******************************************************/

using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace RoyUnity
{
    [Serializable]
    public struct KawaseBlurFeatureSetting : IFeatureSetting
    {
        public int Iteration;
    }

    public class KawaseBlurRenderPass : BaseRenderPass<KawaseBlurFeatureSetting>
    {
        private string m_OffsetName = "_Offset";

        public override void SetCommand(CommandBuffer cmd)
        {
            var offset = 0.5f;
            RenderTargetIdentifier tmpRTId2 = GetTmpRTId(cmd, "tmpRT2");
            for (int i = 0; i < m_Setting.Iteration - 1; ++i)
            {
                cmd.SetGlobalFloat(m_OffsetName, offset + i);
                cmd.Blit(m_RTId_Tmp, tmpRTId2, m_Material);

                GameUtils.Swap(ref m_RTId_Tmp, ref tmpRTId2);
            }
            cmd.SetGlobalFloat(m_OffsetName, offset + m_Setting.Iteration - 1);
        }
    }

    public class RendererFeature_KawaseBlur : BaseRendererFeature<KawaseBlurFeatureSetting, KawaseBlurRenderPass> { }
}