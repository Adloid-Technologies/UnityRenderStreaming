using System;
using UnityEngine.InputSystem;

namespace Unity.RenderStreaming
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class InputChannelReceiverBase : DataChannelBase
    {
        /// <summary>
        /// 
        /// </summary>
#pragma warning disable 0067
        public virtual event Action<InputDevice, InputDeviceChange> onDeviceChange;
        public virtual event Action<Unity.RenderStreaming.InputSystem.InputRemoting.UserMessage> onUserMessage;
#pragma warning restore 0067
    }
}
