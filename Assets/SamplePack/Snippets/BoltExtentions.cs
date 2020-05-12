//MIT. Do whatever you want.
//@DMeville

// USAGE 
// token.WriteInt(value, 0, 3) 
// compresses the "value" in the range [0, 3], sends as 2 bits.
// make sure you ReadInt with the same range you wrote it, otherwise things break

using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public static class BoltExtensions
{
    public static uint RequiredBits(int minValue, int maxValue)
    {
        //shift min and max into range starting at 0
        //so (-1,5) -> (0,4). (-2, 2) -> (0,4)

        //uint c = (uint)(maxValue - maxValue);
        //uint d = (uint)(maxValue - minValue);

        uint delta = (uint)(maxValue - minValue);

        //I have no idea how this works.  I found it on stackoverflow. ᕕ( ᐛ )ᕗ
        uint x = delta;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        //count the ones
        x -= x >> 1 & 0x55555555;
        x = (x >> 2 & 0x33333333) + (x & 0x33333333);
        x = (x >> 4) + x & 0x0f0f0f0f;
        x += x >> 8;
        x += x >> 16;

        return x & 0x0000003f;
    }

    public static int RequiredBitsInt(int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        return (int)RequiredBits(minValue, maxValue);
    }

    public static int ReadInt(this UdpPacket packet, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        int reqBit = (int)RequiredBits(minValue, maxValue);
        return packet.ReadInt(reqBit) + minValue;
    }

    public static void WriteInt(this UdpPacket packet, int value, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        int reqBit = (int)RequiredBits(minValue, maxValue);
        //Debug.Log("WriteInt: " + value + " : requiredBits : " + reqBit);
        if (value < minValue || value > maxValue)
        {
            Debug.Log($"Value [{value}] is outside of range [{minValue}, {maxValue}] and will be clamped");
        }

        value = Mathf.Clamp(value, minValue, maxValue);
        value = value - minValue;

        packet.WriteInt(value, reqBit);
    }

    public static int RequiredBitsFloat(float minValue = float.MinValue, float maxValue = float.MaxValue, float precision = 0.0000001f)
    {
        int intMax = (int)((maxValue - minValue + precision) * (1f / precision));
        //Debug.Log("intmax: " + intMax);
        return (int)RequiredBitsInt(0, intMax);
    }

    public static float ReadFloat(this UdpPacket packet, float minValue = float.MinValue, float maxValue = float.MaxValue, float precision = 0.0000001f)
    {
        int intMax = (int)((maxValue - minValue + precision) * (1f / precision));
        //Debug.Log("read intMax: " + intMax);
        int intVal = packet.ReadInt(0, intMax);
        float value = (intVal * precision) + minValue;
        //need to clean it up because sometimes we're getting 1.000001 with 0.1 prec, so fix that
        value = Mathf.Round((value) * (1f / precision)) * precision;
        return value;
    }

    ///precision is how many digits after decimal. Max is 7 digits (0.0000001)
    //don't use anything but these values probably: 1.0, 0.1, 0.01, 0.001, 0.0001, 0.00001, 0.000001, 0.0000001
    //don't use 0.2, or anything because idk if that works properly, probably doesn't
    ///Lower precision means fewer bits to send
    public static void WriteFloat(this UdpPacket packet, float value, float minValue = float.MinValue, float maxValue = float.MaxValue, float precision = 0.0000001f)
    {
        //what is our int max value do we have from (min -> max ) with precision
        //[0->1] with 0.1 precision means
        //[0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0]
        //[0,   1,   2,   3,   4,   5,   6,   7,   8,   9,  10]
        //max value is 10!

        //[-1 ->1] with 0.1 prec
        //[-1.0, -0.9, -0.8, -0.7, -0.6, -0.5, -0.4, -0.3, -0.2, -0.1, 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0]
        //lets push it to a pos rage.
        //[ 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0]
        //[   0,   1,   2,   3,   4,   5,   6,   7,   8,   9,  10,  11,  12,  13,  14,  15,  16,  17,  18,  19,  20]
        if (value < minValue || value > maxValue)
        {
            Debug.Log($"Value [{value}] is outside of range [{minValue}, {maxValue}] and will be clamped");
        }
        value = Mathf.Clamp(value, minValue, maxValue);
        //float oneOverPrecision = (1f / precision); would this be faster? probably not even worth it idk
        int intMax = (int)((maxValue - minValue + precision) * (1f / precision)); //10
        //Debug.Log("intMax: " + intMax);
        //don't round, just remove values past precision
        //if we passed in 0.4524, we should use the value 0.4 (precision 0.1)
        //and that should be int value of 4
        bool neg = (value < 0);
        value = value - minValue;
        //Debug.Log("offset value: " + value);
        if (!neg)
        {
            value = Mathf.Floor(value * (1 / precision)) * precision; //converts 0.452 to 0.4.
        }
        else
        {
            value = Mathf.Ceil(value * (1 / precision)) * precision; //converts 0.452 to 0.4.
        }
        //Debug.Log("Rounded value: " + value);
        float intVal = ((value) * (1f / precision)); //1.4 should be 14

        //Debug.Log("IntVal: " + intVal);
        //compress values [0->11]
        packet.WriteInt((int)intVal, 0, intMax);
    }

    //usage: In a Bolt.IProtocolToken call ReadQuaterion(0.1f), or WriteQuaterion(quat, 0.0001f);

    public static Quaternion ReadQuaternion(this UdpPacket packet, float precision = 0.0000001f)
    {
        float x = 0f;
        float y = 0f;
        float z = 0f;
        float w = 0f;


        int largestIndex = packet.ReadInt(0, 3);
        //Debug.Log("read largestIndex: " + largestIndex);
        //largestIndex needs to be calculated
        float a = packet.ReadFloat(-1f, 1f, precision);
        float b = packet.ReadFloat(-1f, 1f, precision);
        float c = packet.ReadFloat(-1f, 1f, precision);

        //packet.WriteFloat()


        float d = Mathf.Sqrt(1f - ((a * a) + (b * b) + (c * c))); //largest

        //Debug.Log(a + " : " + b + " : " + c + " : " + d);

        switch (largestIndex)
        {
            case 0:
                return new Quaternion(d, a, b, c);
                break;
            case 1:
                return new Quaternion(a, d, b, c);
                break;

            case 2:
                return new Quaternion(a, b, d, c);
                break;
            case 3:
                return new Quaternion(a, b, c, d);
                break;
        }

        return new Quaternion(0f, 0f, 0f, 1f);
    }

    //uses smallest three compression in the range of [-1, 1] by default. 
    public static void WriteQuaternion(this UdpPacket packet, Quaternion quaternion, float precision = 0.00000001f)
    {

        //find the index with the largest abs value
        int largestIndex = 0;
        float v = quaternion[0];
        for (int i = 1; i < 4; i++)
        {
            if (Mathf.Abs(quaternion[i]) > v)
            {
                v = quaternion[i];
                largestIndex = i;
            }
        }

        //Debug.Log("largest index write: " + largestIndex + " v: " + v);
        packet.WriteInt(largestIndex, 0, 3);
        float sign = (quaternion[largestIndex] < 0) ? -1 : 1;
        //if(Mathf.Approximately(rotation[largestIndex], 1f)) {
        //    //write one bit as true
        //    //because we know if any component is 1, everything else is 0 so don't bother even sending.
        //    //in *most* cases this is an extra bit as this isn't true often I imagine, so maybe it's not worth it
        //    //to save one bit infrequently, while having to send one bit more often.
        //} else {
        //    //write one bit as false
        //}

        switch (largestIndex)
        {
            case 0:
                packet.WriteFloat(quaternion[1] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[2] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[3] * sign, -1f, 1f, precision);
                break;

            case 1:
                packet.WriteFloat(quaternion[0] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[2] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[3] * sign, -1f, 1f, precision);
                break;

            case 2:
                packet.WriteFloat(quaternion[0] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[1] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[3] * sign, -1f, 1f, precision);
                break;

            case 3:
                packet.WriteFloat(quaternion[0] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[1] * sign, -1f, 1f, precision);
                packet.WriteFloat(quaternion[2] * sign, -1f, 1f, precision);
                break;
        }
    }

    //what is the max float value we can have when bitpacking?
    //eg, if we use 2 bits, with a precision of 0.5, what is the largest value? (Answer: 2, [0, 0.5, 1.5, 2.0])
    public static float MaxValueUsingXBits(int bits, float precision = 0.0000001f)
    {
        int intMax = (int)(Mathf.Pow(2, bits));
        return (intMax * precision) - precision;
    }
}
