using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    
    /// <summary>
    /// Logika interakcji dla klasy PasekNarzedzi.xaml
    /// </summary>
    public partial class PasekNarzedzi : UserControl
    {
        public double powiekszenie;
        public event Action<double> zmienionoPowiekszenie;

        public PasekNarzedzi()
        {
            InitializeComponent();
            Powiekszenie.ItemsSource = new List<string> { "100%", "150%", "200%" };
            Powiekszenie.SelectedIndex = 0;
        }

        private void zmianaPowiekszenia(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            string procenty = Powiekszenie.SelectedItem?.ToString();
            int procenty_i = int.Parse(procenty.Substring(0, 3));
            Trace.WriteLine("Zmieniono powiekszenie na " + procenty_i);
            powiekszenie = (double) procenty_i / 100;
            zmienionoPowiekszenie?.Invoke(powiekszenie);
        }

    }
}
