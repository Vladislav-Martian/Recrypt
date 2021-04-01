using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using Recrypt.Core;

namespace Recryptor
{
    class Program
    {
        public const string Version = "1.0.0-Alpha";

        public static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<XorOptions, StoreOptions, RestoreOptions>(args);
            parserResult
            .WithParsed<XorOptions>(HandleXorCall)
            .WithParsed<StoreOptions>(HandleStoreCall)
            .WithParsed<RestoreOptions>(HandleRestoreCall)
            .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        private static void HandleXorCall(XorOptions opts)
        {
            var finfo = new FileInfo(opts.FilePath);
            if (!finfo.Exists)
            {
                throw new IOException("File doesn`t exists.");
            }
            using (var file = finfo.Open(FileMode.Open, FileAccess.ReadWrite))
            {
                CryptoEngine.Recrypt(file, Byter.GetBytes(opts.Password));
            }
            if (opts.Rename != null)
            {
                finfo.MoveTo(Path.Combine(finfo.DirectoryName, opts.Rename + finfo.Extension));
            }
        }

        private static void HandleStoreCall(StoreOptions opts)
        {
            var finfo = new FileInfo(opts.FilePath);
            if (!finfo.Exists)
            {
                throw new IOException("File doesn`t exists.");
            }
            var fm = FileModel.Scan(opts.FilePath);
            if (opts.Rename != null)
            {
                fm = fm.EncryptAndPack(Byter.GetBytes(opts.Password), opts.Rename);
            }
            else
            {
                fm = fm.EncryptAndPack(Byter.GetBytes(opts.Password), fm.Name);
            }
            fm.Save(finfo.DirectoryName);
            if (opts.Delete)
            {
                finfo.Delete();
            }
        }

        private static void HandleRestoreCall(RestoreOptions opts)
        {
            var finfo = new FileInfo(opts.FilePath);
            if (!finfo.Exists)
            {
                throw new IOException("File doesn`t exists.");
            }
            var fm = FileModel.Scan(opts.FilePath);
            fm = fm.DecryptAndUnpack(Byter.GetBytes(opts.Password));
            if (opts.Rename != null)
            {
                fm.Save(finfo.DirectoryName, opts.Rename);
            }
            else
            {
                fm.Save(finfo.DirectoryName, fm.Name);
            }
            if (opts.Delete)
            {
                finfo.Delete();
            }
        }

        //==================================================//
        // Help text
        static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "Recryptor " + Version;
                h.Copyright = "Copyright (c) 2021 Recryptor";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }
    }
}
