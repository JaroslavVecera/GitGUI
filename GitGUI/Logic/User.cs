using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GitGUI.Logic
{
    public class User
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        Bitmap Picture { get; set; }
        public LibGit2Sharp.Identity Identity {  get { return new LibGit2Sharp.Identity(Name, Email); } }
        public LibGit2Sharp.Signature UpToDateSignature { get { return new LibGit2Sharp.Signature(Identity, DateTime.Now); } }
        public static User Anonym { get { return new User() { Name = "Anonym" , Email = "-"}; } }
        string Path { get; set; }
        string IdentityPath { get { return Path + System.IO.Path.DirectorySeparatorChar + "Identity"; } }
        string PotentialPicturePath { get { return Path + System.IO.Path.DirectorySeparatorChar + "Picture"; } }
        public string PicturePath
        {
            get { return (File.Exists(PotentialPicturePath)) ? 
                    AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + PotentialPicturePath : null;
            }
        }

        public User(string path, string name,string email, Bitmap picture)
        {
            Path = path;
            Name = name;
            Email = email;
            Picture = picture;
        }

        public User(string path)
        {
            Path = path;
            Load();
        }

        private User() { }

        public bool HasIdentity(LibGit2Sharp.Identity i)
        {
            return i.Name == Name && i.Email == Email;
        }

        void Load()
        {
            CheckAnonym();
            using (StreamReader r = new StreamReader(IdentityPath))
            {
                Name = r.ReadLine();
                Email = r.ReadLine();
            }
        }

        public void Save()
        {
            CheckAnonym();
            using (StreamWriter w = new StreamWriter(IdentityPath))
            {
                w.WriteLine(Name);
                w.WriteLine(Email);
            }
            if (Picture != null)
                Picture.Save(PotentialPicturePath);
        }

        public void Delete()
        {
            Directory.Delete(Path);
        }

        void CheckAnonym()
        {
            if (Path == null)
                throw new InvalidOperationException("Cannot load anonym user");
        }

        public BitmapImage PictureCopy
        {
            get
            {
                string path = PotentialPicturePath;
                if (path == null)
                    return null;
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bit.CacheOption = BitmapCacheOption.OnLoad;
                bit.UriSource = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + path);
                bit.EndInit();
                return bit;
            }
        }
    }
}
