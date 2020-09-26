# LibGit2Sharp use

---

Description of most common planned usecases for GitGUI project.

This document was written due to lack of proper LibGit2Sharp documentation.

---

- Stashing

    ```csharp
    repo.Stashes.Add(signature, "Optional stash comment"));
    repo.Stashes.Pop(index);
    ```

- Checkout

    ```csharp
    Branch currentBranch = Commands.Checkout(repo , branch);
    ```

- Repository (nezapomenout Dispose())
    - Create

        ```csharp
        string rootedPath = Repository.Init(@"D:\temp\rooted\path");
        ```

    - Open

        ```csharp
        Repository repo = new Repository("path/to/my/repo")
        //kontrola pomocí
        Repository.IsValid("path"); //takže není třeba Exception
        ```

    - Change user [ if possible ]
    - Handle Changes
- Commit
    - Changes

        ```csharp
        List<TreeEntryChanges> c =  repo.Diff.Compare<TreeChanges>();
        List<TreeEntryChanges> c = repo.Diff.Compare<TreeChanges>(repo.Head.Tip.Tree,
        																															DiffTargets.Index);
        Patch p = r.Diff.Compare<Patch>(...);
        ```

    - Create

        ```csharp
        // Stage the file
        repo.Index.Add("fileToCommit.txt");
        repo.Index.Write();

        // Create the committer's signature and commit
        Signature author = new Signature("James", "@jugglingnutcase", DateTime.Now);
        Signature committer = author;

        // Commit to the repository
        Commit commit = repo.Commit("Here's a commit i made!", author, committer);
        ```

    - Parents

        Collection IEnumerable<Commit>

    - Author
        - Name
        - Email
        - When
    - Tree
- Branch
    - Create

        ```csharp
        repo.CreateBranch("develop");
        //or
        repo.CreateBranch("other", target);
        ```

- Exceptions
    - CheckoutConflictException
    - EmptyCommitException
    - EntryExistsException
    - LockedFileException
    - NameConflictException
    - MergeFetchHeadNotFoundException
    - NonFastForwardException

        stane se při pushování, pokud nejde nestratit commity.

- FileChangeWatcher (nezapomnět Dispose();)

    ```csharp
    FileSystemWatcher watcher = new FileSystemWatcher()
    watcher.Path = path;
    watcher.NotifyFilter = NotifyFilters.LastAccess
                         | NotifyFilters.LastWrite
                         | NotifyFilters.FileName
                         | NotifyFilters.DirectoryName;
    // Wath all.
    watcher.Filter = "";

    // Add event handlers.
    watcher.Changed += OnChanged;
    watcher.Created += OnChanged;
    watcher.Deleted += OnChanged;
    watcher.Renamed += OnRenamed;

    // Begin watching.
    watcher.EnableRaisingEvents = true;
    ```

- Fetch

    ```csharp
    var remote = repo.Network.Remotes["origin"];
    var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
    Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
    ```

- Pull

    ```csharp
    LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
        options.FetchOptions = new FetchOptions();
        options.FetchOptions.CredentialsProvider = new CredentialsHandler(
            (url, usernameFromUrl, types) =>
                new UsernamePasswordCredentials()
                {
                    Username = USERNAME,
                    Password = PASSWORD
                });

        // User information to create a merge commit
        var signature = new LibGit2Sharp.Signature(
            new Identity("MERGE_USER_NAME", "MERGE_USER_EMAIL"), DateTimeOffset.Now);

        // Pull
        Commands.Pull(repo, signature, options);
    ```

- Push

    ```csharp
    Remote remote = repo.Network.Remotes["origin"];
    var options = new PushOptions();
    options.CredentialsProvider = (_url, _user, _cred) => 
    new UsernamePasswordCredentials { Username = "USERNAME", Password = "PASSWORD" };
    repo.Network.Push(remote, @"refs/heads/master", options);
    ```