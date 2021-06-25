using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPosition;

    public Packet()
    {
        buffer = new List<byte>();
        readPosition = 0;
    }

    public Packet(int id)
    {
        buffer = new List<byte>();
        readPosition = 0;

        Write(id);
    }

    public Packet(byte[] data)
    {
        buffer = new List<byte>();
        readPosition = 0;

        SetBytes(data);
    }

    #region Functions
    public void SetBytes(byte[] data)
    {
        Write(data);
        readableBuffer = buffer.ToArray();
    }

    public void WriteLength()
    {
        buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
    }

    public void InsertInt(int value)
    {
        buffer.InsertRange(0, BitConverter.GetBytes(value));
    }

    public byte[] ToArray()
    {
        readableBuffer = buffer.ToArray();
        return readableBuffer;
    }

    public int Length()
    {
        return buffer.Count;
    }

    public int UnreadLength()
    {
        return Length() - readPosition;
    }

    public void Reset(bool shouldReset = true)
    {
        if (shouldReset)
        {
            buffer.Clear();
            readableBuffer = null;
            readPosition = 0;
        }
        else
        {
            readPosition -= 4;
        }
    }
    #endregion

    #region Write Data
    public void Write(byte value)
    {
        buffer.Add(value);
    }

    public void Write(byte[] value)
    {
        buffer.AddRange(value);
    }

    public void Write(short value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(int value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(long value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(float value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(bool value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    public void Write(string value)
    {
        Write(value.Length);
        buffer.AddRange(Encoding.ASCII.GetBytes(value));
    }

    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }

    public void Write(Quaternion value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
        Write(value.w);
    }
    #endregion

    #region Read Data
    public byte ReadByte(bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {
            byte value = readableBuffer[readPosition];
            if (moveReadPos)
            {
                readPosition += 1;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'byte'!");
        }
    }

    public byte[] ReadBytes(int length, bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {
            byte[] value = buffer.GetRange(readPosition, length).ToArray();
            if (moveReadPos)
            {
                readPosition += length;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'byte[]'!");
        }
    }

    public short ReadShort(bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {
            short value = BitConverter.ToInt16(readableBuffer, readPosition);
            if (moveReadPos)
            {
                readPosition += 2;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'short'!");
        }
    }

    public int ReadInt(bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {
            int value = BitConverter.ToInt32(readableBuffer, readPosition);
            if (moveReadPos)
            {
                readPosition += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'int'!");
        }
    }

    public long ReadLong(bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {

            long value = BitConverter.ToInt64(readableBuffer, readPosition);
            if (moveReadPos)
            {
                readPosition += 8;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'long'!");
        }
    }

    public float ReadFloat(bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {
            float value = BitConverter.ToSingle(readableBuffer, readPosition);
            if (moveReadPos)
            {
                readPosition += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'float'!");
        }
    }

    public bool ReadBool(bool moveReadPos = true)
    {
        if (buffer.Count > readPosition)
        {
            bool value = BitConverter.ToBoolean(readableBuffer, readPosition);
            if (moveReadPos)
            {
                readPosition += 1;
            }
            return value;
        }
        else
        {
            throw new Exception("Could not read value of type 'bool'!");
        }
    }

    public string ReadString(bool moveReadPos = true)
    {
        try
        {
            int length = ReadInt();
            string value = Encoding.ASCII.GetString(readableBuffer, readPosition, length);
            if (moveReadPos && value.Length > 0)
            {
                readPosition += length;
            }
            return value;
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    public Vector3 ReadVector3(bool moveReadPos = true)
    {
        return new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
    }

    public Quaternion ReadQuaternion(bool moveReadPos = true)
    {
        return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
    }
    #endregion

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPosition = 0;
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}