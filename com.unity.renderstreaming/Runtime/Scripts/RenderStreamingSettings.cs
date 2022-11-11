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

    public abstract class SignalingSettings
    {
        public bool runOnAwake;
        public string urlSignaling = "http://localhost";
        public ICEServer[] iceServers;
    }

    [Serializable]
    public class ICEServer
    {
        public string credential;
        public RTCIceCredentialType credentialType;
        public string[] urls;
        public string username;

        public static implicit operator RTCIceServer(ICEServer source)
        {
            return new RTCIceServer
            {
                credential = source.credential,
                credentialType = source.credentialType,
                urls = source.urls,
                username = source.username
            };
        }

        public static implicit operator ICEServer(RTCIceServer source)
        {
            return new ICEServer
            {
                credential = source.credential,
                credentialType = source.credentialType,
                urls = source.urls,
                username = source.username
            };
        }
    }

    public class HttpSignalingSettings : SignalingSettings
    {
        public float interval = 5.0f;
    }

    public class WebSocketSignalingSettings : SignalingSettings
    {
    }

    public class FurioosSignalingSettings : SignalingSettings
    {
    }

    [Serializable]
    public class CodecSettings : ScriptableObject
    {
    }
}
