using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Recryptor
{
    [Serializable]
    public class FileModel
    {
        public string Extension { get; private set; }
        public string Name { get; private set; }
        public byte[] Content { get; private set; }
        private static BinaryFormatter formatter = new BinaryFormatter();

        public FileModel(in byte[] content, in string name = "Unnamed", in string extension = ".bin")
        {
            Extension = extension;
            Name = name;
            Content = content;
        }
        //==================================================//
        public string FullName
        {
            get => Name + Extension;
        }

        public byte[] GetBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, this);
                return ms.ToArray();
            }
        }
        //==================================================//
        public static FileModel Scan(in string adr)
        {
            var fi = new FileInfo(adr);
            if (!fi.Exists)
            {
                throw new IOException("File doesn`t exist");
            }
            var content = new byte[fi.Length];
            using (var fs = fi.OpenRead())
            {
                fs.Read(content);
            }
            return new FileModel(content, Path.GetFileNameWithoutExtension(fi.Name), fi.Extension);
        }

        public static void Save(in FileModel model, in string adr, in string rename = null)
        {
            var di = new DirectoryInfo(adr);
            if (!di.Exists)
            {
                di.Create();
            }
            if (rename != null) model.Name = rename;
            var adress = adr + "\\" + model.FullName;
            using (var fs = new FileStream(adress, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(model.Content);
            }
        }
        //==================================================//
        public void Save(in string adr)
        {
            FileModel.Save(this, adr);
        }
        public void Save(in string adr, in string rename)
        {
            FileModel.Save(this, adr, rename);
        }
        public void Recrypt(in byte[] key)
        {
            Engine.RecryptInPlace(Content, key);
            Name = Encoding.UTF8.GetString(Engine.RecryptInPlace(Encoding.UTF8.GetBytes(Name), key));
            Extension = Encoding.UTF8.GetString(Engine.RecryptInPlace(Encoding.UTF8.GetBytes(Extension), key));
        }

        public FileModel EncryptAndPack(in byte[] key, in string name = "Unnamed")
        {
            Recrypt(key);
            return new FileModel(GetBytes(), name, ".crypto");
        }

        public FileModel DecryptAndUnpack(in byte[] key)
        {
            var ms = new MemoryStream(Content);
            ms.Position = 0;
            var res = (FileModel)formatter.Deserialize(ms);
            res.Recrypt(key);
            return res;
        }
    }
}
