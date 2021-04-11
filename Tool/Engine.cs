using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Recryptor.Tool
{
    public static class Engine
    {
        /// <summary>
        /// Encrypt/Decrypt byte array with other byte array as key.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns>byte[]: Recrypted byte array (inPlace, same as source array, but values changed)</returns>
        public static byte[] RecryptInPlace(in byte[] source, in byte[] key)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = (byte)(source[i] ^ key[i % key.Length]);
            }
            return source;
        }
        /// <summary>
        /// Encrypt/Decrypt byte array with other byte array as key.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns>byte[]: Recrypted byte array (outPlace, new recrypted array)</returns>
        public static byte[] RecryptOutPlace(in byte[] source, in byte[] key)
        {
            var recrypted = new byte[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                recrypted[i] = (byte)(source[i] ^ key[i % key.Length]);
            }
            return recrypted;
        }
        /// <summary>
        /// Encrypt/Decrypt byte array with other byte array as key.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="inPlace"></param>
        /// <returns>byte[]: recrypted array</returns>
        public static byte[] Recrypt(in byte[] source, in byte[] key, in bool inPlace = true)
        {
            return inPlace ? RecryptInPlace(source, key) : RecryptOutPlace(source, key);
        }
        /// <summary>
        /// Works with string keys in UTF-8.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in byte[] source, in string key)
        {
            return RecryptInPlace(source, Encoding.UTF8.GetBytes(key));
        }

        /// <summary>
        /// Encrypt/Decrypt data in any stream (FileStream, etc...)
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="key"></param>
        public static void Recrypt(in Stream stream, in byte[] key)
        {
            if (!stream.CanRead || !stream.CanWrite)
            {
                throw new Exception("Stream is unavailable for read or write.");
            }
            var recrypted = new byte[stream.Length];
            stream.Read(recrypted);
            RecryptInPlace(recrypted, key);
            stream.SetLength(0);
            stream.Write(recrypted);
        }

        //==================================================//
        // Extract bytes from stream:
        /// <summary>
        /// Get bytes from file stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ExtractBytes(in Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new Exception("Stream is unavailable for read.");
            }
            var bytes = new byte[stream.Length];
            stream.Read(bytes);
            return bytes;
        }

    }

    //==================================================//
    // Class to in flow recrypt
    /// <summary>
    /// Class represents object that encrypts/decrypts item bytes by key: byte[]
    /// </summary>
    public sealed class AutoXOR : IDisposable
    {
        private byte[] key;
        private int stage;
        private readonly int len;
        //==================================================//
        public void Dispose()
        {
            key = null;
        }
        //==================================================//
        public AutoXOR(in byte[] key)
        {
            this.key = key;
            len = key.Length;
            this.stage = -1;
        }
        public AutoXOR(in string key)
        {
            this.key = Encoding.UTF8.GetBytes(key);
            len = key.Length;
            stage = 0;
        }
        //==================================================//
        public byte Apply(in byte source)
        {
            stage++;
            return (byte)(source ^ key[stage % len]);
        }

        public int Apply(in int source)
        {
            stage++;
            return source ^ key[stage % len];
        }

        public byte[] ApplyArrayIn(in byte[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = Apply(source[i]);
            }
            return source;
        }

        public byte[] ApplyArrayOut(in byte[] source)
        {
            var result = new byte[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = Apply(source[i]);
            }
            return result;
        }
    }
}
