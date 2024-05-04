using ForgottenAdventuresDPSConverter.DesktopApplication.Viewmodel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForgottenAdventuresDPSConverter.DesktopApplication.WPF.Pages.FAFolder.Commands
{
    /// <summary>
    /// Interaction logic for WallCommandPage.xaml
    /// </summary>
    public partial class WallCommandPage : Page
    {
        private int startingActualHeight = 0;
        private int currentImageHeight = 0;
        //private static SemaphoreSlim semaphore = new(1, 1);

        WallCommandsViewModel viewmodel;

        public int TotalHeight { get; set; }

        public WallCommandPage(WallCommandsViewModel wallCommandsViewModel)
        {
            InitializeComponent();

            viewmodel = wallCommandsViewModel;

            DataContext = viewmodel;
        }

        private void AutoTrimTopBottom(object sender, RoutedEventArgs e)
        {
            viewmodel.AutoTopBottomTrim();
        }

        private void MoveMouseOverImage(object sender, MouseButtonEventArgs e)
        {
            if(origionalImage.Source is BitmapSource bitmapSource)
            {
                var position = e.GetPosition(origionalImage);
                int x = (int)Math.Round(position.X * bitmapSource.PixelWidth / origionalImage.ActualWidth);
                int y = (int)Math.Round(position.Y * bitmapSource.PixelHeight / origionalImage.ActualHeight);
                mousePosition.Text = x + ", " + y;
            }
        }

        private async void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            previewImageRow.MaxHeight = double.PositiveInfinity;
            previewDoubleImageRow.MaxHeight = double.PositiveInfinity;
            origionalImage.MaxHeight = double.PositiveInfinity;
            previewDoubleImageColumn1.MaxWidth = double.MaxValue;
            previewDoubleImageColumn2.MaxWidth = double.MaxValue;
            */
            if (startingActualHeight == 0)
            {
                startingActualHeight = (int)previewImageGrid.ActualHeight;
            }

            /*
            int actualHeight = 0;
            if (semaphore.Wait(3))
            {
                previewImageGrid.Height = 0;
                while (true)
                {
                    previewImageGrid.Height += 3;

                    await Task.Delay(10);

                    if (previewImageGrid.ActualHeight <= actualHeight)
                    {
                        break;
                    }

                    actualHeight = (int)previewImageGrid.ActualHeight;
                }
                semaphore.Release();
            }
            else
            {
                return;
            }
            */

            int previewImageHeight = 0;
            int previewImageWidth = 0;
            int origionalImageHeight = 0;
            int origionalImageWidth= 0;

            if (previewImage.Source is BitmapSource bitmapSource)
            {
                previewImageHeight = bitmapSource.PixelHeight;
                previewImageWidth = bitmapSource.PixelWidth;
            }
            if (origionalImage.Source is BitmapSource bmpSource)
            {
                origionalImageHeight = bmpSource.PixelHeight;
                origionalImageWidth = bmpSource.PixelWidth;
            }

            int gridWidth = (int)previewImageGrid.ActualWidth;
            int practicalPreviewImageHeight = 0;
            int practicalPreviewImageWidth = 0;
            int practicalDoublePreviewImageHeight = 0;
            int practicalDoublePreviewImageWidth = 0;
            int practicalOrigionalImageHeight = 0;
            int practicalOrigionalImageWidth = 0;


            //create local variables and generally scale the images up to the width
            double imageScale = 1.0 * gridWidth / origionalImageWidth;
            double doubleImageScale = 1;

            if (previewImageHeight != 0)
            {
                practicalPreviewImageHeight = (int)(imageScale * previewImageHeight);
                practicalPreviewImageWidth = (int)(imageScale * previewImageWidth);

                doubleImageScale = 1.0 * gridWidth / (previewImageWidth * 2);
                practicalDoublePreviewImageHeight = (int)(doubleImageScale * previewImageHeight);
                practicalDoublePreviewImageWidth = (int)(doubleImageScale * previewImageWidth);
            }

            if(origionalImageHeight != 0)
            {
                practicalOrigionalImageHeight = (int)(imageScale * origionalImageHeight);
                practicalOrigionalImageWidth = (int)(imageScale * origionalImageWidth);
            }

            //count up all the heights
            int totalHeight = practicalPreviewImageHeight + practicalDoublePreviewImageHeight + practicalOrigionalImageHeight;
            if (currentImageHeight < totalHeight)
            {
                double scale = (1.0 * currentImageHeight - practicalDoublePreviewImageHeight) / (practicalPreviewImageHeight + practicalOrigionalImageHeight);
                if(scale >= doubleImageScale)
                {
                    practicalPreviewImageHeight = (int)(practicalPreviewImageHeight * scale);
                    practicalPreviewImageWidth = (int)(practicalPreviewImageWidth * scale);
                    practicalOrigionalImageHeight = (int)(practicalOrigionalImageHeight * scale);
                    practicalOrigionalImageWidth = (int)(practicalOrigionalImageWidth * scale);
                }
                else
                {
                    scale = 1.0 * currentImageHeight / (totalHeight);
                    if (practicalPreviewImageHeight != 0)
                    {
                        practicalPreviewImageHeight = (int)(practicalPreviewImageHeight * scale);
                        practicalPreviewImageWidth = (int)(practicalPreviewImageWidth * scale);
                        practicalDoublePreviewImageHeight = (int)(practicalDoublePreviewImageHeight * scale);
                        practicalDoublePreviewImageWidth = (int)(practicalDoublePreviewImageWidth * scale);
                    }
                    if(practicalOrigionalImageHeight != 0)
                    {
                        practicalOrigionalImageHeight = (int)(practicalOrigionalImageHeight * scale);
                        practicalOrigionalImageWidth = (int)(practicalOrigionalImageWidth * scale);
                    }
                }
            }

            if(practicalPreviewImageHeight != 0)
            {
                previewImage.Height = practicalPreviewImageHeight;
                previewImage.Width = practicalPreviewImageWidth;
                previewImageNextToEachOther1.Height = practicalDoublePreviewImageHeight;
                previewImageNextToEachOther1.Width = practicalDoublePreviewImageWidth;
                previewImageNextToEachOther2.Height = practicalDoublePreviewImageHeight;
                previewImageNextToEachOther2.Width = practicalDoublePreviewImageWidth;
            }
            if(practicalOrigionalImageHeight != 0)
            {
                origionalImage.Height = practicalOrigionalImageHeight;
                origionalImage.Width = practicalOrigionalImageWidth;
            }

            /*
            int previewImageHeight = 0;
            int origionalImageHeight = 0;

            if(previewImage.Source is BitmapSource bitmapSource)
            {
                previewImageHeight = bitmapSource.PixelHeight;
            }
            if(origionalImage.Source is BitmapSource bmpSource)
            {
                origionalImageHeight = bmpSource.PixelHeight;
            }

            if (previewImageHeight != 0)
            {

                GridLength previewImageRowHeight = new GridLength(previewImageHeight, GridUnitType.Star);
                previewImageRow.Height = previewImageRowHeight;
                previewDoubleImageRow.Height = previewImageRowHeight;
            }
            if (origionalImageHeight != 0)
            {
                origionalImageRow.Height = new GridLength(origionalImageHeight, GridUnitType.Star);
            }
            return;

            double totalImageHeight = Window.GetWindow(this).ActualHeight/2 - textBoxBottomTrim.ActualHeight - textBoxLeftTrim.ActualHeight - textBoxRightTrim.ActualHeight;
            int previewImageMaxHeight = (int)(totalImageHeight * previewImageHeight / (previewImageHeight * 2 + origionalImageHeight));
            int origionalImageMaxHeight = (int)(totalImageHeight * origionalImageHeight / (previewImageHeight * 2 + origionalImageHeight));

            if (previewImageMaxHeight != 0)
            {
                previewImage.MaxHeight = previewImageMaxHeight;
                previewImageNextToEachOther1.MaxHeight = previewImageMaxHeight;
                previewImageNextToEachOther2.MaxHeight = previewImageMaxHeight;
            }
            if(origionalImageMaxHeight != 0)
            {
                origionalImage.MaxHeight = origionalImageMaxHeight;
            }
            */
        }

        private void UpdateSize(object sender, RoutedEventArgs e)
        {
            currentImageHeight = 0;
            previewImageGrid.Height = 0;
            while (true)
            {
                previewImageGrid.Height += 3;

                Task.Delay(1000).Wait();

                if (previewImageGrid.ActualHeight <= currentImageHeight)
                {
                    break;
                }

                currentImageHeight = (int)previewImageGrid.ActualHeight;
            }
        }
    }
}
