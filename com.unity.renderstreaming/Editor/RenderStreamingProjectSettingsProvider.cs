using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.RenderStreaming.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.RenderStreaming
{
    internal class RenderStreamingProjectSettingsProvider : SettingsProvider
    {
        internal VisualElement rootVisualElement { get; private set; }

        const string kSettingsPath = "Project/Render Streaming";
        const string kTemplatePath = "Packages/com.unity.renderstreaming/Editor/UXML/RenderStreamingProjectSettings.uxml";
        const string kStylePath = "Packages/com.unity.renderstreaming/Editor/Styles/RenderStreamingProjectSettings.uss";

        private VisualElementCache cache;
        private VisualElement signalingTypeContainer => cache.Get<VisualElement>("signalingTypeContainer");
        private Editor.UI.SignalingSettings signalingSettings => cache.Get<Editor.UI.SignalingSettings>("signalingSettings");

        private static readonly IReadOnlyList<Type> relevantSignalingSettingTypes =
            TypeCache.GetTypesDerivedFrom<Unity.RenderStreaming.SignalingSettings>().Where(t => t.IsVisible && t.IsClass).ToList();
        private static readonly List<string> types = relevantSignalingSettingTypes.Select(x => x.Name).ToList();
        private static FieldInfo[] baseSignalingSettingFieldInfo =
            typeof(Unity.RenderStreaming.SignalingSettings).GetFields();
        private static Dictionary<string, FieldInfo[]> signalingSettingFieldInfosMap =
            relevantSignalingSettingTypes.ToDictionary(x => x.Name, x => x.GetFields());


        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new RenderStreamingProjectSettingsProvider(kSettingsPath, SettingsScope.Project, new List<string>()
            {
                L10n.Tr("experimental"),
                L10n.Tr("streaming"),
                L10n.Tr("webrtc"),
                L10n.Tr("video"),
                L10n.Tr("audio"),
            });
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var styleSheet = EditorGUIUtility.Load(kStylePath) as StyleSheet;

            rootVisualElement = new ScrollView();
            rootVisualElement.StretchToParentSize();
            rootVisualElement.styleSheets.Add(styleSheet);
            rootElement.Add(rootVisualElement);

            var template = EditorGUIUtility.Load(kTemplatePath) as VisualTreeAsset;

            VisualElement newVisualElement = new VisualElement();
            template.CloneTree(newVisualElement);
            rootVisualElement.Add(newVisualElement);

            cache = new VisualElementCache(newVisualElement);

            var popupField = new PopupField<string>("Signaling Type", types, 0);
            popupField.formatListItemCallback = s => s.Replace("Settings", "").Split('.').LastOrDefault();
            popupField.formatSelectedValueCallback = s => s.Replace("Settings", "").Split('.').LastOrDefault();
            signalingTypeContainer.Add(popupField);

            popupField.RegisterCallback<ChangeEvent<string>>(ChangeSignalingType);
            popupField.index = 1;
        }

        public RenderStreamingProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
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

            var newFieldNames = newTypeFields.Where(x =>
                !baseSignalingSettingFieldInfo.Select(y => y.Name).Contains(x.Name)).Select(x => x.Name);
            signalingSettings.ChangeSignalingType(newFieldNames);
        }
    }
}
