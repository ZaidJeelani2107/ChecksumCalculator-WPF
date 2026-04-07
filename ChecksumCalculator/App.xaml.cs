using ChecksumCalculator.Services;
using System.Windows;
using ChecksumCalculator.ViewModels;
using ChecksumCalculator.Views;

namespace ChecksumCalculator
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return new MainWindow();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IHashService, HashService>();
            containerRegistry.Register<SingleFileViewModel>();
            containerRegistry.Register<CompareFilesViewModel>();
        }
    }
}