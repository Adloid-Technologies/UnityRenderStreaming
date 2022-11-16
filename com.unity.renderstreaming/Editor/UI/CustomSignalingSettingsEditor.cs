using System;
using UnityEditor;
using UnityEngine;

namespace Unity.RenderStreaming.Editor.UI
{
    public class CustomSignalingSettingsEditor : Attribute
    {
        private static readonly TypeCache.TypeCollection customInspectorType =
            TypeCache.GetTypesWithAttribute<CustomSignalingSettingsEditor>();

        private readonly Type inspectedType;

        public CustomSignalingSettingsEditor(Type inspectedType)
        {
            if (inspectedType == null)
                Debug.LogError((object) "Failed to load CustomEditor inspected type");
            this.inspectedType = inspectedType;
        }

        internal static Type FindCustomInspectorTypeByType(Type inspectorType)
        {
            foreach (var typ in customInspectorType)
            {
                foreach (CustomSignalingSettingsEditor custom in typ.GetCustomAttributes(typeof(CustomSignalingSettingsEditor),false))
                {
                    if (custom.inspectedType == inspectorType)
                    {
                        return typ;
                    }
                }
            }

            return null;
        }
    }
}
