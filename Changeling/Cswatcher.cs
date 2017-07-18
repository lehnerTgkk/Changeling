using System;
using System.IO;
//PRobably will need to go use an interface with contracts to make this work
namespace Changeling
{
    public class Cswatcher : ICswatcher
    {
        public void SendFolder(string[] AddFolders, string[] DeleteFolders)
        {
           // DbConn.database_create();
            Run(AddFolders, DeleteFolders);
        }
        DisplayFillerDelegate _displayFillerDelegate = null;

        public Cswatcher(DisplayFillerDelegate dfd)
        {
            _displayFillerDelegate = dfd;
        }

        public void DisplayFiller(Filler content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            if (_displayFillerDelegate != null)
            {
                _displayFillerDelegate(content);
            }
        }

        public void Run(string[] AddFolders, string[] DeleteFolders)
        {
            if (DeleteFolders == null)
            {
                Console.WriteLine("Länge des Arrays in der relevanten Methode" + AddFolders.Length);
                for (int i = 0; i < AddFolders.Length; i++)
                {       
                    if (DeleteFolders == null)
                    {
                        FileSystemWatcher watcher = new FileSystemWatcher();
                        Console.WriteLine("Created this number of monitors" + i);
                        watcher.Path = AddFolders[i];
                        Console.WriteLine("Folder that was added to watchlist: " + AddFolders[i]);
                        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                        watcher.Renamed += new RenamedEventHandler(OnRenamed);
                        watcher.Deleted += new FileSystemEventHandler(OnChanged);
                        //watcher.Changed += new FileSystemEventHandler(OnChanged);
                        watcher.Created += new FileSystemEventHandler(OnChanged);
                        watcher.EnableRaisingEvents = true;
                    }

                    //else
                    //{
                       // watcher.Dispose();
                    //}
                }
                DbConn.database_watchList_fill(AddFolders);
            }
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
           WatcherChangeTypes wct = e.ChangeType;
            string change = wct.ToString();
            SendContent(e.FullPath, change);
            DbConn.database_watchList_changes(e.FullPath, change);

        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            string change = "was renamed to" + e.FullPath;
            SendContent(e.FullPath, change); 
            DbConn.database_watchList_changes(e.OldFullPath, change);
        }
        public void SendContent(string path, string changes)
        {
            DisplayFiller(new Filler(path, changes));
        }
    }
}
