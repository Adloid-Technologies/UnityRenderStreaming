using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.RenderStreaming.Editor.UI
{
    [CustomEditor(typeof(RenderStreamingSettings))]
    internal class RenderStreamingSettingsEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            // var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SampleUI.uxml");
            // visualTree.CloneTree(root);

            // todo: use listview visual element
            root.Add(new IMGUIContainer(() =>
            {
                serializedObject.Update();
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    SerializedProperty prop = serializedObject.FindProperty("m_Script");
                    EditorGUILayout.PropertyField(prop, true, Array.Empty<GUILayoutOption>());
                }

                serializedObject.ApplyModifiedProperties();
            }));

            var signalingSettingsUI = new SignalingSettings();
            var popupField = new SignalingTypePopup("Signaling Type", 0);
            popupField.ChangeEvent += newType =>
            {
                var property = serializedObject.FindProperty("signalingSettings");
                property.managedReferenceValue = Activator.CreateInstance(newType) as SignalingSettings;
                signalingSettingsUI.ChangeSignalingType(newType);
            };
            root.Add(popupField);
            root.Add(signalingSettingsUI);

            return root;
        }
    }
}
