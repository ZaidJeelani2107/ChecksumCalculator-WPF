namespace ChecksumCalculator.Models
{
    public class FileHashResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
        public long FileSizeInBytes { get; set; }
        public string FileSizeDisplay => FileSizeInBytes switch
        {
            < 1024 => $"{FileSizeInBytes} B",
            < 1024 * 1024 => $"{FileSizeInBytes / 1024.0:F1} KB",
            < 1024 * 1024 * 1024 => $"{FileSizeInBytes / (1024 * 1024):F1} MB",
            _ => $"{FileSizeInBytes / (1024 * 1024 * 1024):F2} GB",
        };
    }
}
