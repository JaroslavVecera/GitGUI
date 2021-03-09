﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    class MainWindowViewModel : ViewModelBase
    {
        public StashMenuViewModel StashMenu { get; set; }
        public MainWindowModel Model { get; set; }
        public List<TabViewModel> Tabs { get { return new List<TabViewModel>(Model.Tabs); } }
        public int SelectedIndex { get { return Model.SelectedIndex; } }
        public List<User> Users { get { return Program.GetInstance().UserManager.KnownUsers; } }
        public string RepoPath { get { return Model.RepoPath; } }
        public bool CanClose { get { return RepoPath != null && RepoPath != ""; } }

        public MainWindowViewModel(MainWindowModel model, MainWindow view)
        {
            SubscribeModel(model, view);
            view.DataContext = this;
        }

        void SubscribeModel(MainWindowModel model, MainWindow view)
        {
            Model = model;
            model.ChangedTabs += () => OnPropertyChanged("Tabs");
            model.ChangedIndex += () => OnPropertyChanged("SelectedIndex");
            model.OnContextMenuOpened += () => view.OpenContextMenu();
            model.OnNoAggregationContextMenuOpened += () => view.OpenNoAggregationContextMenu();
            model.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName == "RepoPath")
                OnPropertyChanged("CanClose");
        }
    }
}
