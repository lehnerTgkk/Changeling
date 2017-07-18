using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Changeling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private Cswatcher _csWatcher;
        public MainWindow()
        {
            DbConn.database_create();
            InitializeComponent();
            this.DataContext = this;
            _csWatcher = new Cswatcher(this.DisplayFiller);
        }

        #region Drag and Drop
        public ObservableCollection<string> Files
        {
            get
            {
                return _files;
            }
        }
        List<string> parts = new List<string>();
        private ObservableCollection<string> _files = new ObservableCollection<string>();
        public void DropBox_Drop(object sender, DragEventArgs e)
        { 
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
               // _files.Clear();
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                foreach (string filePath in files)
                {
                    var fileName = System.IO.Path.GetFileName(filePath);
                    _files.Add(fileName);
                    parts.Add(filePath);
                }
                string[] AddFolders = parts.ToArray();
                parts.Fo
                _csWatcher.SendFolder(AddFolders, null);
            }
            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226)); 
        }
        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                var listbox = sender as ListBox;
                listbox.Background = new SolidColorBrush(Color.FromRgb(155, 155, 155));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));
        }
        #endregion Drag and Drop
        #region Changes
        //Background Worker for writing Changes
        public void DisplayFiller(Filler content)
        {
            string path = content.Path == null ? "" : content.Path;
            string change = content.Change == null ? "" : content.Change;
            string PathChange = path + " " + change;
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new insert(AddItemsToList), PathChange);
        }
        
        public void AddItemsToList(string path)
        {
            trackedChanges.Items.Add(path);
        }
        public delegate void insert(string path);
        #endregion Changes

        private void btn_removeFromMonitoredFolders_Click(object sender, RoutedEventArgs e)
        {
            // foreach (string items in DropBox.SelectedItems)
            // {
            //     _files.Remove(items);
            //     parts.Remove(items);
            //     DbConn.database_watchlist_remove_folders(items);
            // }
            // Console.WriteLine("Index of selected items: " + DropBox.SelectedIndex);
            int selection = DropBox.SelectedIndex;
            if (selection > 0)
            {
                parts.RemoveAt(selection);
                DbConn.database_watchlist_remove_folders(_files.ElementAt(selection));
                _files.RemoveAt(selection);

            }
        }

        private void btn_trackedChangesClear_Click(object sender, RoutedEventArgs e)
        {
            trackedChanges.Items.Clear();
        }
    }
}