using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.RenderStreaming.Editor.UI
{
    internal class IceServerSettings : VisualElement
    {
        const string kTemplatePath = "Packages/com.unity.renderstreaming/Editor/UXML/IceServerSettings.uxml";
        const string kStylePath = "Packages/com.unity.renderstreaming/Editor/Styles/IceServerSettings.uss";

        //todo: change codecs model class
        internal List<string> sourceList = new List<string> {""};
#if UNITY_2021_3_OR_NEWER
        internal List<string> draft;
#else
        internal ObservableCollection<string> draft;
#endif

        internal VisualElementCache cache;
        private Foldout titleLabel => cache.Get<Foldout>("title");
        private ListView urlList => cache.Get<ListView>("urlList");
        private Button addButton => cache.Get<Button>("addButton");
        private Button removeButton => cache.Get<Button>("removeButton");
        private EnumField credentialTypeField => cache.Get<EnumField>("credentialTypeField");

        public event Action onClick;

        public IceServerSettings(int index)
        {
            var styleSheet = EditorGUIUtility.Load(kStylePath) as StyleSheet;
            styleSheets.Add(styleSheet);

            var template = EditorGUIUtility.Load(kTemplatePath) as VisualTreeAsset;
            var newVisualElement = new VisualElement();
            template.CloneTree(newVisualElement);
            this.Add(newVisualElement);

            cache = new VisualElementCache(newVisualElement);

            this.AddManipulator(new Clickable(OnClick));

            titleLabel.text = $"Ice Server[{index}]";
            titleLabel.value = false;

#if UNITY_2021_3_OR_NEWER
            draft = new List<string>(sourceList);
            Func<VisualElement> makeItem = () =>
            {
                var textField = new TextField();
                textField.StretchToParentWidth();
                return textField;
            };
#else
            // workaround for unity 2020.3
            // if unity 2021.3 later, prefer using ListView.itemIndexChanged event
            draft = new ObservableCollection<string>(sourceList);
            Func<VisualElement> makeItem = () =>
            {
                var textField = new TextField();
                textField.StretchToParentWidth();
                return textField;
            };
#endif

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                e.contentContainer.Q<TextField>().value = draft[i];
            };
            urlList.makeItem = makeItem;
            urlList.bindItem = bindItem;
            urlList.itemsSource = draft;
            urlList.selectionType = SelectionType.Single;
            urlList.itemHeight = 20;
            urlList.style.height = urlList.itemHeight * draft.Count;
            urlList.reorderable = true;

            urlList.onSelectionChange += objects =>
            {
                removeButton.SetEnabled(objects.Any() && urlList.itemsSource.Count > 1);
            };

            addButton.SetEnabled(true);
            addButton.clickable.clicked += AddUrl;
            removeButton.SetEnabled(false);
            removeButton.clickable.clicked += RemoveUrl;

            credentialTypeField.RegisterCallback<ChangeEvent<Enum>>((evt) =>
            {
                // csharpField.value = evt.newValue;
            });
        }

        private void OnClick()
        {
            onClick?.Invoke();
        }

        private void AddUrl()
        {
            draft.Add("");
            UpdateList();
        }

        private void RemoveUrl()
        {
            foreach (var selectItem in urlList.selectedItems.Cast<string>())
            {
                draft.Remove(selectItem);
            }

            UpdateList();
        }

        private void UpdateList()
        {
            urlList.ClearSelection();
            urlList.style.height = urlList.itemHeight * urlList.itemsSource.Count;
            urlList.Refresh();
        }
    }
}
