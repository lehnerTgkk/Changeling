using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;

namespace Backend
{
    public class Cswatcher
    { 
        public static void addWatchFolder(string[] DraggedFolders)
        {
            DataBase.Logging.database_create();
            Run(DraggedFolders);
        }

        public static void Run(string[] DraggedFolders)
        {
            for (int i = 0; i<DraggedFolders.Length; i++)
            {
                Console.WriteLine("Created this number of monitors" + i);
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = DraggedFolders[i];
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Renamed += new RenamedEventHandler(OnRenamed);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);
                //watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.EnableRaisingEvents = true;
            }
            Console.WriteLine("trying to write to database");
            DataBase.Logging.database_watchList_fill(DraggedFolders); 
        }
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
           WatcherChangeTypes wct = e.ChangeType;
           Console.WriteLine("File:" + e.FullPath + " " + e.ChangeType);
            string change = wct.ToString();
            DataBase.Logging.database_watchList_changes(e.FullPath, change);

        }
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Console.WriteLine("File {0} renamted to {1}", e.OldFullPath, e.FullPath);
            string change = "was renamed to" + e.FullPath;
            DataBase.Logging.database_watchList_changes(e.OldFullPath, change);
        }
    }
}
