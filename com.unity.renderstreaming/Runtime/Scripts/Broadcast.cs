using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.RenderStreaming
{
    public class Broadcast : SignalingHandlerBase,
        IOfferHandler, IAddChannelHandler, IDisconnectHandler, IDeletedConnectionHandler,
        IAddReceiverHandler
    {
        [SerializeField] private List<Component> streams = new List<Component>();

        private List<string> connectionIds = new List<string>();
        private Dictionary<string, string> mapFromConnectionIDToHandlerUuid = new ();

        public override IEnumerable<Component> Streams => streams;

        public void AddComponent(Component component)
        {
            streams.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            streams.Remove(component);
        }

        public void OnDeletedConnection(SignalingEventData eventData)
        {
            Disconnect(eventData.connectionId);
        }

        public void OnDisconnect(SignalingEventData eventData)
        {
            Disconnect(eventData.connectionId);
        }

        private void Disconnect(string connectionId)
        {
            if (!connectionIds.Contains(connectionId))
                return;
            connectionIds.Remove(connectionId);
            UnAssignHandler(connectionId);
            if (mapFromConnectionIDToHandlerUuid.TryGetValue(connectionId, out var value))
            {
                List<Component> relevantStreams = RenderStreamingSingleton.renderStreamingSingleton.RenderStreamingHandlers[value]
                    .Streams;
                foreach (var sender in relevantStreams.OfType<IStreamSender>())
                {
                    RemoveSender(connectionId, sender);
                }
                foreach (var receiver in relevantStreams.OfType<IStreamReceiver>())
                {
                    RemoveReceiver(connectionId, receiver);
                }
                foreach (var channel in relevantStreams.OfType<IDataChannel>().Where(c => c.ConnectionId == connectionId))
                {
                    RemoveChannel(connectionId, channel);
                }
            }

            // foreach (var sender in streams.OfType<IStreamSender>())
            // {
            //     RemoveSender(connectionId, sender);
            // }
            // foreach (var receiver in streams.OfType<IStreamReceiver>())
            // {
            //     RemoveReceiver(connectionId, receiver);
            // }
            // foreach (var channel in streams.OfType<IDataChannel>().Where(c => c.ConnectionId == connectionId))
            // {
            //     RemoveChannel(connectionId, channel);
            // }
        }

        public void OnAddReceiver(SignalingEventData data)
        {
            var track = data.transceiver.Receiver.Track;
            // IStreamReceiver receiver = GetReceiver(track.Kind);
            
            // AssignHandler(data.connectionId);
            IStreamReceiver receiver = GetReceiver(data.connectionId, track.Kind);
            SetReceiver(data.connectionId, receiver, data.transceiver);
        }

        public void OnOffer(SignalingEventData data)
        {
            if (connectionIds.Contains(data.connectionId))
            {
                RenderStreaming.Logger.Log($"Already answered this connectionId : {data.connectionId}");
                return;
            }
            connectionIds.Add(data.connectionId);
            AssignHandler(data.connectionId);
            if (mapFromConnectionIDToHandlerUuid.TryGetValue(data.connectionId, out var value))
            {
                List<Component> relevantStreams = RenderStreamingSingleton.renderStreamingSingleton
                    .RenderStreamingHandlers[value]
                    .Streams;
                foreach (var source in relevantStreams.OfType<IStreamSender>())
                {
                    AddSender(data.connectionId, source);
                }
                foreach (var channel in relevantStreams.OfType<IDataChannel>().Where(c => c.IsLocal))
                {
                    AddChannel(data.connectionId, channel);
                }
            }

            // foreach (var source in streams.OfType<IStreamSender>())
            // {
            //     AddSender(data.connectionId, source);
            // }
            // foreach (var channel in streams.OfType<IDataChannel>().Where(c => c.IsLocal))
            // {
            //     AddChannel(data.connectionId, channel);
            // }
            SendAnswer(data.connectionId);
        }

        public void OnAddChannel(SignalingEventData data)
        {
            // AssignHandler(data.connectionId);
            if (!mapFromConnectionIDToHandlerUuid.TryGetValue(data.connectionId, out var value))
                throw new System.Exception();
            var relevantStreams = RenderStreamingSingleton.renderStreamingSingleton
                .RenderStreamingHandlers[value]
                .Streams;
            switch (data.channel.Label)
            {
                case "sender":
                {
                    var channel = relevantStreams.OfType<InputReceiver>().
                        FirstOrDefault(r => !r.IsConnected && !r.IsLocal);
                    channel?.SetChannel(data.connectionId, data.channel);
                    break;
                }
                case "receiver":
                {
                    var channel = relevantStreams.OfType<InputSender>().
                        FirstOrDefault(r => !r.IsConnected && !r.IsLocal);
                    channel?.SetChannel(data.connectionId, data.channel);
                    break;
                }
                default:
                {
                    var channel = relevantStreams.OfType<IDataChannel>().
                        FirstOrDefault(r => !r.IsConnected && !r.IsLocal);
                    channel?.SetChannel(data.connectionId, data.channel);
                    break;
                }
            }

            // var channel = streams.OfType<IDataChannel>().
            //     FirstOrDefault(r => !r.IsConnected && !r.IsLocal);
            // channel?.SetChannel(data.connectionId, data.channel);
        }

        IStreamReceiver GetReceiver(WebRTC.TrackKind kind)
        {
            if (kind == WebRTC.TrackKind.Audio)
                return streams.OfType<AudioStreamReceiver>().First();
            if (kind == WebRTC.TrackKind.Video)
                return streams.OfType<VideoStreamReceiver>().First();
            throw new System.ArgumentException();
        }

        void AssignHandler(string connectionId)
        {
            if (mapFromConnectionIDToHandlerUuid.ContainsKey(connectionId))
            {
            }
            else
            {
                foreach (string key in RenderStreamingSingleton.renderStreamingSingleton
                             .RenderStreamingHandlers.Keys)
                {
                    if (mapFromConnectionIDToHandlerUuid.ContainsValue(key))
                    {
                    }
                    else
                    {
                        mapFromConnectionIDToHandlerUuid.Add(connectionId, key);
                        return;
                    }
                }
                throw new System.Exception("Handlers used up");
            }
        }

        void UnAssignHandler(string connectionId)
        {
            if (mapFromConnectionIDToHandlerUuid.ContainsKey(connectionId))
            {
                mapFromConnectionIDToHandlerUuid.Remove(connectionId);
            }
        }
        
        IStreamReceiver GetReceiver(string connectionId, WebRTC.TrackKind kind)
        {
            if (mapFromConnectionIDToHandlerUuid.TryGetValue(connectionId, out var value))
            {
                List<Component> relevantStreams = RenderStreamingSingleton.renderStreamingSingleton
                    .RenderStreamingHandlers[value]
                    .Streams;
                if (kind == WebRTC.TrackKind.Audio)
                    return relevantStreams.OfType<AudioStreamReceiver>().First();
                if (kind == WebRTC.TrackKind.Video)
                    return relevantStreams.OfType<VideoStreamReceiver>().First();
            }
            throw new System.Exception();
        }
    }
}
