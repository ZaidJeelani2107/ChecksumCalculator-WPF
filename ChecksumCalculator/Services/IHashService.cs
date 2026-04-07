using ChecksumCalculator.Models;

namespace ChecksumCalculator.Services
{
    public interface IHashService
    {
        Task<string> ComputeHashAsync(
            string filePath,
            HashAlgorithmType algorithmType,
            IProgress<int>? progress = null,
            CancellationToken cancellationToken = default);

        Task<(string, string)> ComputeBothAsync(
            string firstFilePath,
            string secondFilePath,
            HashAlgorithmType algorithmType,
            IProgress<int>? progress = null,
            CancellationToken cancellationToken = default);
    }
}
