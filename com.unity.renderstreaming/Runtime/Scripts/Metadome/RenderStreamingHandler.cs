using System;
using System.Collections;
using System.Collections.Generic;
using Unity.RenderStreaming;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Unity.RenderStreaming
{
    [RequireComponent(typeof(VideoStreamSender))]
    [RequireComponent(typeof(AudioStreamSender))]
    [RequireComponent(typeof(InputSender))]
    [RequireComponent(typeof(InputReceiver))]
    public class RenderStreamingHandler : MonoBehaviour
    {
        public void SendData(string data)
        {
            _inputSender.SendUserMessage(data);
        }

        public delegate void OnDataHandler(string data, byte[] rawData);

        public event OnDataHandler OnData;

        private string _uuid;
        private RenderStreamingSingleton _renderStreamingSingleton;
        private VideoStreamSender _videoStreamSender;
        private AudioStreamSender _audioStreamSender;
        private InputSender _inputSender;
        private InputReceiver _inputReceiver;

        public List<Component> Streams =>
            new() { _videoStreamSender, _audioStreamSender, _inputSender, _inputReceiver };

        private List<Action> _jobs;
        private const int MaxJobsPerFrame = 1000;

        private void QueueJob(Action job)
        {
            _jobs ??= new List<Action>();
            _jobs.Add(job);
        }

        private class ResolutionRequest
        {
            [DataMember] public string id;
            [DataMember] public int height = 1080;
            [DataMember] public int width = 1920;
        }

        private IEnumerator _setWebRtcResolutionRoutine = null;

        private IEnumerator SetWebRtcResolution(int w, int h)
        {
            yield return new WaitForSeconds(1);
            GetComponent<VideoStreamSender>().SetTextureSize(new Vector2Int(w, h));
        }

        private void Awake()
        {
            _renderStreamingSingleton = RenderStreamingSingleton.renderStreamingSingleton
                ? RenderStreamingSingleton.renderStreamingSingleton
                : FindObjectOfType<RenderStreamingSingleton>();
            _videoStreamSender = GetComponent<VideoStreamSender>();
            _audioStreamSender = GetComponent<AudioStreamSender>();
            _inputSender = GetComponent<InputSender>();
            _inputReceiver = GetComponent<InputReceiver>();
            _uuid = System.Guid.NewGuid().ToString();

            _renderStreamingSingleton.RenderStreamingHandlers.Add(_uuid, this);
            _renderStreamingSingleton.broadcastComponent.AddComponent(_videoStreamSender);
            _renderStreamingSingleton.broadcastComponent.AddComponent(_audioStreamSender);
            _renderStreamingSingleton.broadcastComponent.AddComponent(_inputSender);
            _renderStreamingSingleton.broadcastComponent.AddComponent(_inputReceiver);
            // Screen.SetResolution((int)_videoStreamSender.width, (int)_videoStreamSender.height, FullScreenMode.Windowed);
            _inputReceiver.onUserMessage += InputReceiver_onUserMessage;
            // OnData += (data, buffer) =>
            // {
            //     Debug.Log(data);
            // };
        }

        private void OnDestroy()
        {
            _renderStreamingSingleton.RenderStreamingHandlers.Remove(_uuid);
            _renderStreamingSingleton.broadcastComponent.RemoveComponent(_videoStreamSender);
            _renderStreamingSingleton.broadcastComponent.RemoveComponent(_audioStreamSender);
            _renderStreamingSingleton.broadcastComponent.RemoveComponent(_inputSender);
            _renderStreamingSingleton.broadcastComponent.RemoveComponent(_inputReceiver);
        }

        private void Update()
        {
            if (_jobs != null)
            {
                var jobsExecutedCount = 0;
                while (_jobs.Count > 0 && jobsExecutedCount++ < MaxJobsPerFrame)
                {
                    var job = _jobs[0];
                    _jobs.RemoveAt(0);
                    try
                    {
                        job.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Job invoke exception: " + e.Message);
                    }
                }
            }
        }

        private void InputReceiver_onUserMessage(Unity.RenderStreaming.InputSystem.InputRemoting.UserMessage obj)
        {
            Debug.Log(obj.message);
                void Handler()
                {
                    OnData?.Invoke(obj.message, Encoding.UTF8.GetBytes(obj.message));
                }

                QueueJob(Handler);
        }
    }
}
