using Prism.Mvvm;

namespace ChecksumCalculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Checksum Calculator";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
