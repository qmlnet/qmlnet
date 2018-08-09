using System;
using System.Collections.Generic;
using System.IO;
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

        public string FilePath { get; }
        public bool HasBeenShownInThisSession { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PhotoEntry castedObj && Equals(castedObj);
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
            return hashCode;
        }
    }

    public class PhotoDatabase
    {
        private readonly string _photosDirectoryPath;
        private List<PhotoEntry> _photosEntries;

        private readonly Timer _updateTimer;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _fileSystemWatcher;

        private bool IsStopped { get; set; }

        public event EventHandler<PhotosListChangedEventArgs> PhotosListChanged;

        public PhotoDatabase(string photosDirectoryPath)
        {
            _updateTimer = new Timer(o => UpdateDatabase());
            _photosDirectoryPath = photosDirectoryPath;
            UpdateDatabase();
            _fileSystemWatcher = new FileSystemWatcher(photosDirectoryPath)
            {
                NotifyFilter = NotifyFilters.LastWrite
                               | NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
            };
            _fileSystemWatcher.Changed += (s, e) => WaitAndUpdateDatabase();
            _fileSystemWatcher.Created += (s, e) => WaitAndUpdateDatabase();
            _fileSystemWatcher.Deleted += (s, e) => WaitAndUpdateDatabase();
            _fileSystemWatcher.Renamed += (s, e) => WaitAndUpdateDatabase();

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public IReadOnlyList<PhotoEntry> PhotoFiles => _photosEntries.AsReadOnly();

        public void ResetSession()
        {
            _photosEntries.ForEach(pe => pe.HasBeenShownInThisSession = false);
        }

        public void Stop()
        {
            IsStopped = true;
        }

        private void WaitAndUpdateDatabase()
        {
            _updateTimer.Change((int)TimeSpan.FromSeconds(2).TotalMilliseconds, Timeout.Infinite);
        }

        private void UpdateDatabase()
        {
            if(IsStopped)
            {
                return;
            }
            var di = new DirectoryInfo(_photosDirectoryPath);
            var newList = di.EnumerateFiles("*.*")
                .Where(fi => 
                    string.Equals(fi.Extension, ".jpg", StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(fi.Extension, ".png", StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(fi => fi.Name)
                .Select(fi => new PhotoEntry(fi.FullName, false))
                .ToList();
            if(!AreListsEqual(newList, _photosEntries))
            {
                _photosEntries = newList;
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
                if(!Equals(listOne[i], listTwo[i]))
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
