using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Recrypt.Core
{
    public static class CryptoEngine
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
        /// Encrypt string with bytes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in string source, in byte[] key)
        {
            return RecryptInPlace(Byter.GetBytes(source), key);
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
        /// Encrypt string with string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in string source, in string key)
        {
            return RecryptInPlace(Byter.GetBytes(source), Byter.GetBytes(key));
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
        /// <summary>
        /// Works with IBytes instances.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in IBytes source, in IBytes key)
        {
            return RecryptInPlace(source.GetBytes(), key.GetBytes());
        }
        /// <summary>
        /// Works with IBytes instances.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in IBytes source, in byte[] key)
        {
            return RecryptInPlace(source.GetBytes(), key);
        }
        /// <summary>
        /// Works with IBytes instances.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in byte[] source, in IBytes key)
        {
            return RecryptInPlace(source, key.GetBytes());
        }
        /// <summary>
        /// Works with string keys in UTF-8.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] Recrypt(in IBytes source, in string key)
        {
            return RecryptInPlace(source.GetBytes(), Encoding.UTF8.GetBytes(key));
        }
        //==================================================//


        //==================================================//
    }
}
