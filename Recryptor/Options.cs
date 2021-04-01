using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recryptor
{
    /// <summary>
    /// <para>
    /// Class represents options if program called to encrypt file by simplest way, saving its name and extension but with obfuscated binary data. Second call decrypts file back. File size do not changes.
    /// </para>
    /// </summary>
    [Verb("xor", HelpText = "Transform byte representation of file by xor-encryption. " +
        "If you use one key twice file will be encrypted end recrypted. Don`t forget key")]
    class XorOptions
    {
        [Option('f', "file", Required = true, HelpText = "File adress. Example: c:\\temp\\MyTest.txt")]
        public string FilePath { get; set; }

        [Option('k', "key", Required = true, HelpText = "Key to encrypt file, used as UTF8 string.")]
        public string Password { get; set; }

        [Option('r', "rename", Required = false, Default = null,
            HelpText = "Rename resulting file. By default original file name used.")]
        public string Rename { get; set; }
    }

    /// <summary>
    /// <para>
    /// Class represents options if program called to advanced encryption of file. Obfuscate file name and extension. File size changes. Result extension ".crypto".
    /// </para>
    /// </summary>
    [Verb("store", HelpText = "Transform file to its .crypto version. It practically unpossible" +
        " to restore back file, file original name and extension without keyword.")]
    class StoreOptions
    {
        [Option('f', "file", Required = true, HelpText = "File adreess. Example: c:\\temp\\file.bin")]
        public string FilePath { get; set; }

        [Option('k', "key", Required = true, HelpText = "Key to encrypt file, used as UTF8 string.")]
        public string Password { get; set; }

        [Option('r', "rename", Required = false, Default = null,
            HelpText = "Rename resulting file. By default original file name used.")]
        public string Rename { get; set; }

        [Option('d', "delete", Required = false, Default = (bool) false,
            HelpText = "Delete original file.")]
        public bool Delete { get; set; }
    }
    /// <summary>
    /// <para>
    /// Class represents options if program called to restore stored file. Deobfuscate file name and extension. File size changes. Result extension ".crypto".
    /// </para>
    /// </summary>
    [Verb("restore", HelpText = "Restore file from its .crypto version. It practically unpossible" +
        " to restore back file data, file original name and extension without keyword.")]
    class RestoreOptions
    {
        [Option('f', "file", Required = true, HelpText = "File adreess. Example: c:\\temp\\file.bin")]
        public string FilePath { get; set; }

        [Option('k', "key", Required = true, HelpText = "Key to encrypt file, used as UTF8 string.")]
        public string Password { get; set; }

        [Option('r', "rename", Required = false, Default = null,
            HelpText = "Rename resulting file. By default original file name used.")]
        public string Rename { get; set; }

        [Option('d', "delete", Required = false, Default = (bool)false,
            HelpText = "Delete stored file.")]
        public bool Delete { get; set; }
    }
}
