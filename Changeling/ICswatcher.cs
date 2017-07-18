using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Changeling
{
    public interface ICswatcher
    {
        void DisplayFiller(Filler content);
        void SendFolder(string[] AddFolders, string[] DeleteFolders);
    }
    public class Filler
    {
        private string _path = "";
        private string _change = "";

        public Filler() { }
        public Filler(string p, string c)
        {
            _path = p;
            _change = c;
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string Change
        {
            get { return _change; }
            set { _change = value; }
        }
    }
    public delegate void DisplayFillerDelegate(Filler data);
}
