using System.Windows;

namespace UrzadzeniaSImScottplot
{
    /// <summary>
    /// Logika interakcji dla klasy brak_urzadzen_w_bazie.xaml
    /// </summary>
    public partial class brak_urzadzen_w_bazie : Window
    {
        public brak_urzadzen_w_bazie(Window rodzic)
        {
            InitializeComponent();
            Loaded += _wydajDzwiek;
            Owner = rodzic;

        }

        private void _wydajDzwiek(object sender, RoutedEventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play(); 
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
