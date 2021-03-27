using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitGUI.Logic
{
    class StashingManager
    {
        public StashMenuModel StashMenu { get; set; } = new StashMenuModel();
        string _dirPath = "ImplicitStashes";
        StashCollection Stashes { get { return Repository?.Stashes; } }
        Repository Repository { get; set; }
        public IEnumerable<Tuple<string, string>> ManualStashNames
        {
            get { return Stashes?.Where(s => !ImplicitStashesShas.Contains(s.Reference.TargetIdentifier))
                                .Select(s => new Tuple<string, string>(s.FriendlyName + ": " + s.Message, s.Reference.TargetIdentifier)); }
        }
        public IEnumerable<Commit> ImplicitStashBases
        {
            get { return Stashes?.Where(s => ImplicitStashesShas.Contains(s.Reference.TargetIdentifier))
                                .Select(s => s.Base)
                                .ToList(); }
        }
        List<string> ImplicitStashesShas { get; } = new List<string>();

        public StashingManager()
        {
            LibGitService.GetInstance().RepositoryChanged += UpdateStashWindow;
            LibGitService.GetInstance().BranchChanged += ImplicitPop;
            StashMenu.StashApplyed += Apply;
            StashMenu.StashDeleted += RemoveStash;
            StashMenu.StashPopped += Pop;
        }

        void UpdateStashWindow()
        {
            StashMenu.Stashes = ManualStashNames;
        }

        public void SetRepository(Repository r)
        {
            Repository = r;
            if (r != null)
                SetImplicitStashes();
        }

        void SetImplicitStashes()
        {
            Directory.CreateDirectory(_dirPath);
            string logPath = FindCurrentRepositoryStashLog();
            if (logPath == null)
                CreateLog();
            else
                LoadImplicitStashesShas(logPath);
        }

        void CreateLog()
        {
            string nextName = FindNextName();
            string dir = _dirPath + Path.DirectorySeparatorChar + nextName;
            Directory.CreateDirectory(dir);
            string repo = dir + Path.DirectorySeparatorChar + "Repo";
            using (StreamWriter r = new StreamWriter(repo))
            {
                r.WriteLine(Repository.Info.Path);
            }
            File.Create(dir + Path.DirectorySeparatorChar + "Stashes");
        }

        string FindNextName()
        {
            int name = 0;
            while (File.Exists(_dirPath + Path.DirectorySeparatorChar + name.ToString()))
                name++;
            return name.ToString();
        }

        void LoadImplicitStashesShas(string path)
        { 
            using (StreamReader r = new StreamReader(path + Path.DirectorySeparatorChar + "Stashes"))
            {
                while(!r.EndOfStream)
                {
                    ImplicitStashesShas.Add(r.ReadLine());
                }
            }
        }

        string FindCurrentRepositoryStashLog()
        {
            return Directory.GetDirectories(_dirPath).ToList().Find(IsCurrentRepositoryStashLog);
        }

        bool IsCurrentRepositoryStashLog(string path)
        {
            string repoNamePath = path + Path.DirectorySeparatorChar + "Repo";
            string repoName;
            using (StreamReader r = new StreamReader(repoNamePath))
            {
                repoName = r.ReadLine();
            }
            return repoName == Repository.Info.Path;
        }

        int Index(string sha)
        {
            return Stashes.ToList().FindIndex(s => s.Reference.TargetIdentifier == sha);
        }

        public void ImplicitPush(GraphItemModel checkouted)
        {
            Branch b1 = Repository.Head;
            string str = Repository.Head.Reference.ResolveToDirectReference().TargetIdentifier;
            if (checkouted is BranchLabelModel && ((BranchLabelModel)checkouted).Branch.Reference.CanonicalName == Repository.Head.CanonicalName)
                return;
            string descr = checkouted is BranchLabelModel ? "branch " + ((BranchLabelModel)checkouted).Name : "commit" + ((CommitNodeModel)checkouted).Commit.Sha;
            string message = "Implicit stash before checkout " + descr + ".";
            Stash s = Push(message);
            if (s != null)
                LogStash(s.Reference.TargetIdentifier);
        }

        void LogStash(string sha)
        {
            if (ImplicitStashesShas.Contains(sha))
                return;
            ImplicitStashesShas.Add(sha);
            string logPath = FindCurrentRepositoryStashLog();
            logPath += Path.DirectorySeparatorChar + "Stashes";
            using (StreamWriter r = new StreamWriter(logPath, true))
            {
                r.WriteLine(sha);
            }
        }

        void UnlogStash(string sha)
        {
            ImplicitStashesShas.Remove(sha);
            string logPath = FindCurrentRepositoryStashLog();
            logPath += Path.DirectorySeparatorChar + "Stashes";
            List<string> file = File.ReadAllLines(logPath).ToList();
            file.RemoveAll(s => s == sha);
            File.WriteAllLines(logPath, file.ToArray());
        }

        public void RemoveStash(string sha)
        {
            int index = Index(sha);
            Stashes.Remove(index);
            if (ImplicitStashesShas.Contains(sha))
                UnlogStash(sha);
        }

        public void ImplicitPop()
        {
            if (!ImplicitStashBases.Contains(Repository.Head.Tip))
                return;
            int ind = Stashes.ToList().FindIndex(s => s.Base == Repository.Head.Tip);
            Pop(Stashes[ind].Reference.TargetIdentifier);
        }

        public void Pop(string sha)
        {
            int index = Index(sha);
            Stashes.Pop(index);
            if (ImplicitStashesShas.Contains(sha))
                UnlogStash(sha);
        }

        public void Apply(string sha)
        {
            int index = Index(sha);
            Stashes.Apply(index);
        }

        public void PopLast()
        {
            Stashes.Pop(0);
        }

        public void Push()
        {
            LibGitService.GetInstance().Stash();
        }

        Stash Push(string message)
        {
            return LibGitService.GetInstance().Stash(message);
        }
    }
}
