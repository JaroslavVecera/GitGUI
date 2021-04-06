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
        bool Cancel { get; set; }
        bool PushErrors { get; set; } = false;
        static LibGitNetworkService _instance;
        PushOptions PushOptions { get; set; }
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

        void CreateErrorHandlerMap()
        {
            ErrorHandlersMap.Add("failed to send request: Operace nebyla v požadované době dokončena.\r\n", TimeOut);
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

        public void AddRemote(string name, string url)
        {
            Repository.Network.Remotes.Add(name, url);
        }

        public void Push()
        {
            ProgressBarDialog = new ProgressBarDialog();
            ProgressBarDialog.Owner = Application.Current.MainWindow;
            ProgressBarDialog.Message = "Pushing data...";
            new Action(DoPush).BeginInvoke(CloseProgressBarDialog, null);
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
            Cancel = false;
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
                dialog.Owner = Application.Current.MainWindow;
                dr = dialog.ShowDialog();
            });
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
            throw new NotImplementedException(message);
        }

        void NotImplemented(string message)
        {
            throw new NotImplementedException(message);
        }

        void TimeOut(string message)
        {
            throw new NotImplementedException(message);
        }

        void TooManyTries(string message)
        {
            throw new NotImplementedException(message);
        }

        void NoBranch(string message)
        {
            throw new NotImplementedException(message);
        }

        public static LibGitNetworkService GetInstance()
        {
            if (_instance == null)
                _instance = new LibGitNetworkService();
            return _instance;
        }
    }
}
