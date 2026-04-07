using ChecksumCalculator.Models;
using ChecksumCalculator.Services;

namespace ChecksumCalculator.ViewModels
{
    public class SingleFileViewModel : HashViewModelBase
    {
        public FileEntry File { get; } = new();
        public DelegateCommand ComputeCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public bool CanCompute => File.HasFile && !IsComputing;

        public SingleFileViewModel(IHashService hashService) : base(hashService)
        {
            ComputeCommand = new DelegateCommand(
                async () => await Compute(),
                () => CanCompute);

            CancelCommand = new DelegateCommand(
                Cancel,
                () => IsComputing);

            File.PropertyChanged += (_, _) =>
                ComputeCommand.RaiseCanExecuteChanged();

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
            StartOperation($"Computing {SelectedAlgorithm}…");
            try
            {
                var progress = new Progress<int>(x => Progress = x);
                File.Hash = await HashService.ComputeHashAsync(
                    File.FilePath,
                    SelectedAlgorithm,
                    progress,
                    TokenSource!.Token);

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