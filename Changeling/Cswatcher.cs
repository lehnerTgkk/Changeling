using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//PRobably will need to go use an interface with contracts to make this work
namespace Changeling
{
    public class Cswatcher : ICswatcher
    {
        List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
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
        //ISSUE: I should remove the DeleteFolders string array, since it is no longer needed. The problem with removing the filewatchers can be traced back to the fact that here i give an array, so everytime that array is posted, the number of file watchers grow.
        public void SendFolder(string[] AddFolders, string[] DeleteFolders)
        {
                Console.WriteLine("Länge des Arrays in der relevanten Methode" + AddFolders.Length);
            for (int i = 0; i < AddFolders.Length; i++)
            {       
                watchers.Add(new FileSystemWatcher(AddFolders[i]));
                //Console.WriteLine("Created this number of monitors" + i);
                //.Path = AddFolders[i];
                Console.WriteLine("Folder that was added to watchlist: " + AddFolders[i]);
                //watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                //watcher.Renamed += new RenamedEventHandler(OnRenamed);
                //watcher.Deleted += new FileSystemEventHandler(OnChanged);
                //watcher.Created += new FileSystemEventHandler(OnChanged);
                //watcher.EnableRaisingEvents = true;
                //watcher.
                }
                Console.WriteLine("Number of active watchers: " + watchers.Count);
                DbConn.database_watchList_fill(AddFolders);
            foreach (FileSystemWatcher element in watchers)
            {
                element.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                element.Renamed += new RenamedEventHandler(OnRenamed);
                element.Deleted += new FileSystemEventHandler(OnChanged);
                element.Created += new FileSystemEventHandler(OnChanged);
                element.EnableRaisingEvents = true;
            }

        }
        public void RemoveFolder(int RemoveFolders)
        {
            FileSystemWatcher element = watchers.ElementAt(RemoveFolders);
            element.EnableRaisingEvents = false;
            Console.WriteLine("Watcher removed at " + RemoveFolders);
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
