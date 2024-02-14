using System;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using UnityEngine;

namespace Unity.RenderStreaming
{
    [RequireComponent(typeof(Broadcast))]
    public class RenderStreamingSingleton : MonoBehaviour
    {
        public static RenderStreamingSingleton renderStreamingSingleton;
        public Broadcast broadcastComponent;

        // public struct PlayerStreamingScripts
        // {
        //     VideoStreamSender videoStreamSender;
        //     AudioStreamSender audioStreamSender;
        //     InputSender inputSender;
        //     InputReceiver inputReceiver;
        // }
        public Dictionary<string, RenderStreamingHandler> RenderStreamingHandlers = new();

        private void Awake()
        {
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
            // If there is an instance, and it's not me, delete myself.
            if (renderStreamingSingleton != null && renderStreamingSingleton.gameObject != this.gameObject)
            {
                Destroy(this.gameObject);
            }
            else
            {
                renderStreamingSingleton = this;
                renderStreamingSingleton.broadcastComponent = GetComponent<Broadcast>();
                DontDestroyOnLoad(renderStreamingSingleton.gameObject.transform);
            }
        }
    }
}
