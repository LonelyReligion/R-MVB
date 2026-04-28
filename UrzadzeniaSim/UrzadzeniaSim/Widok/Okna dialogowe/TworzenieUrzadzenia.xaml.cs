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

namespace UrzadzeniaSim.Widok.Okna_dialogowe
{
    /// <summary>
    /// Logika interakcji dla klasy TworzenieUrzadzenia.xaml
    /// </summary>
    public partial class TworzenieUrzadzenia : Window
    {
        bool sukces = false; //czy otrzymalismy poprawne dane
        public TworzenieUrzadzenia()
        {
            InitializeComponent();
        }

        private void Przeslij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
