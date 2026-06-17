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
    /// Logika interakcji dla klasy TworzenieLosowychUrzadzen.xaml
    /// </summary>
    public partial class TworzenieLosowychUrzadzen : Window
    {
        public bool sukces = false;
        public int ileUrzadzen = 0;
        public TworzenieLosowychUrzadzen()
        {
            InitializeComponent();
        }

        private void Przeslij_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            ileUrzadzen = (int)Liczba.Value;
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private void Liczba_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Liczba.Value != null)
                Przeslij.IsEnabled = true;
            else
                Przeslij.IsEnabled = false;
        }
    }
}
