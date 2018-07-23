using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;

namespace PhotoFrame.Logic.BL
{
    public class PhotosListChangedEventArgs : EventArgs { }

    public class PhotoEntry : IEquatable<PhotoEntry>
    {
        public PhotoEntry(string filePath, bool hasBeenShownInThisSession)
        {
            FilePath = filePath;
            HasBeenShownInThisSession = hasBeenShownInThisSession;
        }

        public string FilePath { get; private set; }
        public bool HasBeenShownInThisSession { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            var castedObj = obj as PhotoEntry;
            if(castedObj == null)
            {
                return false;
            }
            return Equals(castedObj);
        }

        public bool Equals(PhotoEntry other)
        {
            return other != null &&
                   FilePath == other.FilePath &&
                   HasBeenShownInThisSession == other.HasBeenShownInThisSession;
        }

        public override int GetHashCode()
        {
            var hashCode = -167777169;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            hashCode = hashCode * -1521134295 + HasBeenShownInThisSession.GetHashCode();
            return hashCode;
        }
    }

    public class PhotoDatabase
    {
        private string _PhotosDirectoryPath;
        private List<PhotoEntry> _PhotosEntries;

        private Timer _UpdateTimer;

        private FileSystemWatcher _FileSystemWatcher;

        private bool IsStopped { get; set; }

        public event EventHandler<PhotosListChangedEventArgs> PhotosListChanged;

        public PhotoDatabase(string photosDirectoryPath)
        {
            _UpdateTimer = new Timer(new TimerCallback(o => UpdateDatabase()));
            _PhotosDirectoryPath = photosDirectoryPath;
            UpdateDatabase();
            _FileSystemWatcher = new FileSystemWatcher(photosDirectoryPath);
            _FileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite
                                            | NotifyFilters.FileName 
                                            | NotifyFilters.DirectoryName;
            _FileSystemWatcher.Changed += (s, e) => WaitAndUpdateDatabase();
            _FileSystemWatcher.Created += (s, e) => WaitAndUpdateDatabase();
            _FileSystemWatcher.Deleted += (s, e) => WaitAndUpdateDatabase();
            _FileSystemWatcher.Renamed += (s, e) => WaitAndUpdateDatabase();

            _FileSystemWatcher.EnableRaisingEvents = true;
        }

        public IReadOnlyList<PhotoEntry> PhotoFiles
        {
            get
            {
                return _PhotosEntries.AsReadOnly();
            }
        }

        public void ResetSession()
        {
            _PhotosEntries.ForEach(pe => pe.HasBeenShownInThisSession = false);
        }

        public void Stop()
        {
            IsStopped = true;
        }

        private void WaitAndUpdateDatabase()
        {
            _UpdateTimer.Change((int)TimeSpan.FromSeconds(2).TotalMilliseconds, Timeout.Infinite);
        }

        private void UpdateDatabase()
        {
            if(IsStopped)
            {
                return;
            }
            var di = new DirectoryInfo(_PhotosDirectoryPath);
            var newList = di.EnumerateFiles("*.*")
                .Where(fi => 
                    string.Equals(fi.Extension, ".jpg", StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(fi.Extension, ".png", StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(fi => fi.Name)
                .Select(fi => new PhotoEntry(fi.FullName, false))
                .ToList();
            if(!AreListsEqual(newList, _PhotosEntries))
            {
                _PhotosEntries = newList;
                RaisePhotosListChanged();
            }
        }

        private bool AreListsEqual<T>(IList<T> listOne, IList<T> listTwo)
        {
            if(listOne == null && listTwo == null)
            {
                return true;
            }
            if(listOne == null || listTwo == null)
            {
                return false;
            }
            if(listOne.Count != listTwo.Count)
            {
                return false;
            }
            for (int i = 0; i < listOne.Count - 1; i++)
            {
                if(!object.Equals(listOne[i], listTwo[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private void RaisePhotosListChanged()
        {
            PhotosListChanged?.Invoke(this, new PhotosListChangedEventArgs());
        }
    }
}
