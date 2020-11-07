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
        public List<ChangesTreeItem> Items { get; private set; } = new List<ChangesTreeItem>();
        IEnumerable<ChangesTreeDirectoryItem> Dirs { get { return Items.Where(item => item is ChangesTreeDirectoryItem).Cast<ChangesTreeDirectoryItem>(); } }
        public override bool IsChecked { get { return base.IsChecked; } set { base.IsChecked = value; PropagateDown(value); } }
        bool ListenCheckedEvents { get; set; } = true;

        public void InsertItem(string path, FileStatus status)
        {
            InsertItem(path.Split(new char[] { Path.DirectorySeparatorChar, '/' }).ToList(), status);
        }

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

        public void InsertItem(List<string> path, FileStatus status)
        {
            string first = path.First();
            path.RemoveAt(0);
            if (path.Any())
                InsertDirectoryItem(first, path, status);
            else
                InsertFileItem(first, status);
        }

        void InsertDirectoryItem(string dirName, List<string> restPath, FileStatus status)
        {
            ChangesTreeDirectoryItem d = Dirs.ToList().Find(dir => dir.Name == dirName);
            if (d == null)
                Items.Add(d = new ChangesTreeDirectoryItem() { Name = dirName });
            d.Checked += SubItemChecked;
            d.Unchecked += SubItemUnchecked;
            d.InsertItem(restPath, status);
        }

        void InsertFileItem(string name, FileStatus status)
        {
            ChangesTreeFileItem i = new ChangesTreeFileItem() { Name = name };
            i.Unchecked += SubItemUnchecked;
            i.Checked += SubItemChecked;
            Items.Add(i);
        }

        private void SubItemUnchecked()
        {
            if (IsChecked && ListenCheckedEvents)
                base.IsChecked = false;
        }

        private void SubItemChecked()
        {
            if (!IsChecked && Items.All(item => item.IsChecked) && ListenCheckedEvents)
                IsChecked = true;
        }

        void PropagateDown(bool val)
        {
            ListenCheckedEvents = false;
            Items.ForEach(x => x.IsChecked = val);
            ListenCheckedEvents = true;
        }
    }
}
