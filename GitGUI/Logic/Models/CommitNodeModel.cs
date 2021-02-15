﻿using System;
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

namespace GitGUI.Logic
{
    public class CommitNodeModel : GraphItemModel
    {
        public Commit Commit { get; private set; }
        string _path;
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
            get { return Path != null && ((App)Application.Current).Settings.ShowAuthorMiniatures; }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged("Path"); }
        }

        public CommitNodeModel(Commit c, string picturePath)
        {
            Commit = c;
            Path = picturePath;
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
