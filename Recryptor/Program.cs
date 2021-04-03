using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Text;

namespace Recryptor
{
    class Program
    {
        public const string Version = "1.0.0-Alpha";

        public static void Main(string[] args)
        {
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<
                XorOptions,
                StoreOptions,
                RestoreOptions,
                AlterXorOptions,
                AlterStoreOptions,
                AlterRestoreOptions>(args);
            parserResult
            .WithParsed<XorOptions>(HandleXorCall)
            .WithParsed<StoreOptions>(HandleStoreCall)
            .WithParsed<RestoreOptions>(HandleRestoreCall)
            .WithParsed<AlterXorOptions>(HandleAlterXorCall)
            .WithParsed<AlterStoreOptions>(HandleAlterStoreCall)
            .WithParsed<AlterRestoreOptions>(HandleAlterRestoreCall)
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
                Engine.Recrypt(file, Encoding.UTF8.GetBytes(opts.Password));
            }
            if (opts.Rename != null)
            {
                finfo.MoveTo(Path.Combine(finfo.DirectoryName, opts.Rename + finfo.Extension));
            }
        }

        private static void HandleAlterXorCall(AlterXorOptions opts)
        {
            var finfo = new FileInfo(opts.FilePath);
            if (!finfo.Exists)
            {
                throw new IOException("File doesn`t exists.");
            }
            var kinfo = new FileInfo(opts.Password);
            if (!kinfo.Exists)
            {
                throw new IOException("Key file doesn`t exists.");
            }
            using (var file = finfo.Open(FileMode.Open, FileAccess.ReadWrite))
            using (var kfile = kinfo.Open(FileMode.Open, FileAccess.Read))
            {
                byte[] byteskey = new byte[kfile.Length];
                kfile.Read(byteskey);
                Engine.Recrypt(file, byteskey);
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
                fm = fm.EncryptAndPack(Encoding.UTF8.GetBytes(opts.Password), opts.Rename);
            }
            else
            {
                fm = fm.EncryptAndPack(Encoding.UTF8.GetBytes(opts.Password), fm.Name);
            }
            fm.Save(finfo.DirectoryName);
            if (opts.Delete)
            {
                finfo.Delete();
            }
        }

        private static void HandleAlterStoreCall(AlterStoreOptions opts)
        {
            var finfo = new FileInfo(opts.FilePath);
            if (!finfo.Exists)
            {
                throw new IOException("File doesn`t exists.");
            }
            var fm = FileModel.Scan(opts.FilePath);
            using (var key = File.OpenRead(opts.Password))
            {
                Engine.ExtractBytes(key);
                fm = fm.EncryptAndPack(Engine.ExtractBytes(key),
                    opts.Rename ?? fm.Name);
            }
            try
            {
                fm.Save(finfo.DirectoryName);
                if (opts.Delete) finfo.Delete(); // delete file only if saving successful
            }
            catch (Exception ex)
            {
                Console.WriteLine("Canceled:");
                Console.WriteLine(ex);
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
            fm = fm.DecryptAndUnpack(Encoding.UTF8.GetBytes(opts.Password));
            try
            {
                fm.Save(
                    finfo.DirectoryName,
                    opts.Rename ?? null
                );
                if (opts.Delete) finfo.Delete(); // delete file only if saving successful
            }
            catch (Exception ex)
            {
                Console.WriteLine("Canceled:");
                Console.WriteLine(ex);
            }
        }

        private static void HandleAlterRestoreCall(AlterRestoreOptions opts)
        {
            var finfo = new FileInfo(opts.FilePath);
            if (!finfo.Exists) throw new IOException("File doesn`t exists."); // test file existing
            var fm = FileModel.Scan(finfo.FullName);
            using (var key = File.OpenRead(opts.Password))
            {
                Engine.ExtractBytes(key);
                fm = fm.DecryptAndUnpack(Engine.ExtractBytes(key));
            }
            try
            {
                fm.Save(
                    finfo.DirectoryName,
                    opts.Rename ?? null
                );
                if (opts.Delete) finfo.Delete(); // delete file only if saving successful
            }
            catch (Exception ex)
            {
                Console.WriteLine("Canceled:");
                Console.WriteLine(ex);
            }
        }

        //==================================================//
        // Help text
        static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = true;
                h.Heading = "Recryptor " + Version;
                h.Copyright = "Copyright (c) 2021 Recryptor";
                h.AddVerbs(
                    typeof(XorOptions),             // Encrypt-Decrypt basic by string key
                    typeof(StoreOptions),           // Encrypt advanced to .crypto file by string key
                    typeof(RestoreOptions),         // Decrypt advanced from .crypto by string key
                    typeof(AlterXorOptions),        // Encrypt-Decrypt basic with file key
                    typeof(AlterStoreOptions),      // Encrypt advanced to .crypto file by file key
                    typeof(AlterRestoreOptions)     // Decrypt advanced from .crypto by file key
                    );
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }
    }
}
