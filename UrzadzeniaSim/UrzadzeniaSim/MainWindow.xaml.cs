using System.Windows;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
        bool klik = false;
        public MainWindow()
        {
            InitializeComponent();
            przycisk.Content = "Super!";
            //przycisk.
        }
        private void przycisk_click(object sender, RoutedEventArgs e)
        {
            if (!klik)
            {
                tekst.Text = "yay";
            }
            else {
                tekst.Text = "yippiee";
            }
            klik = !klik;
        }
    }
    
}