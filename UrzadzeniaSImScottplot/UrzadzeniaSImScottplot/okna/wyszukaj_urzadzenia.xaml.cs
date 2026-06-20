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
    /// Logika interakcji dla klasy wyszukaj_urzadzenia.xaml
    /// </summary>
    public partial class wyszukaj_urzadzenia : Window
    {
        bool sukces = false;
        public wyszukaj_urzadzenia()
        {
            InitializeComponent();
        }

        private void anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private void przeslij_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            Close();
        }
    }
}
