using System;
using System.Collections.Generic;
using System.Text;

namespace Recrypt.Core
{
    /// <summary>
    /// Very simple interface with 1 method:
    /// <para>
    /// byte[] GetBytes();
    /// </para>
    /// </summary>
    public interface IBytes
    {
        public byte[] GetBytes();
    }
    /// <summary>
    /// Static class with functions that converts values to byte arrays.
    /// </summary>
    public static class Byter
    {
        /// <summary>
        /// Converts IBytes instance to byte[].
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] GetBytes(in IBytes o)
        {
            return o.GetBytes();
        }
        /// <summary>
        /// Converts string to byte array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] GetBytes(in string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(source);
        }
    }
}
