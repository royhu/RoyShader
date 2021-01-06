/********************************************************
 * Copyright(C) 2020 by RoyApp All rights reserved.
 * FileName:    URPHelperInspector.cs
 * Author:      Roy Hu
 * Version:     2.0
 * Date:        2020-12-21 19:51
 * Description: 
 *******************************************************/

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace RoyUnity.Editor
{
    [CustomEditor(typeof(URPHelper))]
    public class URPHelperInspector : UnityEditor.Editor
    {
        struct FeatureProperty
        {
            public SerializedObject Object;
            public string Name;
            public SerializedProperty Active;

            public SerializedProperty Setting;

            public AnimBool AnimActive;
        }

        private List<ScriptableRendererFeature> m_Features;
        private List<FeatureProperty> m_Props;

        private void OnEnable()
        {
            m_Features = (target as URPHelper).m_Renderer?.rendererFeatures;
            m_Props = new List<FeatureProperty>();

            SerializedObject so;
            foreach (var feature in m_Features)
            {
                so = new SerializedObject(feature);

                FeatureProperty fp;
                fp.Object = so;
                fp.Name = so.FindProperty("m_Name").stringValue;
                fp.Active = so.FindProperty("m_Active");
                fp.Setting = so.FindProperty(nameof(ABS_BaseRendererFeature.m_Setting));
                fp.AnimActive = new AnimBool(fp.Active.boolValue);
                fp.AnimActive.valueChanged.AddListener(Repaint);

                m_Props.Add(fp);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            foreach (var fp in m_Props)
            {
                fp.AnimActive.target = EditorGUILayout.ToggleLeft(fp.Name, fp.AnimActive.target);
                if (EditorGUILayout.BeginFadeGroup(fp.AnimActive.faded))
                {
                    EditorGUI.indentLevel += 2;
                    EditorGUILayout.PropertyField(fp.Setting);
                    EditorGUI.indentLevel -= 2;
                }
                EditorGUILayout.EndFadeGroup();
                fp.Active.boolValue = fp.AnimActive.value;

                if (fp.Object.hasModifiedProperties)
                    fp.Object.ApplyModifiedProperties();
            }
        }
    }
}