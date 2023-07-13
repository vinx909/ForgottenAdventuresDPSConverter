using ForgottenAdventuresDPSConverter.Core.Reports;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window, IProgress<double>, IProgress<FAFolderUpdateReport>, INotifyPropertyChanged
    {
        private Treeviewer treeview;
        private Treeviewer failedToAddTreeViewer;
        private Treeviewer failedToUpdate;
        private Treeviewer noLongerExisting;
        private Treeviewer added;
        private Treeviewer updated;
        private Treeviewer unaltered;
        private double progress;

        public Treeviewer Treeview { get => treeview; set { if (treeview != value) { treeview = value; RaiseProppertyChanged(); } } }
        //public double Progress { get => progress; set { if (progress != value) { progress = value; RaiseProppertyChanged(); } } }

        public UpdateWindow()
        {
            failedToAddTreeViewer = new() { Name = "failed to add" };
            failedToUpdate = new() { Name = "failed to update" };
            noLongerExisting = new() { Name = "no longer existing" };
            added = new() { Name = "added" };
            updated = new() { Name = "updated" };
            unaltered = new() { Name = "unaltered" };

            treeview = new();
            treeview.Children.Add(failedToAddTreeViewer);
            treeview.Children.Add(failedToUpdate);
            treeview.Children.Add(noLongerExisting);
            treeview.Children.Add(added);
            treeview.Children.Add(updated);
            treeview.Children.Add(unaltered);

            InitializeComponent();
        }

        public void Report(double value)
        {
            //Progress = value;
            progressbar.Value = value;

            if (value == 1)
            {
                progressbar.Visibility = Visibility.Collapsed;
            }
        }

        public void Report(FAFolderUpdateReport value)
        {
            throw new NotImplementedException();
        }

        public class Treeviewer
        {
            public ObservableCollection<Treeviewer> Children { get; set; }

            public string Name { get; set; }

            public Treeviewer()
            {
                Children = new();
                Name = string.Empty;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaiseProppertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
