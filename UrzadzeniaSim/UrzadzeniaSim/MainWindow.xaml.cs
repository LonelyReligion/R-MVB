using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using UrzadzeniaSim.Widok.Kontrolki;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Narzędzia;
namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
        Generatory generator = new Generatory();

        double wysokosc_okna;
        double szerokosc_okna;

        public MainWindow()
        {
            InitializeComponent();

            okno.SizeChanged += (s, e) =>
            {
                wysokosc_okna = okno.ActualHeight;
                szerokosc_okna = okno.ActualWidth;
            };

        }

        private void powiedzOtymSiatce(double powiekszenie) {
            Trace.WriteLine("Tu okno, wiem o wszystkim.");
            siatkaWalcowa.powiekszSiatke(powiekszenie);
        }

        private void siatkaZmienPoludniki(bool onoff) { 
            siatkaWalcowa.zmienDokladnoscPoludniki(onoff);
        }

        private void siatkaZmienRownolezniki(bool onoff) {
            siatkaWalcowa.zmienDokladnoscRownolezniki(onoff);
        }
        private void GenerujLosoweUrzadzenie()
        {
            (decimal x, decimal y) = generator.generujWspolrzedne();
            Trace.WriteLine("Generujemy nowe urządzenie o współrzędnych: " + x + ", " + y);

            Urzadzenie_Model nowe_urzadzenie = new Urzadzenie_Model(0, (x, y));
            siatkaWalcowa.dodajUrzadzenie(nowe_urzadzenie); //zrobic metode ktora doda i przeladuje od razu w wersji dodawanie z listy i dodawanie pojedyncze
        }
    }
    
}