using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private Mailer _mailer;
        public MainWindow()
        {
            DbConn.database_create();
            InitializeComponent();
            this.DataContext = this;
            _csWatcher = new Cswatcher(this.DisplayFiller);
            _mailer = new Mailer();
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
                    //_csWatcher.SendFolder(filePath, null);
                }
                string[] AddFolders = parts.ToArray();
                //parts.Fo
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
           // string PathChange = path + " " + change;
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new insert(AddItemsToList), path, change);
        }
        
        public void AddItemsToList(string path, string change)
        {
            TrackedChanges.Items.Add(path);
            TrackedChanges.Items.Add(change);
        }
        public delegate void insert(string path, string change);
        #endregion Changes

        private void Btn_removeFromMonitoredFolders_Click(object sender, RoutedEventArgs e)
        {
            int selection = DropBox.SelectedIndex;
            //ISSUE: here i need to check whether the selection is smaller as null, as clicking on remove on an empty list creates an exception
            if (selection != 0)
            {
                parts.RemoveAt(selection);
                Console.WriteLine("Got order to delete folder at Index: " + selection);
                DbConn.database_watchlist_remove_folders(_files.ElementAt(selection));
                _files.RemoveAt(selection);
                _csWatcher.RemoveFolder(selection);
            }
            
        }
        private void List_DelKey (object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                int selection = DropBox.SelectedIndex;
                parts.RemoveAt(selection);
                DbConn.database_watchlist_remove_folders(_files.ElementAt(selection));
                _files.RemoveAt(selection);
                e.Handled = true;
                _csWatcher.RemoveFolder(selection);
            }
        }
        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //this currently works, in theory, but needs me to fix the FullPath + Changes thing
            try
            {
                Process.Start((string)TrackedChanges.SelectedValue);
                Console.WriteLine("Selected Value " + TrackedChanges.SelectedValue);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message); 
            }
        }

        private void Btn_trackedChangesClear_Click(object sender, RoutedEventArgs e)
        {
            TrackedChanges.Items.Clear();
        }

        private void EmailActivateButton_Click(object sender, RoutedEventArgs e)
        {
            _mailer.Smtp_mailer(EmailTextBox.Text, EmailPasswordBox.Password);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void NumberValidationTextBox_spacehandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}