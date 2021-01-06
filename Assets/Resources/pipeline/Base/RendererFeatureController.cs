/********************************************************
 * Copyright(C) 2020 by RoyApp All rights reserved.
 * FileName:    RendererFeatureController.cs
 * Author:      Roy Hu
 * Version:     2.0
 * Date:        2020-12-22 17:08
 * Description: 
 *******************************************************/

using UnityEngine;

namespace RoyUnity
{
    public class RendererFeatureController : MonoBehaviour
    {
        public ABS_BaseRendererFeature m_Feature = null;

        public IFeatureSetting m_Setting;

        public bool Active = true;

        public virtual void Update()
        {
            if (m_Feature == null) return;
            m_Feature.SetActive(Active);
            if (Active) UpdateSetting();
        }

        private void OnDisable() => m_Feature?.SetActive(Active = false);

        public virtual void UpdateSetting() => m_Feature.SetSetting(m_Setting);
    }

    public class RendererFeatureController<S> : RendererFeatureController
        where S : IFeatureSetting
    {
        public new S m_Setting;

        public override void UpdateSetting() => m_Feature.SetSetting((S)m_Setting);
    }
}