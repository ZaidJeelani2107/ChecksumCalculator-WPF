using ChecksumCalculator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChecksumCalculator.Services
{
    public class HashService : IHashService
    {
        private readonly int _bufferSize = 81920;
        public async Task<string> ComputeHashAsync(string filePath, HashAlgorithmType algorithmType, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            using var algorithm = CreateAlgorithm(algorithmType);
            using var fileStream = new FileStream(
                filePath, 
                FileMode.Open, 
                FileAccess.Read, 
                FileShare.Read, 
                _bufferSize, 
                true);

            var totalBytes = fileStream.Length;
            var buffer = new byte[_bufferSize];
            long processed = 0;
            int read;

            while((read = await fileStream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                algorithm.TransformBlock(buffer, 0, read, null, 0);
                processed += read;

                if(totalBytes > 0)
                    progress?.Report((int)(processed * 100/ totalBytes));
            }

            algorithm.TransformFinalBlock(buffer, 0, 0);
            return BitConverter.ToString(algorithm.Hash!).Replace("-", "").ToLowerInvariant();
        }

        public async Task<(string, string)> ComputeBothAsync(string firstFilePath, string secondFilePath, HashAlgorithmType algorithmType, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
        {
            var firstFileProgress = new Progress<int>(x => progress?.Report(x / 2));
            var secondFileProgress = new Progress<int>(x => progress?.Report(50 + x / 2));

            var firstFileHash = await ComputeHashAsync(firstFilePath, algorithmType, firstFileProgress, cancellationToken);
            var secondFileHash = await ComputeHashAsync(secondFilePath, algorithmType, secondFileProgress, cancellationToken);

            return(firstFileHash, secondFileHash);
        }

        private static HashAlgorithm CreateAlgorithm(HashAlgorithmType algorithmType)
            => algorithmType switch
            {
                HashAlgorithmType.MD5 => MD5.Create(),
                HashAlgorithmType.SHA1 => SHA1.Create(),
                HashAlgorithmType.SHA256 => SHA256.Create(),
                HashAlgorithmType.SHA512 => SHA512.Create(),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithmType))
            };
    }
}
