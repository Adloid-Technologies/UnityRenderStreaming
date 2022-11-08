using System.Linq;
using Unity.RenderStreaming;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    private RenderStreamingHandler renderstreaming;
    private Broadcast broadcast;
    private SingleConnection singleConnection;
    private VideoStreamSender videoStreamSender;
    private VideoStreamReceiver receiveVideoViewer;
    private AudioStreamSender audioStreamSender;

    [SerializeField] private RawImage remoteVideoImage;
    private void Awake()
    {
        videoStreamSender = gameObject.AddComponent<VideoStreamSender>();
        videoStreamSender.source = VideoStreamSource.Screen;

        videoStreamSender.sourceDeviceIndex = 0;
        videoStreamSender.autoRequestUserAuthorization = true;
        videoStreamSender.sourceCamera = Camera.main;
        videoStreamSender.sourceTexture = remoteVideoImage.texture;
        videoStreamSender.SetTextureSize(new Vector2Int(1280, 720));

        audioStreamSender = gameObject.AddComponent<AudioStreamSender>();
        audioStreamSender.audioListener = gameObject.GetComponent<AudioListener>();
        audioStreamSender.source = AudioStreamSource.AudioListener;
        audioStreamSender.sourceDeviceIndex = 0;
        audioStreamSender.autoRequestUserAuthorization = true;

        broadcast = gameObject.AddComponent<Broadcast>();
        broadcast.AddComponent(videoStreamSender);
        broadcast.AddComponent(audioStreamSender);

        // receiveVideoViewer = gameObject.AddComponent<VideoStreamReceiver>();
        // receiveVideoViewer.OnUpdateReceiveTexture += texture => remoteVideoImage.texture = texture;
        //
        // singleConnection = gameObject.AddComponent<SingleConnection>();
        // singleConnection.AddComponent(videoStreamSender);
        // singleConnection.AddComponent(receiveVideoViewer);

        renderstreaming = gameObject.AddComponent<RenderStreamingHandler>();
        renderstreaming.AddSignalingHandler(broadcast);
        renderstreaming.settings = RenderStreaming.GetSignalingSettings<WebSocketSignalingSettings>();
        renderstreaming.Run();
    }

    private void Update()
    {
        // if (Keyboard.current.aKey.isPressed)
        // {
        //     if (renderstreaming.HandlerBases.Contains(singleConnection))
        //     {
        //         return;
        //     }
        //
        //     renderstreaming.AddSignalingHandler(singleConnection);
        //     singleConnection.CreateConnection("12345");
        // }
        //
        // if (Keyboard.current.rKey.isPressed)
        // {
        //     if (!renderstreaming.HandlerBases.Contains(singleConnection))
        //     {
        //         return;
        //     }
        //
        //     singleConnection.DeleteConnection("12345");
        //     renderstreaming.RemoveSignalingHandler(singleConnection);
        // }

        if (Keyboard.current.sKey.isPressed)
        {
            if (videoStreamSender.source == VideoStreamSource.Screen)
            {
                return;
            }

            videoStreamSender.source = VideoStreamSource.Screen;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            if (videoStreamSender.source == VideoStreamSource.WebCamera)
            {
                return;
            }

            videoStreamSender.source = VideoStreamSource.WebCamera;
        }

        if (Keyboard.current.tKey.isPressed)
        {
            if (videoStreamSender.source == VideoStreamSource.Texture)
            {
                return;
            }

            videoStreamSender.source = VideoStreamSource.Texture;
        }

        if (Keyboard.current.cKey.isPressed)
        {
            if (videoStreamSender.source == VideoStreamSource.Camera)
            {
                return;
            }

            videoStreamSender.source = VideoStreamSource.Camera;
        }

        if (Keyboard.current.mKey.isPressed)
        {
            if (audioStreamSender.source == AudioStreamSource.Microphone)
            {
                return;
            }

            audioStreamSender.source = AudioStreamSource.Microphone;
        }

        if (Keyboard.current.lKey.isPressed)
        {
            if (audioStreamSender.source == AudioStreamSource.AudioListener)
            {
                return;
            }

            audioStreamSender.source = AudioStreamSource.AudioListener;
        }
    }
}
