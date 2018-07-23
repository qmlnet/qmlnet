using PhotoFrame.Logic.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;
using System.Linq;
using System.Threading;

namespace PhotoFrame.Logic.Tests.BL
{
    public class PhotoDatabaseTest : IDisposable
    {
        private string _TestDataPath;
        private string _SourceDataPath;

        public PhotoDatabaseTest()
        {
            _TestDataPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_TestDataPath);

            _SourceDataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BL", "testphotos");
        }

        private IEnumerable<string> AllTestFileNames => Directory.GetFiles(_SourceDataPath).Select(fPath => Path.GetFileName(fPath));

        private PhotoDatabase CreateUUTWithAllImages()
        {
            return CreateUUT(AllTestFileNames.ToArray());
        }

        private PhotoDatabase CreateUUT(params string[] initialImageFileNames)
        {
            Thread.Sleep(500);
            foreach(var fName in initialImageFileNames)
            {
                File.Copy(
                    Path.Combine(_SourceDataPath, fName),
                    Path.Combine(_TestDataPath, fName)
                    );
            }
            var result = new PhotoDatabase(_TestDataPath);

            return result;
        }

        public void Dispose()
        {
            Directory.Delete(_TestDataPath, true);
        }

        [Fact]
        public void ContainsAllImages()
        {
            var uut = CreateUUTWithAllImages();
            var entries = uut.PhotoFiles;
            Assert.Equal(5, entries.Count);
            Assert.Contains(entries, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "abandoned-forest-industry-34950.jpg")));
            Assert.Contains(entries, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "agriculture-apple-blur-257840.jpg")));
            Assert.Contains(entries, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "animal-animal-photography-big-cat-145939.jpg")));
            Assert.Contains(entries, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "art-ball-ball-shaped-235615.jpg")));
            Assert.Contains(entries, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "beautiful-bird-s-eye-view-boats-1039302.jpg")));
            uut.Stop();
        }

        [Fact]
        public void InitialHasBeenShownIsFalse()
        {
            var uut = CreateUUTWithAllImages();
            var entries = uut.PhotoFiles;

            Assert.All(entries, e => Assert.False(e.HasBeenShownInThisSession));
            uut.Stop();
        }

        [Fact]
        public void ReactsToImageFileDeleted()
        {
            var uut = CreateUUTWithAllImages();
            var entries = uut.PhotoFiles;
            Assert.Equal(5, entries.Count);

            IReadOnlyList<PhotoEntry> newList = null;

            uut.PhotosListChanged += (s, e) =>
            {
                newList = ((PhotoDatabase)s).PhotoFiles;
            };

            File.Delete(Path.Combine(_TestDataPath, "animal-animal-photography-big-cat-145939.jpg"));

            Thread.Sleep(4000);

            Assert.NotNull(newList);
            Assert.Equal(4, newList.Count);
            Assert.DoesNotContain(newList, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "animal-animal-photography-big-cat-145939.jpg")));
            uut.Stop();
        }

        [Fact]
        public void CanHandleNoImages()
        {
            var uut = CreateUUT();
            var entries = uut.PhotoFiles;
            Assert.Equal(0, entries.Count);
            uut.Stop();
        }

        [Fact]
        public void ReactsToImageFileAdded()
        {
            var allFileNames = AllTestFileNames.ToList();
            allFileNames.Remove("animal-animal-photography-big-cat-145939.jpg");

            var uut = CreateUUT(allFileNames.ToArray());
            var entries = uut.PhotoFiles;
            Assert.Equal(4, entries.Count);

            IReadOnlyList<PhotoEntry> newList = null;

            uut.PhotosListChanged += (s, e) =>
            {
                newList = ((PhotoDatabase)s).PhotoFiles;
            };

            File.Copy(
                Path.Combine(_SourceDataPath, "animal-animal-photography-big-cat-145939.jpg"),
                Path.Combine(_TestDataPath, "animal-animal-photography-big-cat-145939.jpg")
                );

            Thread.Sleep(4000);

            Assert.NotNull(newList);
            Assert.Equal(5, newList.Count);
            Assert.Contains(newList, e => string.Equals(e.FilePath, Path.Combine(_TestDataPath, "animal-animal-photography-big-cat-145939.jpg")));
            uut.Stop();
        }
    }
}
