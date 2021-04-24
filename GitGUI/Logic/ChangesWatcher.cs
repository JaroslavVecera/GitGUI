using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GitGUI.Logic
{
    public class ChangesWatcher
    {
        bool First { get; set; } = true;
        bool Repeat { get; set; } = false;
        bool Processing { get; set; } = false;
        FileSystemWatcher Watcher { get; set; }
        System.Timers.Timer ChangesGroupTimer { get; }
        Semaphore Mutex { get; } = new Semaphore(1, 1);
        public bool IsActive { get { return Watcher != null; } }

        public event Action ChangeNoticed;

        public ChangesWatcher()
        {
            ChangesGroupTimer = new System.Timers.Timer(1000) { Enabled = false, AutoReset = false };
            ChangesGroupTimer.Elapsed += OnTimedEvent;
        }

        public void Watch(string path)
        {
            if (IsActive)
                End();
            Watcher = new FileSystemWatcher()
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                             | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "",
                Path = path,
                EnableRaisingEvents = true
            };
            SubscribeWatcherEvents();
        }

        public void End()
        {
            if (!IsActive)
                return;
            UnsubscribeWatcherEvents();
            Watcher = null;
        }

        void SubscribeWatcherEvents()
        {
            Watcher.Changed += Fs;
            Watcher.Created += Fs;
            Watcher.Deleted += Fs;
            Watcher.Renamed += Rs;
        }

        void UnsubscribeWatcherEvents()
        {
            Watcher.Changed -= Fs;
            Watcher.Created -= Fs;
            Watcher.Deleted -= Fs;
            Watcher.Renamed -= Rs;
        }

        void Fs(object sender, FileSystemEventArgs e) =>
            InvokeChangeIfLastNotifyFromSequence();

        void Rs(object sender, RenamedEventArgs e) =>
            InvokeChangeIfLastNotifyFromSequence();

        void InvokeChangeIfLastNotifyFromSequence()
        {
            Mutex.WaitOne();
            if (Processing)
                Repeat = true;
            else if (ChangesGroupTimer.Enabled)
                RestartTimer();
            else if (First)
            {
                First = false;
                ChangesGroupTimer.Start();
            }
            Mutex.Release();
        }

        void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            while (true)
            {
                Mutex.WaitOne();
                Repeat = false;
                Processing = true;
                Mutex.Release();
                Application.Current.Dispatcher.BeginInvoke((Action)(InvokeChange));
                Mutex.WaitOne();
                if (Repeat)
                {
                    Mutex.Release();
                    continue;
                }
                Processing = false;
                First = true;
                Mutex.Release();
                return;
            }
        }

        void RestartTimer()
        {
            ChangesGroupTimer.Stop();
            ChangesGroupTimer.Start();
        }

        void InvokeChange()
        {
            ChangeNoticed?.Invoke();
        }
    }
}
