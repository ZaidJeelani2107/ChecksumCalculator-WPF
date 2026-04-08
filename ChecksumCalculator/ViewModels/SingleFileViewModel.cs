using ChecksumCalculator.Models;
using ChecksumCalculator.Services;

namespace ChecksumCalculator.ViewModels
{
    public class SingleFileViewModel : HashViewModelBase
    {
        public FileEntry File { get; } = new();
        public DelegateCommand ComputeCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand CompareCommand { get; }


        private string _pastedHash = string.Empty;
        public string PastedHash
        {
            get => _pastedHash;
            set
            {
                SetProperty(ref _pastedHash, value);
                CompareCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _hasCompareResult;
        public bool HasCompareResult
        {
            get => _hasCompareResult;
            set => SetProperty(ref _hasCompareResult, value);
        }

        private bool _isCompareMatch;
        public bool IsCompareMatch
        {
            get => _isCompareMatch;
            set => SetProperty(ref _isCompareMatch, value);
        }

        private string _compareResult = string.Empty;
        public string CompareResult
        {
            get => _compareResult;
            set => SetProperty(ref _compareResult, value);
        }

        public bool CanCompute => File.HasFile && !IsComputing;

        public bool CanCompareHash => File.HasHash && !string.IsNullOrWhiteSpace(PastedHash);

        public SingleFileViewModel(IHashService hashService) : base(hashService)
        {
            ComputeCommand = new DelegateCommand(
                async () => await Compute(),
                () => CanCompute);

            CancelCommand = new DelegateCommand(
                Cancel,
                () => IsComputing);

            CompareCommand = new DelegateCommand(
                ExecuteCompare,
                () => CanCompareHash);

            File.PropertyChanged += (_, _) =>
            {
                ComputeCommand.RaiseCanExecuteChanged();
                CompareCommand.RaiseCanExecuteChanged();
                HasCompareResult = false;
            };

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IsComputing))
                {
                    ComputeCommand.RaiseCanExecuteChanged();
                    CancelCommand.RaiseCanExecuteChanged();
                }
            };
        }

        private void ExecuteCompare()
        {
            IsCompareMatch = string.Equals(
                File.Hash.Trim(),
                PastedHash.Trim(),
                StringComparison.OrdinalIgnoreCase);

            CompareResult = IsCompareMatch ? "✓  Checksums match" : "✗  Checksums do not match";
            HasCompareResult = true;
        }

        private async Task Compute()
        {
            StartOperation($"Computing {SelectedAlgorithm}…");
            try
            {
                var progress = new Progress<int>(x => Progress = x);
                File.Hash = await HashService.ComputeHashAsync(
                    File.FilePath,
                    SelectedAlgorithm,
                    progress,
                    TokenSource!.Token);

                ComputedAlgorithm = SelectedAlgorithm;
                StatusText = "Done";
            }
            catch (OperationCanceledException)
            {
                StatusText = "Cancelled";
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
            }
            finally
            {
                EndOperation();
            }
        }
    }
}