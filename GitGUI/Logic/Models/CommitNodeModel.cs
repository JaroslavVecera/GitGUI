using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using LibGit2Sharp;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace GitGUI.Logic
{
    public class CommitNodeModel : GraphItemModel
    {
        public Commit Commit { get; private set; }
        BitmapImage _picture;
        public string Message { get { return Commit.Message; } }
        public string Sha { get { return Commit.Sha; } }
        public string Author {  get { return Commit.Author.Name; } }
        public event Action<CommitNodeModel> CopyShaRequested;
        public event Action<CommitNodeModel> ShowChanges;
        public RelayCommand CopySha { get; private set; }
        public RelayCommand OnShowChanges { get; private set; }

        public event Action<CommitNodeModel> AddBranch;

        public bool EnabledPhoto
        {
            get { return BitmapImage != null && ((App)Application.Current).Settings.ShowAuthorMiniatures; }
        }

        public BitmapImage BitmapImage
        {
            get { return _picture; }
            set { _picture = value; OnPropertyChanged("BitmapImage"); }
        }

        public CommitNodeModel(Commit c, BitmapImage picture)
        {
            Commit = c;
            BitmapImage = picture;
            CopySha = new RelayCommand(() => CopyShaRequested?.Invoke(this));
            OnShowChanges = new RelayCommand(
                () => 
                ShowChanges?.Invoke(this));
        }

        public void OnAddBranch()
        {
            AddBranch?.Invoke(this);
        }
    }
}
