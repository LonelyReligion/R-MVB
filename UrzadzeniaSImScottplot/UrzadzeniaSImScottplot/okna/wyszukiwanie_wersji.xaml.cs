using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

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

        public int wariant_tesktu = 0;

        //do wariantu 0 i 1
        public int? szukane_id = null;
        //do wariantu 1
        public int? szukane_v = null;

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
            wariant_tesktu = 0;

            bool blad_baza = false;
            bool blad_mvb = false;

            szukane_id = IdUrzadzenia1.Value;

            Wersja? szukana_bd = null;
            Wersja? szukana_rmvb = null;

            Stopwatch sw = Stopwatch.StartNew();

            komunikat_bledu = "";
           
            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    int id = (int)szukane_id;
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
                szukana_rmvb = _rmvb.szukaj((int)szukane_id);
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
            wariant_tesktu = 1;

            szukane_id = (int)IdUrzadzenia2.Value;
            szukane_v = (int)idWersji.Value;

            Wersja? znaleziona_baza = null;
            bool blad_bd = false;
            bool blad_rmvb = false;
            
            Stopwatch sw = Stopwatch.StartNew();

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    
                    var znalezione = ctx.Wersje
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UrzadzenieID == szukane_id && u.WersjaID == szukane_v);

                    
                    if (znalezione is null && !blad_bd)
                    {
                        blad_bd = true;
                    }
                    else
                    {
                        znaleziona_baza = znalezione;
                    }
                }
            }
            czasBD = sw.ElapsedMilliseconds;


            sw = Stopwatch.StartNew();
            Wersja? znaleziona_rmvb = null;
            for (int i = 0; i < 10; i++)
            {
                znaleziona_rmvb = null;

                znaleziona_rmvb = _rmvb.szukaj((int)szukane_id, (int)szukane_v);
                
                if (znaleziona_rmvb is null && !blad_rmvb)
                {
                   //do debuggowania
                    //znalezione_rmvb[i] = rmvb.szukaj(szukane_id, szukane_v);
                    blad_rmvb = true;
                }
            }
            czasRMVB = sw.ElapsedMilliseconds;

            blad = blad_bd || blad_rmvb;

            if (!blad)
            {
                //to ze nie ma null sprawdzilismy wyzej
                if(znaleziona_baza != null)
                    odnalezione_wersje.Add((Wersja)znaleziona_baza);
            }
            else
            {
                komunikat_bledu += "Wyszukiwanie wersji o UrzązenieID: " + szukane_id + " i WersjaID: " + szukane_v + " nie powiodło się.\n";
                komunikat_bledu += "Komunikat(y) błędu(ów): \n";


                if (znaleziona_baza is null && znaleziona_rmvb is null)
                {
                    bledy.Add("Baza i RMVB nie odnalazły wersji urzadzenia.");
                }
                else if (znaleziona_baza is null)
                {
                    bledy.Add("Baza nie odnalazła wersji urzadzenia.");
                }
                else if (znaleziona_rmvb is null)
                {
                    bledy.Add("RMVB nie odnalazło wersji urzadzenia.");
                }

            }

            sukces = true;
            Close();
        }

        //po datach
        private void Przeslij3_Click(object sender, RoutedEventArgs e)
        {
            wariant_tesktu = 2;
            bool blad = false;
            //najwczesniejsza data poczatku


            using (var ctx = new Kontekst())
            {
                DateTime poczatek = (DateTime)ui_poczatek.SelectedDate.Value;
                DateTime koniec_nie_9999 = (DateTime)ui_koniec.SelectedDate.Value;

                List<(DateTime, DateTime)> losowe_przedzialy = new List<(DateTime, DateTime)>(){ (poczatek, koniec_nie_9999) };


                /*var szukane_wersje = new List<List<Wersja>>();
                var szukane_wersje_mvb = new List<List<Wersja>>();

                //Console.WriteLine(poczatek.Ticks + "-" + koniec.Ticks);
                sw = Stopwatch.StartNew();
                for (int i = 0; i < ileRazy; i++)
                {
                    szukane_wersje.Add(new List<Wersja>());
                    DateTime start = losowe_przedzialy[i].Item1;
                    DateTime end = losowe_przedzialy[i].Item2;
                    szukane_wersje[i].AddRange(ctx.Wersje.AsNoTracking().Where(u => u.dataOstatniejModyfikacji >= start).Where(u => u.dataWygasniecia < end).ToList());
                }
                long czas_baza = sw.ElapsedMilliseconds;
                Console.WriteLine("Baza: " + szukane_wersje.Count + " w czasie: " + czas_baza + " ms.");

                sw = Stopwatch.StartNew();
                for (int i = 0; i < ileRazy; i++)
                {
                    szukane_wersje_mvb.Add(new List<Wersja>());
                    DateTime start = losowe_przedzialy[i].Item1;
                    DateTime end = losowe_przedzialy[i].Item2;
                    szukane_wersje_mvb[i].AddRange(rmvb.szukaj(start, end));
                }
                long czas_mvb = sw.ElapsedMilliseconds;
                Console.WriteLine("RMVB: " + szukane_wersje_mvb.Count + " w czasie: " + czas_mvb + " ms.");

                for (int i = 0; i < ileRazy; i++)
                {
                    if (szukane_wersje[i].Count != szukane_wersje_mvb[i].Count)
                    {
                        bledy.Add("Działanie testów zakończyło się na wyszukiwaniu wersji aktualnych w zadany przedziale czasu. Kolejne testy nie zostały wykonane, poprzednie zostały zrealizowane pomyślnie. ");
                        bledy.Add("Przedzial: " + losowe_przedzialy[i].Item1.Ticks + "-" + losowe_przedzialy[i].Item2.Ticks);
                        Console.WriteLine("Przedzial: " + losowe_przedzialy[i].Item1.Ticks + "-" + losowe_przedzialy[i].Item2.Ticks);
                        bledy.Add("Komunikat(y) błędu(ów): \n");

                        var duplicates = szukane_wersje_mvb
                        .GroupBy(i => i)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key).ToList();
                        //except nie zadziala
                        var nieznalezione = szukane_wersje[i]
                                            .Where(d => !szukane_wersje_mvb[i].Any(mvb =>
                                                mvb.UrzadzenieID == d.UrzadzenieID &&
                                                mvb.WersjaID == d.WersjaID))
                                            .ToList();
                        int liczba_roznych_urzadzen = szukane_wersje_mvb[i].DistinctBy(x => new { x.UrzadzenieID, x.WersjaID }).Count();
                        int liczba_urzadzen = szukane_wersje_mvb[i].Count();


                        if (nieznalezione.Count != 0)
                        {
                            bledy.Add("MVB znalazlo następujących urządzeń: ");
                            Console.WriteLine("Nie znaleziono następujących urządzeń: ");
                            foreach (var u in nieznalezione)
                            {
                                Console.WriteLine("BAZA: " + u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
                                Console.WriteLine("MVB: " + u.UrzadzenieID + "v" + u.WersjaID + " " + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataOstatniejModyfikacji.Ticks + "(" + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataOstatniejModyfikacji + ")"
                                    + "-" + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataWygasniecia.Ticks + "(" + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataWygasniecia + ")");

                                bledy.Add(u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
                            }
                        }
                        else if (szukane_wersje[i].Count < szukane_wersje_mvb[i].Count && liczba_roznych_urzadzen == liczba_urzadzen)
                        {
                            Console.WriteLine("MVB odnalazlo wiecej urzadzen niz baza...");
                        }

                        if (liczba_roznych_urzadzen != liczba_urzadzen)
                        {
                            bledy.Add("MVB znalazło nadmiarowe (powstarzające się) urządzenia: ");
                            Console.WriteLine("Znaleziono nadmiarowe urządzenia: ");
                            List<Wersja> nadmiarowe = new List<Wersja>(szukane_wersje_mvb[i]);

                            foreach (var elem in szukane_wersje_mvb[i].Distinct())
                                nadmiarowe.Remove(elem);

                            //zostaja same duble, wychodzi nam cos niemozliwego...
                            foreach (var u in nadmiarowe)
                            {
                                Console.WriteLine("BAZA: " + u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
                                Console.WriteLine("MVB: " + u.UrzadzenieID + "v" + u.WersjaID + " " + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataOstatniejModyfikacji.Ticks + "(" + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataOstatniejModyfikacji + ")" +
                                    "-" + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataWygasniecia.Ticks + "(" + rmvb.szukaj(u.UrzadzenieID, u.WersjaID).dataWygasniecia.Ticks + ")");

                                bledy.Add(u.UrzadzenieID + "v" + u.WersjaID + " " + u.dataOstatniejModyfikacji.Ticks + "-" + u.dataWygasniecia.Ticks);
                            }
                        }

                        blad = true;

                        DateTime start = losowe_przedzialy[i].Item1;
                        DateTime end = losowe_przedzialy[i].Item2;
                        rmvb.szukaj(start, end);
                    }
                    else
                    {
                        //bez bledu
                    }
                }*/
            }
            sukces = true;
            Close();
        }
    }
}
