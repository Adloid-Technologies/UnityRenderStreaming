using System;
using Unity.WebRTC;
using UnityEngine;

namespace Unity.RenderStreaming
{
    public class RenderStreamingSettings : ScriptableObject
    {
        public bool automaticStreaming;
        public SignalingSettings signalingSettings;
        public CodecSettings codecSettings;
    }

    public enum SignalingType
    {
        WebSocket,
        Http,
        Furioos
    }

    [Serializable]
    public class SignalingSettings
    {
        public bool runOnAwake;
        public string urlSignaling = "http://localhost";
        public SignalingType signalingType = SignalingType.WebSocket;
        public RTCIceServer[] iceServers;
        public float interval = 5.0f;
    }

    public class HttpSignalingSettings : SignalingSettings
    {
    }

    public class WebSocketSignalingSettings : SignalingSettings
    {
    }

    public class FurioosSignalingSettings : SignalingSettings
    {
    }

    [Serializable]
    public class CodecSettings
    {
    }
}
