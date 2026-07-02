using System;
using System.Collections.Generic;
using System.Linq;
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
