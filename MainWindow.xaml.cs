using FlexiTools.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace FlexiTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = DataContext as SideMenuViewModel;
            if (viewModel != null)
            {
                viewModel.NavigationFrame = MainFrame;
            }

            StateChanged += MainWindow_StateChanged;
        }

        private bool isMenuOpen = false;

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuOpen)
            {
                // Fechar menu
                var closeStoryboard = (Storyboard)FindResource("CloseMenuStoryboard");
                closeStoryboard.Begin();
            }
            else
            {
                // Abrir menu
                var openStoryboard = (Storyboard)FindResource("OpenMenuStoryboard");
                openStoryboard.Begin();
            }
            isMenuOpen = !isMenuOpen;
        }


        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                AdjustForTaskbar();
            }
        }

        private void AdjustForTaskbar()
        {
            // Get the working area which is the screen area excluding the taskbar
            var workingArea = SystemParameters.WorkArea;

            // Set the window size and position to match the working area
            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
            this.Left = workingArea.Left;
            this.Top = workingArea.Top;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
