using System;
using System.Collections.Generic;
using System.Text;

namespace Recrypt.Core
{
    public static class Byter
    {
        /// <summary>
        /// Converts string to byte array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] FromString(in string source, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(source);
        }
    }
}
