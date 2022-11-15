using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.RenderStreaming.Editor.UI
{
    internal class SignalingTypePopup : PopupField<string>
    {
        public delegate void ChangeSignalingSettingField(Type newType);

        public event ChangeSignalingSettingField ChangeEvent;

        private static readonly IReadOnlyList<Type> relevantSignalingSettingTypes =
            TypeCache.GetTypesDerivedFrom<Unity.RenderStreaming.SignalingSettings>().Where(t => t.IsVisible && t.IsClass).ToList();

        private static readonly List<string> types = relevantSignalingSettingTypes.Select(x => x.Name).ToList();

        public SignalingTypePopup(string label, int defaultIndex) :
            base(label, types, defaultIndex, ItemNameFormatter, ItemNameFormatter)
        {
            RegisterCallback<ChangeEvent<string>>(ChangeSignalingType);
        }

        private static string ItemNameFormatter(string original)
        {
            return original.Replace("Settings", "").Split('.').LastOrDefault();
        }

        private void ChangeSignalingType(ChangeEvent<string> evt)
        {
            if (evt.previousValue == evt.newValue)
            {
                return;
            }

            var type = relevantSignalingSettingTypes.First(x => x.Name == evt.newValue);
            ChangeEvent?.Invoke(type);
        }
    }
}
