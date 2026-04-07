using System.IO;
using System.Windows;

namespace ChecksumCalculator.Models
{
    public class FileEntry : BindableBase
    {
        private string _filePath = string.Empty;
        public string FilePath
        {
            get => _filePath;
            set
            {
                SetProperty(ref _filePath, value);
                RaisePropertyChanged(nameof(HasFile));
            }
        }

        private string _fileName = string.Empty;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private string _fileSize = string.Empty;
        public string FileSize
        {
            get => _fileSize;
            set => SetProperty(ref _fileSize, value);
        }

        private string _hash = string.Empty;
        public string Hash
        {
            get => _hash;
            set
            {
                SetProperty(ref _hash, value);
                RaisePropertyChanged(nameof(HasHash));
            }
        }

        public bool HasFile => !string.IsNullOrEmpty(FilePath);
        public bool HasHash => !string.IsNullOrEmpty(Hash);

        //Commands
        public DelegateCommand BrowseCommand { get; }

        public DelegateCommand CopyCommand { get; }

        public FileEntry()
        {
            BrowseCommand = new DelegateCommand(BrowseFile);

            CopyCommand = new DelegateCommand(CopyHash, () => HasHash)
                .ObservesProperty(() => Hash);
        }

        public void LoadFile(string path)
        {
            if (!File.Exists(path)) return;

            FilePath = path;
            FileName = Path.GetFileName(path);
            FileSize = FormatSize(new FileInfo(path).Length);
            Hash = string.Empty; // clear stale hash from previous file
        }

        private void BrowseFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Select a file" };
            if (dlg.ShowDialog() == true)
                LoadFile(dlg.FileName);
        }

        private void CopyHash()
        {
            if (HasHash) Clipboard.SetText(Hash);
        }

        private static string FormatSize(long bytes) => bytes switch
        {
            < 1_024 => $"{bytes} B",
            < 1_024 * 1_024 => $"{bytes / 1_024.0:F1} KB",
            < 1_024 * 1_024 * 1_024 => $"{bytes / (1_024.0 * 1_024):F1} MB",
            _ => $"{bytes / (1_024.0 * 1_024 * 1_024):F2} GB"
        };
    }
}