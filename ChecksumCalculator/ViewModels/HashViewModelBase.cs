using ChecksumCalculator.Models;
using ChecksumCalculator.Services;

namespace ChecksumCalculator.ViewModels
{
    public abstract class HashViewModelBase : BindableBase
    {
        public IReadOnlyList<HashAlgorithmType> AvailableAlgorithms { get; } = (HashAlgorithmType[])Enum.GetValues(typeof(HashAlgorithmType));

        private HashAlgorithmType _selectedAlgorithm = HashAlgorithmType.SHA256;
        public HashAlgorithmType SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set => SetProperty(ref _selectedAlgorithm, value);
        }

        private int _progress;
        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private bool _isComputing;
        public bool IsComputing
        {
            get => _isComputing;
            protected set => SetProperty(ref _isComputing, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _loadingMessage = string.Empty;
        public string LoadingMessage
        {
            get => _loadingMessage;
            set => SetProperty(ref _loadingMessage, value);
        }

        protected CancellationTokenSource? TokenSource { get; private set; }
        protected IHashService HashService { get; }

        protected HashViewModelBase(IHashService hashService)
        {
            HashService = hashService;
        }

        protected void StartOperation(string message = "Computing…")
        {
            TokenSource = new CancellationTokenSource();
            IsComputing = true;
            IsLoading = true;
            LoadingMessage = message;
            Progress = 0;
        }

        protected void EndOperation()
        {
            IsComputing = false;
            IsLoading = false;
            Progress = 0;
            TokenSource?.Dispose();
            TokenSource = null;
        }

        protected void Cancel() => TokenSource?.Cancel();
    }
}