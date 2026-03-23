using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using UrzadzeniaSim.Widok.Kontrolki;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
       
        double wysokosc_okna;
        double szerokosc_okna;

        public MainWindow()
        {
            InitializeComponent();

            okno.SizeChanged += (s, e) =>
            {
                wysokosc_okna = okno.ActualHeight;
                wysokosc_okna = okno.ActualWidth;
            };

            

        }

        private void powiedzOtymSiatce(double powiekszenie) {
            Trace.WriteLine("Tu okno, wiem o wszystkim.");
            siatkaWalcowa.powiekszSiatke(powiekszenie);
        }
    }
    
}