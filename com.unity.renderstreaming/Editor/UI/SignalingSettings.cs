using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.RenderStreaming.Editor.UI
{
    internal class SignalingSettings : VisualElement
    {
        const string kTemplatePath = "Packages/com.unity.renderstreaming/Editor/UXML/SignalingSettings.uxml";
        const string kStylePath = "Packages/com.unity.renderstreaming/Editor/Styles/SignalingSettings.uss";


        private static readonly IReadOnlyList<Type> relevantSignalingSettingTypes =
            TypeCache.GetTypesDerivedFrom<Unity.RenderStreaming.SignalingSettings>().Where(t => t.IsVisible && t.IsClass).ToList();

        private static FieldInfo[] baseSignalingSettingFieldInfo =
            typeof(Unity.RenderStreaming.SignalingSettings).GetFields();

        private static Dictionary<Type, FieldInfo[]> signalingSettingFieldInfosMap =
            relevantSignalingSettingTypes.ToDictionary(x => x, x => x.GetFields());

        internal new class UxmlFactory : UxmlFactory<SignalingSettings>
        {
        }

        //todo: change codecs model class
        internal List<ICEServer> sourceList =
            new List<ICEServer> {new ICEServer {urls = new[] {"stun:stun.l.google.com:19302"}}};

        internal ObservableCollection<ICEServer> draft;
        internal VisualElementCache cache;

        private TextField signalingUrlField => cache.Get<TextField>("signalingUrlField");
        private IntegerField iceServerCountField => cache.Get<IntegerField>("iceServerCountField");
        private VisualElement iceServerList => cache.Get<VisualElement>("iceServerList");
        private VisualElement extensionSettingContainer => cache.Get<VisualElement>("extensionSettingContainer");

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

            iceServerCountField.RegisterCallback<ChangeEvent<int>>(ChangeSize);
        }

        public void ChangeSignalingType(Type newType)
        {
            extensionSettingContainer.Clear();

            if (!signalingSettingFieldInfosMap.TryGetValue(newType, out var newTypeFields))
            {
                throw new InvalidOperationException();
            }

            var newFieldNames = newTypeFields
                .Where(x =>!baseSignalingSettingFieldInfo.Select(y => y.Name).Contains(x.Name))
                .Select(x => x.Name);
            foreach (var fieldName in newFieldNames)
            {
                extensionSettingContainer.Add(new TextField(fieldName));
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
