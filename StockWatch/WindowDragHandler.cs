using System.Windows;
using System.Windows.Input;

namespace StockPricesApp
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false;
        private Point offset;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            offset = e.GetPosition(this);
            ((UIElement)sender).CaptureMouse();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
            SaveCheckboxStatesToConfig();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point newPoint = e.GetPosition(null);
                double deltaX = newPoint.X - offset.X;
                double deltaY = newPoint.Y - offset.Y;
                Left += deltaX;
                Top += deltaY;
            }
        }
    }
}