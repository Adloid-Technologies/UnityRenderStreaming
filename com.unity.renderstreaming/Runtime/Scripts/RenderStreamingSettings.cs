using System;
using Unity.WebRTC;
using UnityEngine;

namespace Unity.RenderStreaming
{
    [CreateAssetMenu]
    public class RenderStreamingSettings : ScriptableObject
    {
        [SerializeField] public bool automaticStreaming;
        [SerializeReference] public SignalingSettings signalingSettings = new WebSocketSignalingSettings();
        [SerializeReference] public CodecSettings codecSettings = new CodecSettings();
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

    [Serializable]
    public class HttpSignalingSettings : SignalingSettings
    {
        public float interval = 5.0f;
    }

    [Serializable]
    public class WebSocketSignalingSettings : SignalingSettings
    {
    }

    [Serializable]
    public class FurioosSignalingSettings : SignalingSettings
    {
    }

    [Serializable]
    public class CodecSettings
    {
    }
}
