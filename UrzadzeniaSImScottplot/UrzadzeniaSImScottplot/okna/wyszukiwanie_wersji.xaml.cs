using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.RightsManagement;
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

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy wyszukiwanie_wersji.xaml
    /// </summary>
    public partial class wyszukiwanie_wersji : Window, INotifyPropertyChanged
    {
        // te zmienne przechowuja efekty dzialania okna
        public bool sukces = false;
        public bool blad = false;

        public List<string> bledy = new List<string>();
        public String komunikat_bledu;

        public List<Wersja> odnalezione_wersje = new List<Wersja>();
        public long czasBD;
        public long czasRMVB;
        //

        private RMVB _rmvb;

        public event PropertyChangedEventHandler? PropertyChanged;

        private int? _maxId = null;
        public int? maxId
        {
            get { return _maxId; }
            set
            {
                _maxId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("maxId"));
            }

        }

        void zaktualizujWersje(int id)
        {
            using (var ctx = new Kontekst()) {        
                Urzadzenie oMaxId = ctx.Urzadzenia.Where(u => u.UrzadzenieID == id).First();
                
                maxVer = oMaxId.Wersje.Last().WersjaID; 
                minVer = oMaxId.Wersje.First().WersjaID;
                idWersji.Value = minVer;

            }

        }

        private void _inicjujKontrolki()
        {
            using (var ctx = new Kontekst())
            {
                //maksymalne id urzadzenia
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);
                if (maxId != null) {
                    zaktualizujWersje((int)maxId);
                }
            }
        }

        public wyszukiwanie_wersji(RMVB rmvb)
        {
            _rmvb = rmvb;
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
        }

        private void _sprawdzCzyMamyUrzadzenia(object sender, EventArgs e)
        {
            using (var ctx = new Kontekst())
            {
                bool istnieje = ctx.Urzadzenia.Any();

                if (!istnieje)
                {
                    Window dialog = (Window)new brak_urzadzen_w_bazie(this);
                    dialog.ShowDialog();
                    Close();
                }
            }
        }


        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            //max id wersji dla urzadzenia o danym id, zaciagnac z bazy
            //maxVer =

            int Id = (int)IdUrzadzenia2.Value;

            if (Id != null)
            {
                zaktualizujWersje(Id);
            }

        }

        private int? _maxVer = null;
        //aktualizowac po zmianie wartosci id-ka
        public int? maxVer
        {
            get { return _maxVer; }
            set
            {
                _maxVer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("maxVer"));
            }

        }

        private int? _minVer = null;
        //aktualizowac po zmianie wartosci id-ka
        public int? minVer
        {
            get { return _minVer; }
            set
            {
                _minVer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("minVer"));
            }

        }

        private void Anuluj1_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        
        private void Anuluj2_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }
        private void Anuluj3_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        //po urzadzeniu
        private void Przeslij1_Click(object sender, RoutedEventArgs e)
        {
            bool blad_baza = false;
            bool blad_mvb = false;

            int szukane_id = (int)IdUrzadzenia1.Value;

            Wersja? szukana_bd = null;
            Wersja? szukana_rmvb = null;

            Stopwatch sw = Stopwatch.StartNew();

            komunikat_bledu = "";
           
            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    int id = szukane_id;
                    szukana_bd = ctx.Wersje
                        .AsNoTracking() //nie uzywamy zbuforowanych (wynikow poprzednich wykonan)
                        .Where(u => u.UrzadzenieID == id)
                        .OrderByDescending(u => u.WersjaID)
                        .FirstOrDefault();

                    if (szukana_bd == null && !blad_baza)
                    {
                        komunikat_bledu += "Baza nie odnalazla rekordu o id " + id + ". ";
                        blad_baza = true;
                    }

                }
            }
            czasBD = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                szukana_rmvb = _rmvb.szukaj(szukane_id);
                if (szukana_rmvb == null)
                {
                    komunikat_bledu += "RMVB nie odnalazlo urzadzenia o id " + szukane_id + ".";
                    blad_mvb = true;
                }
            }

            czasRMVB = sw.ElapsedMilliseconds;

            if (szukana_rmvb != szukana_bd) //wlasny operator zrobic poronania jezeli nie zadziala....
            {
                komunikat_bledu += "RMVB i baza danych odnalazły różne wersje urządzenia.";
                blad_mvb = true;
                blad_baza = true;
            }


            if (blad_baza || blad_mvb)
            {
            
            }
            else
            {
                //sukces
                odnalezione_wersje.Add(szukana_rmvb);
            }

            blad = blad_baza || blad_mvb;
            sukces = true;
            Close();
        }

        //po urzazdzeniu i wersji
        private void Przeslij2_Click(object sender, RoutedEventArgs e)
        {

            /*//doprowadzic spowrotem do porzadku (losowe)
            List<(int, int)> szukane_id_v = new List<(int, int)>();
            for (int i = 0; i < 10; i++)
            {
                int losowe_urzadzenie_id = repo.pobierzUrzadzenia().ElementAt(rnd.Next(repo.pobierzUrzadzenia().Count - 1)).Value.UrzadzenieID;
                int losowa_wersja_id = repo.zwroc_urzadzenie_wersje().ElementAt(losowe_urzadzenie_id).Value.ElementAt(rnd.Next(repo.zwroc_urzadzenie_wersje().ElementAt(losowe_urzadzenie_id).Value.Count - 1));
                szukane_id_v.Add((losowe_urzadzenie_id, losowa_wersja_id));
            }

            //>
            List<Wersja?> znalezione_baza = new List<Wersja>();
            sw = Stopwatch.StartNew();

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < szukane_id_v.Count(); i++)
                {
                    znalezione_baza.Add(null);
                    int id = szukane_id_v[i].Item1;
                    int v = szukane_id_v[i].Item2;

                    znalezione_baza[i] = ctx.Wersje
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UrzadzenieID == id && u.WersjaID == v);

                    if (znalezione_baza[i] == null)
                    {
                        Console.WriteLine("Uwaga: Baza nie odnalazla rekordu.");
                        blad = true;
                    }
                }
            }
            long czas_baza = sw.ElapsedMilliseconds;


            sw = Stopwatch.StartNew();
            List<Wersja?> znalezione_rmvb = new List<Wersja?>();
            for (int i = 0; i < szukane_id_v.Count(); i++)
            {
                znalezione_rmvb.Add(null);
                int id = szukane_id_v[i].Item1;
                int v = szukane_id_v[i].Item2;
                znalezione_rmvb[i] = rmvb.szukaj(id, v);

                if (znalezione_rmvb[i] == null)
                {
                    Console.WriteLine("Uwaga: RMVB nie odnalazlo rekordu.");
                    //do debuggowania
                    //znalezione_rmvb[i] = rmvb.szukaj(id, v);
                    blad = true;
                }
            }
            long czas_mvb = sw.ElapsedMilliseconds;
            if (!blad)
            {
                Console.WriteLine("CZAS WYKONANIA: baza: " + czas_baza + " rmvb: " + czas_mvb);
                wyniki.Add("MVB | wyszukiwanie losowych urządzeń po id i wersji | " + czas_baza + " | " + czas_mvb);
            }
            else
            {
                bledy.Add("Działanie testów zakończyło się na wyszukiwaniu wersji urządzenia o określonym id oraz numerze wersji. Kolejne testy nie zostały wykonane, poprzednie zostały zrealizowane pomyślnie. ");
                bledy.Add("Komunikat(y) błędu(ów): \n");

                for (int i = 0; i < 10; i++)
                {
                    if (znalezione_baza[i] == null && znalezione_rmvb[i] == null)
                    {
                        Console.WriteLine("Nie odnaleziono urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                        bledy.Add("Nie odnaleziono urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                    }
                    else if (znalezione_baza[i] == null)
                    {
                        Console.WriteLine("Baza nie odnalazła urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                        bledy.Add("Baza nie odnalazła urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                    }
                    else if (znalezione_rmvb[i] == null)
                    {
                        Console.WriteLine("RMVB nie odnalazło urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                        bledy.Add("MVB nie odnalazło urzadzenia o id " + szukane_id_v[i].Item1 + " i wersji " + szukane_id_v[i].Item2);
                    }
                    bledy.Add("");
                }
            }
*/
            sukces = true;
            Close();
        }

        //po datach
        private void Przeslij3_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            Close();
        }
    }
}
