using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace GitGUI.Logic
{
    class LibGitNetworkService
    {
        bool PushErrors { get; set; } = false;
        static LibGitNetworkService _instance;
        PushOptions PushOptions { get; set; }
        LibGitService LibGitService { get; set; }
        public Repository Repository { get { return LibGitService.Repository; } }
        public RemoteCollection Remotes { get { return Repository.Network.Remotes; } }
        Dictionary<string, Action<string>> ErrorHandlersMap { get; } = new Dictionary<string, Action<string>>();

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
        }

        void CreatePushOptions()
        { 
            PushOptions = new PushOptions()
            {
                CredentialsProvider = CredentialsProvider,
                OnPushStatusError = PushStatusErrorHandler
            };
        }

        public void AddRemote(string name, string url)
        {
            Repository.Network.Remotes.Add(name, url);
        }

        public void Push()
        {
            var test = Repository.Network.Remotes.First();
            string currentBranch = Repository.Head.CanonicalName;
            try
            {
                Repository.Network.Push(test, new List<string>() { currentBranch }, PushOptions);
                if (!PushErrors)
                {
                    Repository.Branches.Update(Repository.Head,
                        b => b.Remote = test.Name,
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
        }

        Credentials CredentialsProvider(string url, string usernameFromUrl, SupportedCredentialTypes types)
        {
            string username = "jarekvecer@seznam.cz";
            string password = "";
            return new UsernamePasswordCredentials() { Username = username, Password = password };
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
