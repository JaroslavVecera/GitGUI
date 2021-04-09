using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using LibGit2Sharp.Handlers;
using System.Threading;
using System.Windows.Threading;

namespace GitGUI.Logic
{
    class LibGitNetworkService
    {
        bool First { get; set; }
        bool PushErrors { get; set; } = false;
        static LibGitNetworkService _instance;
        PushOptions PushOptions { get; set; }
        FetchOptions FetchOptions { get; set; }
        LibGitService LibGitService { get; set; }
        public Repository Repository { get { return LibGitService.Repository; } }
        public RemoteCollection Remotes { get { return Repository.Network.Remotes; } }
        Dictionary<string, Action<string>> ErrorHandlersMap { get; } = new Dictionary<string, Action<string>>();
        ProgressBarDialog ProgressBarDialog { get; set; }
        Semaphore Wait { get; } = new Semaphore(0, 1);

        protected LibGitNetworkService()
        {
            LibGitService = LibGitService.GetInstance();
            CreatePushOptions();
            CreateErrorHandlerMap();
        }

        public void DeleteRemote(string name)
        {
            Repository.Network.Remotes.Remove(name);
        }

        void CreateErrorHandlerMap()
        {
            ErrorHandlersMap.Add("failed to send request: Operace nebyla v požadované době dokončena.\r\n", NoInternetConnection);
            ErrorHandlersMap.Add("failed to send request: Nelze rozpoznat", NoInternetConnection);
            ErrorHandlersMap.Add("too many redirects or authentication replays", TooManyTries);
            ErrorHandlersMap.Add("invalid refspec", TooManyTries);
            ErrorHandlersMap.Add("UsernamePasswordCredentials contains a null Username or Password.", str => { });
        }

        void CreatePushOptions()
        {
            PushOptions = new PushOptions()
            {
                CredentialsProvider = CredentialsProvider,
                OnPushStatusError = PushStatusErrorHandler,
                OnPushTransferProgress = PushTransferProgressHandler,
            };
            FetchOptions = new FetchOptions()
            {
                CredentialsProvider = CredentialsProvider,
                OnTransferProgress = FetchTransferProgressHandler
            };
        }

        public bool PackBuilderProgressHandler(PackBuilderStage stage, int current, int total)
        {
            Console.WriteLine(stage + " " + current + " " + total);
            return true;
        }

        public bool PushTransferProgressHandler(int current, int total, long bytes)
        {
            ProgressBarDialog.MinorThreadSetCount(current);
            ProgressBarDialog.MinorThreadSetTotal(total);
            ProgressBarDialog.MinorThreadSetBytes(bytes);
            return true;
        }

        public bool FetchTransferProgressHandler(TransferProgress p)
        {
            ProgressBarDialog.MinorThreadSetCount(p.ReceivedObjects);
            ProgressBarDialog.MinorThreadSetTotal(p.TotalObjects);
            ProgressBarDialog.MinorThreadSetBytes(p.ReceivedBytes);
            return true;
        }

        public void AddRemote(string name, string url)
        {
            Repository.Network.Remotes.Add(name, url);
        }

        public void RemoveRemote(string name)
        {
            Repository.Network.Remotes.Remove(name);
        }

        public void UpdateRemote(string oldName, string url)
        {
            LibGit2Sharp.Remote r = Repository.Network.Remotes[oldName];
            Repository.Network.Remotes.Update(oldName, rem => rem.Url = url);
        }

        public void Push()
        {
            ObserveProgress("Pushing HEAD", new Action(DoPush));
        }

        public void Fetch()
        {
            ObserveProgress("Fetching all tracked branches", new Action(DoFetch));
        }

        public void Pull()
        {
            ObserveProgress("Pulling to HEAD", new Action(DoPull));
        }

        public void ObserveProgress(string message, Action action)
        {
            ProgressBarDialog = new ProgressBarDialog
            {
                Owner = Application.Current.MainWindow,
                Message = message
            };
            action.BeginInvoke(CloseProgressBarDialog, null);
            ProgressBarDialog.ShowDialog();
            Wait.WaitOne();
        }

        void CloseProgressBarDialog(IAsyncResult res)
        {
            ProgressBarDialog.MinorThreadClose();
            ProgressBarDialog = null;
            Wait.Release();
        }

        void DoPush()
        {
            First = true;
            var selected = Program.GetInstance().RemoteManager.SelectedRepositoryRemote;
            string currentBranch = Repository.Head.CanonicalName;
            try
            {
                Repository.Network.Push(selected, new List<string>() { currentBranch }, PushOptions);
                if (!PushErrors)
                {
                    Repository.Branches.Update(Repository.Head,
                        b => b.Remote = selected.Name,
                        b => b.UpstreamBranch = Repository.Head.CanonicalName);
                    PushErrors = false;
                }
            }
            catch (NonFastForwardException e)
            {
                NonFastForwardPush(e.Message);
            }
            catch (LibGit2SharpException e)
            {
                ParseGeneralException(e);
            }
            PushErrors = false;
        }

        void DoFetch()
        {
            First = true;
            var selected = Program.GetInstance().RemoteManager.SelectedRepositoryRemote;
            string currentBranch = Repository.Head.CanonicalName;
            try
            {
                Repository.Network.Fetch(selected.Name, selected.FetchRefSpecs.Select(s => s.ToString()), FetchOptions);
            }
            catch (NonFastForwardException e)
            {
                NonFastForwardPush(e.Message);
            }
            catch (LibGit2SharpException e)
            {
                ParseGeneralException(e);
            }
        }

        void DoPull()
        {
            First = true;
            var selected = Program.GetInstance().RemoteManager.SelectedRepositoryRemote;
            if (selected == null)
                Message("No remote to pull from");
            if (Repository.Info.IsHeadDetached || Repository.Info.IsHeadDetached)
                Message("No branch to pull");
            Branch currentBranch = Repository.Head;
            Repository.Branches.Update(currentBranch, b => b.Remote = selected.Name, b => b.UpstreamBranch = currentBranch.CanonicalName);
            try
            {
                PullOptions options = new PullOptions() { FetchOptions = FetchOptions, };
                Commands.Pull(Repository, Program.GetInstance().UserManager.CurrentSignature, options);
            }
            catch (NonFastForwardException e)
            {
                NonFastForwardPush(e.Message);
            }
            catch (LibGit2SharpException e)
            {
                ParseGeneralException(e);
            }
        }

        Credentials CredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            var selected = Program.GetInstance().RemoteManager.SelectedRemote;
            if (selected.UserName != "" && selected.Password != "")
                return AuthentificateFromLog(selected.UserName, selected.Password);
            else
                return PrompUserAuthentification();
        }

        Credentials AuthentificateFromLog(string name, string password)
        {
            return new UsernamePasswordCredentials() { Username = name, Password = password };
        }

        Credentials PrompUserAuthentification()
        {
            AuthentificationDialog dialog = null;
            bool? dr = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialog = new AuthentificationDialog();
                if (!First)
                    dialog.invalidCredentioalsInfo.Visibility = Visibility.Visible;
                dialog.Owner = Application.Current.MainWindow;
                dr = dialog.ShowDialog();
            });
            First = false;
            if (dr == true)
                return new UsernamePasswordCredentials() { Username = dialog.MinorThreadGetName(), Password = dialog.MinorThreadGetPassword() };
            return new UsernamePasswordCredentials() { Username = null, Password = null }; ;
        }

        void PushStatusErrorHandler(PushStatusError errors)
        {
            PushErrors = true;
            throw new NotImplementedException(errors.Message);
        }

        void ParseGeneralException(LibGit2SharpException e)
        {
            string message = e.Message;
            string prefix = ErrorHandlersMap.Keys.FirstOrDefault(p => e.Message.StartsWith(p));
            if (prefix != null)
                ErrorHandlersMap[prefix].Invoke(message);
            else
                NotImplemented(message);
        }

        void NonFastForwardPush(string message)
        {
            Message("Nonfastforward push. Consider pull first.");
        }

        void NotImplemented(string message)
        {
            Message(message);
        }

        void NoInternetConnection(string message)
        {
            Message("Bad internet connection");
        }

        void TooManyTries(string message)
        {
            Message("Too many authentification tries");
        }

        void NoBranch(string message)
        {
            throw new NotImplementedException(message);
        }

        void Message(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show(Application.Current.MainWindow, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
        }

        public static LibGitNetworkService GetInstance()
        {
            if (_instance == null)
                _instance = new LibGitNetworkService();
            return _instance;
        }
    }
}
