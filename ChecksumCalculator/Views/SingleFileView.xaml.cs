using ChecksumCalculator.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChecksumCalculator.Views
{
    public partial class SingleFileView : UserControl
    {
        public SingleFileView()
        {
            InitializeComponent();
        }

        private void DropFile_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DropFile.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF8, 0xFF));
            DropFile.BorderBrush = new SolidColorBrush(Color.FromRgb(0xA0, 0xCC, 0xEE));
        }

        private void DropFile_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ResetDropFile();

        private async void DropFile_Drop(object sender, DragEventArgs e)
        {
            ResetDropFile();
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            if (DataContext is not SingleFileViewModel vm) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;

            vm.IsLoading = true;
            vm.LoadingMessage = "Loading file…";
            await Task.Run(() => vm.File.LoadFile(files[0]));
            vm.IsLoading = false;
        }

        private void DropFile_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            DropFile.BorderBrush = new SolidColorBrush(Color.FromRgb(91, 173, 238));
            DropFile.Background = new SolidColorBrush(Color.FromArgb(30, 91, 173, 238));
            e.Handled = true;
        }

        private void DropFile_DragLeave(object sender, DragEventArgs e) => ResetDropFile();

        private async void DropFile_Click(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not SingleFileViewModel vm) return;

            var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Select a file" };
            if (dlg.ShowDialog() != true) return;

            vm.IsLoading = true;
            vm.LoadingMessage = "Loading file…";
            await Task.Run(() => vm.File.LoadFile(dlg.FileName));
            vm.IsLoading = false;
        }

        private void ResetDropFile()
        {
            DropFile.BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
            DropFile.Background = new SolidColorBrush(Colors.White);
        }
    }
}