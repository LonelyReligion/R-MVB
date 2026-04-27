using System.Diagnostics;
using System.Windows;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Narzedzia;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model.RMVB;
using UrzadzeniaSim.Widok.Kontrolki;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
        Kontekst ctx = new Kontekst();
        Repo repozytorium = new Repo();
        Generatory generator;
        RMVB rMVB;
        
        double wysokosc_okna;
        double szerokosc_okna;

        public MainWindow()
        {
            ctx.Database.ExecuteSqlCommand("delete from Urzadzenie_Model");

            InitializeComponent();

            // Inicjowanie 
            generator = new Generatory(repozytorium);
            Wersja.ctx = ctx;
            InDBStorage.ctx = ctx;
            Repo.ctx = ctx;
            Urzadzenie_Model.ctx = ctx;
            rMVB = new RMVB(ctx);
            PanelBoczny.ctx = ctx;

            ctx.Urzadzenia.FirstOrDefault();
            //

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

            Urzadzenie_Model nowe_urzadzenie = new Urzadzenie_Model(generator.generujWspolrzedne());
            Task.Run(() => rMVB.dodajUrzadzenie(nowe_urzadzenie)); //zlecamy wykonanie wątkowi w tle, nie blokuje GUI
            siatkaWalcowa.dodajUrzadzenie(nowe_urzadzenie); //zrobic metode ktora doda i przeladuje od razu w wersji dodawanie z listy i dodawanie pojedyncze
        }

        private void powiedz_o_znaczeniu_panelowi(int id_urzadzenia) {
            panelBoczny.uzupelnijInformacjeOurzadzeniu(id_urzadzenia);
        }

        private void pasekNarzedzi_wyczysc_wszystko()
        {
            rMVB.Reset(); //usuwa z bazy i z lokalnego repo
            siatkaWalcowa.Reset();
        }
    }
    
}