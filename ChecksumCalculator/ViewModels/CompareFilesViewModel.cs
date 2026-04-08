using ChecksumCalculator.Models;
using ChecksumCalculator.Services;

namespace ChecksumCalculator.ViewModels
{
    public class CompareFilesViewModel : HashViewModelBase
    {
        public FileEntry FirstFile { get; } = new();
        public FileEntry SecondFile { get; } = new();
        public DelegateCommand ComputeCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public bool CanCompute =>
            FirstFile.HasFile && SecondFile.HasFile && !IsComputing;

        private string _matchStatus = string.Empty;
        public string MatchStatus
        {
            get => _matchStatus;
            set => SetProperty(ref _matchStatus, value);
        }

        private bool _isMatch;
        public bool IsMatch
        {
            get => _isMatch;
            set => SetProperty(ref _isMatch, value);
        }

        private bool _hasResults;
        public bool HasResults
        {
            get => _hasResults;
            set => SetProperty(ref _hasResults, value);
        }

        public CompareFilesViewModel(IHashService hashService) : base(hashService)
        {
            ComputeCommand = new DelegateCommand(
                async () => await Compute(),
                () => CanCompute);

            CancelCommand = new DelegateCommand(
                Cancel,
                () => IsComputing);

            FirstFile.PropertyChanged += (_, _) => ComputeCommand.RaiseCanExecuteChanged();
            SecondFile.PropertyChanged += (_, _) => ComputeCommand.RaiseCanExecuteChanged();

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(IsComputing))
                {
                    ComputeCommand.RaiseCanExecuteChanged();
                    CancelCommand.RaiseCanExecuteChanged();
                }
            };
        }

        private async Task Compute()
        {
            HasResults = false;
            StartOperation($"Comparing files ({SelectedAlgorithm})…");
            try
            {
                var progress = new Progress<int>(x => Progress = x);

                var (firstHash, secondHash) = await HashService.ComputeBothAsync(
                    FirstFile.FilePath,
                    SecondFile.FilePath,
                    SelectedAlgorithm,
                    progress,
                    TokenSource!.Token);

                FirstFile.Hash = firstHash;
                SecondFile.Hash = secondHash;
                EvaluateMatch();
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

        private void EvaluateMatch()
        {
            IsMatch = string.Equals(FirstFile.Hash, SecondFile.Hash,
                              StringComparison.OrdinalIgnoreCase);
            MatchStatus = IsMatch ? "✓  Files are identical" : "✗  Files are different";
            HasResults = true;
        }
    }
}