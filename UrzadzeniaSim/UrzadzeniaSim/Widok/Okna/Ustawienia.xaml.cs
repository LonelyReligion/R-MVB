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
using UrzadzeniaSim.Model.RMVB.MVB;
using UrzadzeniaSim.Narzedzia;

namespace UrzadzeniaSim.Widok.Okna
{
    /// <summary>
    /// Logika interakcji dla klasy Ustawienia.xaml
    /// </summary>
    public partial class Ustawienia : Window
    {
        public Ustawienia()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

            granicaPrzezywalnosci.Value = Korzen.granica_przezywalnosci;
            minLiczbaUrzadzen.Value = Korzen.min_urzadzen_korzen;
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {
            Korzen.granica_przezywalnosci = (decimal)granicaPrzezywalnosci.Value;
            Korzen.min_urzadzen_korzen = (int)minLiczbaUrzadzen.Value;
            Close();
        }
    }
}
