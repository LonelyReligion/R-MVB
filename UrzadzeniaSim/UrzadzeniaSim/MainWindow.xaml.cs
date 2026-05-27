using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model.RMVB;
using UrzadzeniaSim.Narzedzia;
using UrzadzeniaSim.Widok.Kontrolki;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
        Kontekst ctx = new Kontekst();
        Generatory generator;
        DrzewoRMVB rMVB;

        double wysokosc_okna;
        double szerokosc_okna;

        //na potrzeby UI
        public static ObservableCollection<Urzadzenie_Model> s_UrzadzeniaUI = new ObservableCollection<Urzadzenie_Model>();
        public MainWindow()
        {
            //to powinno byc w repo
            ctx.Database.ExecuteSqlCommand("DELETE FROM Wersjas");
            ctx.Database.ExecuteSqlCommand("DELETE FROM Urzadzenie_Model");
            //


            // Inicjowanie 
            rMVB = new DrzewoRMVB(ctx);

            generator = new Generatory(rMVB.zwrocRepo());
            Wersja.ctx = ctx;
            InDBStorage.s_Ctx = ctx;
            Repo.s_Ctx = ctx;
            Urzadzenie_Model.ctx = ctx;

            PanelBoczny.s_Ctx = ctx;
            PanelBoczny.s_Repozytorium = rMVB.zwrocRepo();

            Urzadzenie_Model.repo = rMVB.zwrocRepo();
            Urzadzenie_Model.rMVB = rMVB;
            PanelBoczny.s_Generator = generator;

            ctx.Urzadzenia.FirstOrDefault();
            //

            InitializeComponent();

            // Inicjowanie Widokow
            pasekNarzedzi.rodzic = this;
            pasekNarzedzi.repo = rMVB.zwrocRepo();
            //

            okno.SizeChanged += (s, e) =>
            {
                wysokosc_okna = okno.ActualHeight;
                szerokosc_okna = okno.ActualWidth;
            };

        }

        private void powiedzOtymSiatce(double powiekszenie)
        {
            Trace.WriteLine("Tu okno, wiem o wszystkim.");
            siatkaWalcowa.powiekszSiatke(powiekszenie);
        }

        private void siatkaZmienPoludniki(bool onoff)
        {
            siatkaWalcowa.zmienDokladnoscPoludniki(onoff);
        }

        private void siatkaZmienRownolezniki(bool onoff)
        {
            siatkaWalcowa.zmienDokladnoscRownolezniki(onoff);
        }
        private void GenerujLosoweUrzadzenie()
        {
            (decimal x, decimal y) = generator.GenerujWspolrzedne();
            Trace.WriteLine("Generujemy nowe urządzenie o współrzędnych: " + x + ", " + y);

            Urzadzenie_Model nowe_urzadzenie = new Urzadzenie_Model(generator.GenerujWspolrzedne());
            System.Threading.Tasks.Task.Run(() => { rMVB.dodajUrzadzenie(nowe_urzadzenie); rMVB.dodajWersje(new Wersja(nowe_urzadzenie.UrzadzenieID, rMVB.zwrocRepo())); }); //zlecamy wykonanie wątkowi w tle, nie blokuje GUI
            siatkaWalcowa.dodajUrzadzenie(nowe_urzadzenie); //zrobic metode ktora doda i przeladuje od razu w wersji dodawanie z listy i dodawanie pojedyncze


            s_UrzadzeniaUI.Add(nowe_urzadzenie);

        }

        private void powiedz_o_znaczeniu_panelowi(int id_urzadzenia)
        {
            panelBoczny.uzupelnijInformacjeOurzadzeniu(id_urzadzenia);
        }

        private void pasekNarzedzi_wyczysc_wszystko()
        {
            rMVB.Reset(); //usuwa z bazy i z lokalnego repo
            siatkaWalcowa.Reset();
        }

        private void pasekNarzedzi_dodaj_urzadzenie((decimal, decimal) obj)
        {
            Urzadzenie_Model nowe_urzadzenie = new Urzadzenie_Model(obj);
            System.Threading.Tasks.Task.Run(() => { rMVB.dodajUrzadzenie(nowe_urzadzenie); rMVB.dodajWersje(new Wersja(nowe_urzadzenie.UrzadzenieID, rMVB.zwrocRepo())); }); //zlecamy wykonanie wątkowi w tle, nie blokuje GUI
            siatkaWalcowa.dodajUrzadzenie(nowe_urzadzenie);

            s_UrzadzeniaUI.Add(nowe_urzadzenie);
        }
    }

}