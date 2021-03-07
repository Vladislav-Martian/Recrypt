using System;
using Recrypt.Core;

namespace EXP
{
    class Program
    {
        static void Main(string[] args)
        {
            var adrfolder = @"C:\Users\Vlad Martian\Desktop";
            var adrfile = @"C:\Users\Vlad Martian\Desktop\EXP.exe";
            var model = FileModel.Scan(adrfile);
            Console.WriteLine(model.FullName);
            var newmodel = model.EncryptAndPack(Byter.GetBytes("password"), model.Name);
            newmodel.Save(adrfolder);
            var restored = newmodel.DecryptAndUnpack(Byter.GetBytes("password"));
            Console.WriteLine(restored);
            Console.WriteLine(restored.FullName);
            Console.WriteLine(restored.Content);
            restored.Save(adrfolder, restored.Name + "Restored");
            Console.ReadLine();
        }
    }
}
