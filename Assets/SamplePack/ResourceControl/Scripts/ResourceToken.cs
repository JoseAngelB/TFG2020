using UnityEngine;
using System.Collections;
using System;


    public class ResourceToken : Bolt.IProtocolToken
    {

    public int Number;

        void Bolt.IProtocolToken.Read(UdpKit.UdpPacket packet)
        {
            Number = packet.ReadInt();
        }

        void Bolt.IProtocolToken.Write(UdpKit.UdpPacket packet)
        {
            packet.WriteInt(Number);
        }
        
    }
