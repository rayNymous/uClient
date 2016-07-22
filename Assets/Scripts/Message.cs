using System;
using UnityEngine;
using System.Collections;
using System.IO;

public class Message {

    public static int POCKETSIZELEN=2; //for 2 bytes ushort length type
 //   public static int POCKETSIZELEN = 4; // for uint32 4 bytes type 

    public ushort Length { get; set; }
    public byte[] Content { get; set; }

    public Message(byte[] data)
    {
        Content = data;
        Length = (ushort)data.Length;
    }

    public static Message ReadFromStream(BinaryReader reader)
    {
        ushort len;
        byte[] len_buf;
        byte[] buffer;

        len_buf = reader.ReadBytes(POCKETSIZELEN);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(len_buf);
        }
        len = BitConverter.ToUInt16(len_buf, 0);
        buffer = reader.ReadBytes(len);

        return new Message(buffer);
    }

    public void WriteToStream(BinaryWriter writer)
    {
        byte[] len_bytes = BitConverter.GetBytes(Length);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(len_bytes);
        }
        writer.Write(len_bytes);
        writer.Write(Content);
    }

    public static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }
}
