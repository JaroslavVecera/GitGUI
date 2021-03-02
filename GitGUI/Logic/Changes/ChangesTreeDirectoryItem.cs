using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitGUI.Logic
{
    public class ChangesTreeDirectoryItem : ChangesTreeItem
    {
        public event Action SubItemCheckedChanged;
        public List<ChangesTreeItem> Items { get; private set; } = new List<ChangesTreeItem>();
        IEnumerable<ChangesTreeDirectoryItem> Dirs { get { return Items.Where(item => item is ChangesTreeDirectoryItem).Cast<ChangesTreeDirectoryItem>(); } }
        public override bool IsChecked { get { return base.IsChecked; } set { base.IsChecked = value; PropagateDown(value); } }
        bool ListenCheckedEvents { get; set; } = true;

        public override IEnumerable<string> GetCheckedPaths(string prefix)
        {
            string newPref = prefix == "" ? Name : prefix + '/' + Name;
            if (IsChecked)
                return new List<string>() { newPref };
            IEnumerable<string> res = new List<string>();
            foreach (ChangesTreeItem it in Items)
            {
                res = res.Union(it.GetCheckedPaths(newPref));
            }
            return res;
        }

        public override IEnumerable<string> GetUncheckedPaths(string prefix)
        {
            string newPref = prefix == "" ? Name : prefix + '/' + Name;
            if (!IsChecked)
                return new List<string>() { newPref };
            IEnumerable<string> res = new List<string>();
            foreach (ChangesTreeItem it in Items)
            {
                res = res.Union(it.GetUncheckedPaths(newPref));
            }
            return res;
        }

        public void InsertItem(string path, ChangesInfo info, bool staged)
        {
            InsertItem(path.Split(new char[] { Path.DirectorySeparatorChar, '/' }).ToList(), info, staged);
        }

        public void InsertItem(List<string> path, ChangesInfo info, bool staged)
        {
            string first = path.First();
            path.RemoveAt(0);
            if (path.Any())
                InsertDirectoryItem(first, path, info, staged);
            else
                InsertFileItem(first, info, staged);
        }

        void InsertDirectoryItem(string dirName, List<string> restPath, ChangesInfo info, bool staged)
        {
            ChangesTreeDirectoryItem d = Dirs.ToList().Find(dir => dir.Name == dirName);
            if (d == null)
                Items.Add(d = new ChangesTreeDirectoryItem() { Name = dirName });
            d.Checked += SubItemChecked;
            d.Unchecked += SubItemUnchecked;
            d.SubItemCheckedChanged += NotifyChange;
            d.InsertItem(restPath, info, staged);
        }

        void InsertFileItem(string name, ChangesInfo info, bool staged)
        {
            ChangesTreeFileItem it = new ChangesTreeFileItem() { Name = name };
            it.Unchecked += SubItemUnchecked;
            it.Checked += SubItemChecked;
            it.Info = info;
            it.IsChecked = staged;
            Items.Add(it);
        }

        private void SubItemUnchecked()
        {
            if (IsChecked && ListenCheckedEvents)
                base.IsChecked = false;
            else
                NotifyChange();
        }

        private void SubItemChecked()
        {
            if (!IsChecked && Items.All(item => item.IsChecked) && ListenCheckedEvents)
                IsChecked = true;
            else
                NotifyChange();
        }

        void NotifyChange()
        {
            SubItemCheckedChanged?.Invoke();
        }

        void PropagateDown(bool val)
        {
            ListenCheckedEvents = false;
            Items.ForEach(x => x.IsChecked = val);
            ListenCheckedEvents = true;
        }
    }
}
