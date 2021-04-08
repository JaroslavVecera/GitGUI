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
        public bool IsEditable { get; private set; } = true;
        public LibGit2Sharp.Identity Identity {  get { return new LibGit2Sharp.Identity(Name, Email); } }
        public LibGit2Sharp.Signature UpToDateSignature { get { return new LibGit2Sharp.Signature(Identity, DateTime.Now); } }
        public static User Anonym { get { return new User() { Name = "Anonym", Email = "-", IsEditable = false }; } }
        string Path { get; set; }
        string IdentityPath { get { return Path + System.IO.Path.DirectorySeparatorChar + "Identity"; } }
        string PotentialPicturePath { get { return Path + System.IO.Path.DirectorySeparatorChar + "Picture"; } }
        public RelayCommand OnEdit { get; private set; }
        public RelayCommand OnDelete { get; private set; }
        public string PicturePath
        {
            get { return (File.Exists(PotentialPicturePath)) ? PotentialPicturePath : null;
            }
        }

        void InitializeCommands()
        {
            OnDelete = new RelayCommand(() => Program.GetInstance().UserManager.DeleteUser(this));
            OnEdit = new RelayCommand(() => Program.GetInstance().UserManager.EditUser(this));
        }

        public User(string path, string name, string email, Bitmap picture)
        {
            Path = path;
            Name = name;
            Email = email;
            Picture = picture;
            Save();
            InitializeCommands();
        }

        public User(string path)
        {
            Path = path;
            Load();
            InitializeCommands();
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
            Directory.CreateDirectory(Path);
            File.Create(IdentityPath).Dispose();
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
            Directory.Delete(Path, true);
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
                string path = PicturePath;
                if (path == null)
                    return null;
                BitmapImage bit = new BitmapImage();
                bit.BeginInit();
                bit.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bit.CacheOption = BitmapCacheOption.OnLoad;
                bit.UriSource = new Uri(path);
                bit.EndInit();
                return bit;
            }
        }

        public Bitmap BitmapCopy
        {
            get
            {
                string path = PicturePath;
                if (path == null)
                    return null;
                return new Bitmap(path);
            }
        }
    }
}
