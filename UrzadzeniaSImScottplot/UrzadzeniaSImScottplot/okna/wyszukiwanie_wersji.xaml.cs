using System.ComponentModel;
using System.Diagnostics;
using System.Text;
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

        //do wariantu 3
        public List<Wersja> nadmiarowe_nieodnalezione = new List<Wersja>();
        public DateTime? poczatek;
        public DateTime? koniec;

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
            blad = false;

            if (((bool)poczatku_checkbox.IsChecked))
            {
                poczatek = DateTime.MinValue;
            }
            else
            {
                poczatek = (DateTime?)ui_poczatek.Value;
            }

            if (((bool)konca_checkbox.IsChecked)) { 
                koniec = DateTime.MaxValue;
            }
            else
            {
                koniec = (DateTime?)ui_koniec.Value;

            }


            using (var ctx = new Kontekst())
            {
                
                if (poczatek is null || koniec is null)
                    return;

                var szukane_wersje = new List<Wersja>();
                var szukane_wersje_mvb = new List<Wersja>();

                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < 10; i++)
                {
                    szukane_wersje = new List<Wersja>();

                    if (koniec != DateTime.MaxValue)
                    {
                        szukane_wersje.AddRange(ctx.Wersje.AsNoTracking().Where(u => u.dataOstatniejModyfikacji >= poczatek).Where(u => u.dataWygasniecia < koniec).ToList());
                    }
                    else {
                        szukane_wersje.AddRange(ctx.Wersje.AsNoTracking().Where(u => u.dataOstatniejModyfikacji >= poczatek).ToList());
                    }
                }
                czasBD = sw.ElapsedMilliseconds;
                
                sw = Stopwatch.StartNew();
                for (int i = 0; i < 10; i++)
                {
                    szukane_wersje_mvb = new List<Wersja>();
                    szukane_wersje_mvb.AddRange(_rmvb.szukaj((DateTime)poczatek, (DateTime)koniec));
                }
                czasRMVB = sw.ElapsedMilliseconds;

                if (szukane_wersje.Count != szukane_wersje_mvb.Count)
                {
                    komunikat_bledu = "Wszukiwanie wersji aktualnych w zadanym przedziale czasu od " + ((DateTime)poczatek).Ticks.ToString() + "(tików) do " + ((DateTime)koniec).Ticks.ToString() + " nie powiodło się.";
                    
                    var duplicates = szukane_wersje_mvb
                    .GroupBy(i => i)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key).ToList();

                    //except nie zadziala
                    var nieznalezione = szukane_wersje
                                        .Where(d => !szukane_wersje_mvb.Any(mvb =>
                                            mvb.UrzadzenieID == d.UrzadzenieID &&
                                            mvb.WersjaID == d.WersjaID))
                                        .ToList();
                    int liczba_roznych_urzadzen = szukane_wersje_mvb.DistinctBy(x => new { x.UrzadzenieID, x.WersjaID }).Count();
                    int liczba_urzadzen = szukane_wersje_mvb.Count();


                    if (nieznalezione.Count != 0)
                    {
                        komunikat_bledu += "\nRMVB nie znalazlo następujących urządzeń:";
                        nadmiarowe_nieodnalezione.AddRange(nieznalezione);
                    }
                    else if (szukane_wersje.Count < szukane_wersje_mvb.Count && liczba_roznych_urzadzen == liczba_urzadzen)
                    {
                        komunikat_bledu += "RMVB odnalazlo wiecej urzadzen niz baza";
                    }

                    if (liczba_roznych_urzadzen != liczba_urzadzen)
                    {
                        bledy.Add("RMVB znalazło nadmiarowe (powstarzające się) urządzenia: ");
                        List<Wersja> nadmiarowe = new List<Wersja>(szukane_wersje_mvb);

                        foreach (var elem in szukane_wersje_mvb.Distinct())
                            nadmiarowe.Remove(elem);

                        //zostaja same duble, wychodzi nam cos niemozliwego...
                        nadmiarowe_nieodnalezione.AddRange(nadmiarowe);
                    }

                    blad = true;

                    //_rmvb.szukaj((DateTime)poczatek, (DateTime)koniec);
                }
                else
                {
                    //bez bledu
                    odnalezione_wersje = szukane_wersje;
                }

            }
            sukces = true;
            Close();
        }

        private void poczatku_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            //gdy odczytujamy jest jeszcze niezaznaczone?
            ui_poczatek.IsReadOnly = ((bool)poczatku_checkbox.IsChecked);
        }

        private void konca_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            ui_koniec.IsReadOnly = ((bool)konca_checkbox.IsChecked);
        }
    }
}
