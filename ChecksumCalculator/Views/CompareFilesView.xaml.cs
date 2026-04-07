using ChecksumCalculator.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChecksumCalculator.Views
{
    public partial class CompareFilesView : UserControl
    {
        public CompareFilesView()
        {
            InitializeComponent();
        }

        private void DropFirstFile_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => SetHover(DropFirst);

        private void DropFirstFile_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ResetBorder(DropFirst);

        private void DropSecondFile_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => SetHover(DropSecond);

        private void DropSecondFile_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ResetBorder(DropSecond);

        private async void DropFirstFile_Drop(object sender, DragEventArgs e)
        {
            ResetBorder(DropFirst);
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            if (DataContext is not CompareFilesViewModel vm) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;

            vm.IsLoading = true;
            vm.LoadingMessage = "Loading first file…";
            await Task.Run(() => vm.FirstFile.LoadFile(files[0]));
            vm.IsLoading = false;
        }

        private void DropFirstFile_DragOver(object sender, DragEventArgs e)
            => SetDragOver(DropFirst, e);

        private void DropFirstFile_DragLeave(object sender, DragEventArgs e)
            => ResetBorder(DropFirst);

        private async void DropFirstFile_Click(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not CompareFilesViewModel vm) return;

            var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Select first file" };
            if (dlg.ShowDialog() != true) return;

            vm.IsLoading = true;
            vm.LoadingMessage = "Loading first file…";
            await Task.Run(() => vm.FirstFile.LoadFile(dlg.FileName));
            vm.IsLoading = false;
        }

        private async void DropSecondFile_Drop(object sender, DragEventArgs e)
        {
            ResetBorder(DropSecond);
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            if (DataContext is not CompareFilesViewModel vm) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;

            vm.IsLoading = true;
            vm.LoadingMessage = "Loading second file…";
            await Task.Run(() => vm.SecondFile.LoadFile(files[0]));
            vm.IsLoading = false;
        }

        private void DropSecondFile_DragOver(object sender, DragEventArgs e)
            => SetDragOver(DropSecond, e);

        private void DropSecondFile_DragLeave(object sender, DragEventArgs e)
            => ResetBorder(DropSecond);

        private async void DropSecondFile_Click(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is not CompareFilesViewModel vm) return;

            var dlg = new Microsoft.Win32.OpenFileDialog { Title = "Select second file" };
            if (dlg.ShowDialog() != true) return;

            vm.IsLoading = true;
            vm.LoadingMessage = "Loading second file…";
            await Task.Run(() => vm.SecondFile.LoadFile(dlg.FileName));
            vm.IsLoading = false;
        }

        private static void SetDragOver(Border b, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            b.BorderBrush = new SolidColorBrush(Color.FromRgb(91, 173, 238));
            b.Background = new SolidColorBrush(Color.FromArgb(30, 91, 173, 238));
            e.Handled = true;
        }

        private static void SetHover(Border b)
        {
            b.Background = new SolidColorBrush(Color.FromRgb(0xF0, 0xF8, 0xFF));
            b.BorderBrush = new SolidColorBrush(Color.FromRgb(0xA0, 0xCC, 0xEE));
        }

        private static void ResetBorder(Border b)
        {
            b.BorderBrush = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC));
            b.Background = new SolidColorBrush(Colors.White);
        }
    }
}