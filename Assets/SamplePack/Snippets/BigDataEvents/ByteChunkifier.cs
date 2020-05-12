
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Helper class to split and recombine a byte array into chunks
/// </summary>
public class ByteChunkifier
{
    public int ChunkSize { get { return _chunkSize; } }
    public int Size { get { return _array.Length; } }
    public byte[] FullArray { get { return _array; } }

    private byte[] _array;
    private int _chunkSize;
    private int _byteIndex;
    private int _bytesLeft;

    /// <summary>
    /// Start a byte chunkifier to read out chunks
    /// </summary>
    /// <param name="array"></param>
    /// <param name="chunkSize"></param>
    public ByteChunkifier(byte[] array, int chunkSize)
    {
        _array = array;
        _chunkSize = chunkSize;
        _byteIndex = 0;
        _bytesLeft = _array.Length;
    }

    /// <summary>
    /// Start a byte chunkifier to reassemble chunks
    /// </summary>
    /// <param name="totalSize"></param>
    public ByteChunkifier(int totalSize, int chunkSize)
    {
        _array = new byte[totalSize];
        _chunkSize = chunkSize;
    }

    public bool ReadChunk(out byte[] buffer)
    {
        buffer = null;
		if (!HasBytesLeft())
            return false; // end of stream

        int bytesToSend = Mathf.Min(_bytesLeft, _chunkSize);
        buffer = new byte[bytesToSend];
        Array.Copy(_array, _byteIndex, buffer, 0, bytesToSend);

        _byteIndex += bytesToSend;
        _bytesLeft -= bytesToSend;
        return true;
    }

	public bool HasBytesLeft()
	{
		return _bytesLeft > 0;
	}

    public void WriteChunk(byte[] buffer, int index)
    {
        int offset = index * ChunkSize;
        Array.Copy(buffer, 0, _array, offset, buffer.Length);
    }

}
