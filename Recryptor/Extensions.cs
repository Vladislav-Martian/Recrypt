using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Recryptor
{
    public static class Extensions
    {
        public static string GetNameOnly(this FileInfo source)
        {
            return Path.GetFileNameWithoutExtension(source.FullName);
        }
    }
}
