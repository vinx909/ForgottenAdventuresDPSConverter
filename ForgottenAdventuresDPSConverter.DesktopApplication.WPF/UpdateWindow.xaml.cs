using ForgottenAdventuresDPSConverter.Core.Interfaces;
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
        private readonly IFAFolderService fAFolderService;

        private Treeviewer treeview;
        private Treeviewer failedToAddTreeViewer;
        private Treeviewer failedToUpdateTreeViewer;
        private Treeviewer noLongerExistingTreeViewer;
        private Treeviewer addedTreeViewer;
        private Treeviewer updatedTreeViewer;
        private Treeviewer unalteredTreeViewer;
        private double progress;
        private FAFolderUpdateReport? report;

        public Treeviewer Treeview { get => treeview; set { if (treeview != value) { treeview = value; RaiseProppertyChanged(); } } }
        public double Progress { get => progress; set { if (value > progress) { progress = value; RaiseProppertyChanged(); } } }

        public UpdateWindow()
        {
            failedToAddTreeViewer = new() { Name = "failed to add" };
            failedToUpdateTreeViewer = new() { Name = "failed to update" };
            noLongerExistingTreeViewer = new() { Name = "no longer existing" };
            addedTreeViewer = new() { Name = "added" };
            updatedTreeViewer = new() { Name = "updated" };
            unalteredTreeViewer = new() { Name = "unaltered" };

            InitializeComponent();

            DataContext = this;

            Treeview = new();
            Treeview.Children.Add(failedToAddTreeViewer);
            Treeview.Children.Add(failedToUpdateTreeViewer);
            Treeview.Children.Add(noLongerExistingTreeViewer);
            Treeview.Children.Add(addedTreeViewer);
            Treeview.Children.Add(updatedTreeViewer);
            Treeview.Children.Add(unalteredTreeViewer);

            treeviewXaml.ItemsSource = Treeview.Children;
        }

        public UpdateWindow(IFAFolderService fAFolderService) : this()
        {
            this.fAFolderService = fAFolderService;
        }

        public void UpdateUpdatedWindow(FAFolderUpdateReport report)
        {
            foreach (string failedToAddFolder in report.FailedToAdd)
            {
                failedToAddTreeViewer.Children.Add(new() { Name = failedToAddFolder });
            }

            foreach (int failedToUpdateFolderId in report.FailedToUpdate)
            {
                failedToUpdateTreeViewer.Children.Add(new() { Id = failedToUpdateFolderId, Name = fAFolderService.Get(failedToUpdateFolderId).Result.RelativePath });
            }

            foreach (int noLongerExistingId in report.NoLongerExisting)
            {
                noLongerExistingTreeViewer.Children.Add(new() { Id = noLongerExistingId, Name = fAFolderService.Get(noLongerExistingId).Result.RelativePath });
            }

            foreach (int addedId in report.Added)
            {
                addedTreeViewer.Children.Add(new() { Id = addedId, Name = fAFolderService.Get(addedId).Result.RelativePath });
            }

            foreach (int updatedId in report.Updated)
            {
                updatedTreeViewer.Children.Add(new() { Id = updatedId, Name = fAFolderService.Get(updatedId).Result.RelativePath });
            }


            foreach (int unalteredId in report.Unaltered)
            {
                unalteredTreeViewer.Children.Add(new() { Id = unalteredId, Name = fAFolderService.Get(unalteredId).Result.RelativePath });
            }
        }

        public void Report(double value)
        {
            Progress = value;
            if(Progress == 1)
            {
                RefreshButton_Click(null, null);
            }
        }

        public void Report(FAFolderUpdateReport value)
        {
            if(report == null)
            {
                report = value;
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (report != null)
            {
                FAFolderUpdateReport reportClone = (FAFolderUpdateReport)report.Clone();

                failedToAddTreeViewer.Children.Clear();
                foreach (string failedToAddFolder in reportClone.FailedToAdd.OrderBy(f => f))
                {
                    failedToAddTreeViewer.Children.Add(new() { Name = failedToAddFolder });
                }



                List<Treeviewer> failedToUpdateTreeViewerClone = new List<Treeviewer>(failedToUpdateTreeViewer.Children);
                foreach (int failedToUpdateFolderId in reportClone.FailedToUpdate)
                {
                    if (failedToUpdateTreeViewerClone.FirstOrDefault(f => f.Id == failedToUpdateFolderId) == null)
                    {
                        failedToUpdateTreeViewerClone.Add(new() { Id = failedToUpdateFolderId, Name = fAFolderService.Get(failedToUpdateFolderId).Result.RelativePath });
                    }
                }
                failedToUpdateTreeViewer.Children.Clear();
                foreach (Treeviewer failedToUpdate in failedToUpdateTreeViewerClone.OrderBy(f => f.Name))
                {
                    failedToUpdateTreeViewer.Children.Add(failedToUpdate);
                }



                List<Treeviewer> noLongerExistingTreeViewerClone = new List<Treeviewer>(noLongerExistingTreeViewer.Children);
                foreach (int noLongerExistingId in reportClone.NoLongerExisting)
                {
                    if (noLongerExistingTreeViewerClone.FirstOrDefault(f => f.Id == noLongerExistingId) == null)
                    {
                        noLongerExistingTreeViewerClone.Add(new() { Id = noLongerExistingId, Name = fAFolderService.Get(noLongerExistingId).Result.RelativePath });
                    }
                }
                noLongerExistingTreeViewer.Children.Clear();
                foreach (Treeviewer noLongerExisting in noLongerExistingTreeViewerClone.OrderBy(f => f.Name))
                {
                    noLongerExistingTreeViewer.Children.Add(noLongerExisting);
                }



                List<Treeviewer> addedTreeViewerClone = new List<Treeviewer>(addedTreeViewer.Children);
                foreach (int addedId in reportClone.Added)
                {
                    if (addedTreeViewerClone.FirstOrDefault(f => f.Id == addedId) == null)
                    {
                        addedTreeViewerClone.Add(new() { Id = addedId, Name = fAFolderService.Get(addedId).Result.RelativePath });
                    }
                }
                addedTreeViewer.Children.Clear();
                foreach (Treeviewer added in addedTreeViewerClone.OrderBy(f => f.Name))
                {
                    addedTreeViewer.Children.Add(added);
                }



                List<Treeviewer> updatedTreeViewerClone = new List<Treeviewer>(updatedTreeViewer.Children);
                foreach (int updatedId in reportClone.Updated)
                {
                    if (updatedTreeViewerClone.FirstOrDefault(f => f.Id == updatedId) == null)
                    {
                        updatedTreeViewerClone.Add(new() { Id = updatedId, Name = fAFolderService.Get(updatedId).Result.RelativePath });
                    }
                }
                updatedTreeViewer.Children.Clear();
                foreach (Treeviewer updated in updatedTreeViewerClone.OrderBy(f => f.Name))
                {
                    updatedTreeViewer.Children.Add(updated);
                }



                List<Treeviewer> unalteredTreeViewerClone = new List<Treeviewer>(unalteredTreeViewer.Children);
                foreach (int unalteredId in reportClone.Unaltered)
                {
                    if (unalteredTreeViewerClone.FirstOrDefault(f => f.Id == unalteredId) == null)
                    {
                        unalteredTreeViewerClone.Add(new() { Id = unalteredId, Name = fAFolderService.Get(unalteredId).Result.RelativePath });
                    }
                }
                unalteredTreeViewer.Children.Clear();
                foreach (Treeviewer unaltered in unalteredTreeViewerClone.OrderBy(f => f.Name))
                {
                    unalteredTreeViewer.Children.Add(unaltered);
                }
            }
        }

        /*
        private static object FAFolderUpdateReportLock = new object();
        public void Report(FAFolderUpdateReport value)
        {
            lock (FAFolderUpdateReportLock)
            {
                value = (FAFolderUpdateReport)value.Clone();

                foreach (string failedToAddFolder in value.FailedToAdd)
                {
                    if (failedToAddTreeViewer.Children.FirstOrDefault(f => f.Name == failedToAddFolder) == null)
                    {
                        failedToAddTreeViewer.Children.Add(new() { Name = failedToAddFolder });
                    }
                }

                foreach (int failedToUpdateFolderId in value.FailedToUpdate)
                {
                    if (failedToUpdateTreeViewer.Children.FirstOrDefault(f => f.Id == failedToUpdateFolderId) == null)
                    {
                        failedToUpdateTreeViewer.Children.Add(new() { Id = failedToUpdateFolderId, Name = fAFolderService.Get(failedToUpdateFolderId).Result.RelativePath });
                    }
                }

                foreach (int noLongerExistingId in value.NoLongerExisting)
                {
                    if (noLongerExistingTreeViewer.Children.FirstOrDefault(f => f.Id == noLongerExistingId) == null)
                    {
                        noLongerExistingTreeViewer.Children.Add(new() { Id = noLongerExistingId, Name = fAFolderService.Get(noLongerExistingId).Result.RelativePath });
                    }
                }

                foreach (int addedId in value.Added)
                {
                    if (addedTreeViewer.Children.FirstOrDefault(f => f.Id == addedId) == null)
                    {
                        addedTreeViewer.Children.Add(new() { Id = addedId, Name = fAFolderService.Get(addedId).Result.RelativePath });
                    }
                }

                foreach (int updatedId in value.Updated)
                {
                    if (updatedTreeViewer.Children.FirstOrDefault(f => f.Id == updatedId) == null)
                    {
                        updatedTreeViewer.Children.Add(new() { Id = updatedId, Name = fAFolderService.Get(updatedId).Result.RelativePath });
                    }
                }


                foreach (int unalteredId in value.Unaltered)
                {
                    if (unalteredTreeViewer.Children.FirstOrDefault(f => f.Id == unalteredId) == null)
                    {
                        unalteredTreeViewer.Children.Add(new() { Id = unalteredId, Name = fAFolderService.Get(unalteredId).Result.RelativePath });
                    }
                }
            }
        }
        */

        public class Treeviewer
        {
            public ObservableCollection<Treeviewer> Children { get; set; }

            public string Name { get; set; }
            public int Id { get; set; }

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
