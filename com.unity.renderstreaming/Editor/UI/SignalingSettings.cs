using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Unity.RenderStreaming.Signaling;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.RenderStreaming.Editor.UI
{
    internal class SignalingSettings : VisualElement
    {
        const string kTemplatePath = "Packages/com.unity.renderstreaming/Editor/UXML/SignalingSettings.uxml";
        const string kStylePath = "Packages/com.unity.renderstreaming/Editor/Styles/SignalingSettings.uss";

        internal new class UxmlFactory : UxmlFactory<SignalingSettings>
        {
        }

        //todo: change codecs model class
        internal List<ICEServer> sourceList =
            new List<ICEServer> {new ICEServer {urls = new[] {"stun:stun.l.google.com:19302"}}};

        internal ObservableCollection<ICEServer> draft;
        internal VisualElementCache cache;

        private VisualElement signalingTypeContainer => cache.Get<VisualElement>("signalingTypeContainer");
        private TextField signalingUrlField => cache.Get<TextField>("signalingUrlField");
        private IntegerField iceServerCountField => cache.Get<IntegerField>("iceServerCountField");
        private VisualElement iceServerList => cache.Get<VisualElement>("iceServerList");
        private VisualElement extensionSettingContainer => cache.Get<VisualElement>("extensionSettingContainer");

        private static readonly IReadOnlyList<Type> relevantSignalingSettingTypes =
            TypeCache.GetTypesDerivedFrom<Unity.RenderStreaming.SignalingSettings>().Where(t => t.IsVisible && t.IsClass).ToList();
        private static readonly List<string> types = relevantSignalingSettingTypes.Select(x => x.Name).ToList();
        private static FieldInfo[] baseSignalingSettingFieldInfo =
            typeof(Unity.RenderStreaming.SignalingSettings).GetFields();
        private static Dictionary<string, FieldInfo[]> signalingSettingFieldInfosMap =
            relevantSignalingSettingTypes.ToDictionary(x => x.Name, x => x.GetFields());

        public SignalingSettings()
        {
            var styleSheet = EditorGUIUtility.Load(kStylePath) as StyleSheet;
            styleSheets.Add(styleSheet);

            var template = EditorGUIUtility.Load(kTemplatePath) as VisualTreeAsset;
            var newVisualElement = new VisualElement();
            template.CloneTree(newVisualElement);
            this.Add(newVisualElement);

            cache = new VisualElementCache(newVisualElement);
            draft = new ObservableCollection<ICEServer>();

            var popupField = new PopupField<string>("Signaling Type", types, 0);
            popupField.formatListItemCallback = s => s.Replace("Settings", "").Split('.').LastOrDefault();
            popupField.formatSelectedValueCallback = s => s.Replace("Settings", "").Split('.').LastOrDefault();
            signalingTypeContainer.Add(popupField);

            popupField.RegisterCallback<ChangeEvent<string>>(ChangeSignalingType);
            popupField.index = 1;

            iceServerCountField.RegisterCallback<ChangeEvent<int>>(ChangeSize);
        }

        private void ChangeSignalingType(ChangeEvent<string> evt)
        {
            if (evt.previousValue == evt.newValue)
            {
                return;
            }

            if (!signalingSettingFieldInfosMap.TryGetValue(evt.newValue, out var newTypeFields))
            {
                throw new InvalidOperationException();
            }

            extensionSettingContainer.Clear();
            foreach (var field in newTypeFields.Where(x =>
                         !baseSignalingSettingFieldInfo.Select(y => y.Name).Contains(x.Name)))
            {
                extensionSettingContainer.Add(new TextField(field.Name));
            }
        }

        private void ChangeSize(ChangeEvent<int> evt)
        {
            var diff = evt.newValue - evt.previousValue;
            if (diff == 0)
            {
                return;
            }

            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    draft.Add(new ICEServer());
                    iceServerList.Add(new IceServerSettings(draft.Count));
                }
            }
            else
            {
                for (int i = 0; i > diff; i--)
                {
                    draft.RemoveAt(draft.Count - 1);
                    iceServerList.RemoveAt(iceServerCountField.childCount - 1);
                }
            }
        }
    }
}
