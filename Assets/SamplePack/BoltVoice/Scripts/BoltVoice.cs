using UnityEngine;
using System.Collections;
using VoiceChat;
public class BoltVoice : Bolt.GlobalEventListener
{
    [SerializeField]
    private VoiceChatPlayer player;


    // Use this for initialization
    void Start()
    {
        VoiceChatRecorder.Instance.NewSample += OnNewSample;

        player = gameObject.AddComponent<VoiceChatPlayer>();

        VoiceChatRecorder.Instance.StartRecording();
    }

    void OnNewSample(VoiceChatPacket packet)
    {
        var evnt = VoiceEvent.Create(Bolt.GlobalTargets.Everyone, Bolt.ReliabilityModes.Unreliable);
        evnt.length = packet.Length;
        evnt.NetworkId = packet.NetworkId;
        evnt.packetId = (int)packet.PacketId;
        evnt.BinaryData = packet.Data;
        evnt.Send();       
    }

    public override void OnEvent(VoiceEvent evnt)
    {
        if (evnt.FromSelf == false)
        {
            VoiceChatPacket vcp;
            vcp.Compression = VoiceChatCompression.Speex;
            vcp.Data = evnt.BinaryData;
            vcp.Length = evnt.length;
            vcp.NetworkId = evnt.NetworkId;
            vcp.PacketId = (ulong)evnt.packetId;
            player.OnNewSample(vcp);
        }
    }


}
