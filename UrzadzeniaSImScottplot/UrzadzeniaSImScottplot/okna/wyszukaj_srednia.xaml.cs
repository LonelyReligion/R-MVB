using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using UrzadzeniaSImScottplot.kontrolki;

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy wyszukaj_srednia.xaml
    /// </summary>
    public partial class wyszukaj_srednia : Window, INotifyPropertyChanged
    {
        public bool sukces = false;
        public bool blad = false;


        public event PropertyChangedEventHandler? PropertyChanged;
        private RMVB _rmvb;

        public List<string> bledy = new List<string>();
        public String komunikat_bledu;

        public decimal czasBD;
        public decimal czasRMVB;
        public decimal srednia;
        public Rectangle szukany;
        public int id = -1;

        public int wariant = 0; //false to prostokat, a true to urzadzenie 
        
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
        
        private void _inicjujKontrolki() {
            using (var ctx = new Kontekst()) {
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);
            }
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
        public wyszukaj_srednia(RMVB rmvb)
        {
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
            _rmvb = rmvb;

            this.Loaded += Wyszukaj_srednia_Loaded;
        }

        private void Wyszukaj_srednia_Loaded(object sender, RoutedEventArgs e)
        {

            using (var ctx = new Kontekst())
            {
                if (!ctx.Urzadzenia.Any())
                    return;

                Urzadzenie zerowe = ctx.Urzadzenia.Where(u => u.UrzadzenieID == 0).First();
                dlugosc.Content = zerowe.Dlugosc.ToString();
                szerokosc.Content = zerowe.Szerokosc.ToString();
            }
        }

        private void Przeslij1_Click(object sender, RoutedEventArgs e) //wariant z prostokatem
        {
            wariant = 0;
            szukany = new Rectangle(prostkat.Ymin, prostkat.Xmin, prostkat.Ymax, prostkat.Xmax);

            List<Decimal> resultDB = new List<Decimal>();
            List<Decimal> resultRTree = new List<Decimal>();
            List<string> Out = new List<string>();

            Stopwatch sw;
            sw = Stopwatch.StartNew();
            List<int> ile = new List<int>();
            int cnt_1 = 0;

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    Decimal x1 = szukany.XMin;
                    Decimal y1 = szukany.YMin;
                    Decimal x2 = szukany.XMax;
                    Decimal y2 = szukany.YMax;

                    ile.Add(0);

                    int cnt = 0;
                    resultDB.Add(0);

                    List<int> ids = new List<int>();

                    ids.AddRange(ctx.Urzadzenia
                    .AsNoTracking()
                    //contains
                    .Where(u => x1 <= u.Dlugosc)
                    .Where(u => y1 <= u.Szerokosc)
                    .Where(u => x2 >= u.Dlugosc)
                    .Where(u => y2 >= u.Szerokosc)
                    .Select(o => o.UrzadzenieID)
                    .ToList());


                    Out.Add("(");

                    List<Pomiar> aktualne_pomiary = ctx.Pomiary.AsNoTracking().Where(p => ids.Contains(p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID))
                        .Where(p => p.dtpomiaru > new DateTime(2024, 7, 18, 0, 0, 0)).ToList();

                    foreach (Pomiar p in aktualne_pomiary)
                    {
                        ile[i]++;
                        resultDB[i] += p.Wartosc;
                        Out[i] += p.Wartosc + "+";
                    }

                    if (ile[i] != 0)
                        resultDB[i] /= ile[i];
                    else
                        resultDB[i] = 0;
                    cnt_1 = cnt;
                }
                czasBD = sw.ElapsedMilliseconds;
            }

            sw = Stopwatch.StartNew();
            int cnt_r = 0;
            List<Decimal> ile_r = new List<Decimal>();
            for (int i = 0; i < 10; i++)
            {
                (Decimal liczba_elementow, Decimal srednia) = _rmvb.szukajAgregatu(szukany);
                resultRTree.Add(srednia);
                ile_r.Add(liczba_elementow);

            }
            czasRMVB = sw.ElapsedMilliseconds;

            if (ile[0] != ile_r[0])
            {
                if (blad == false) //powinno wykonać się tylko raz :)
                {
                    komunikat_bledu = "Wyszukiwanie agregatu powierzchniowego dla obszaru: xMin(" + szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.XMax + "), " + "yMax(" + szukany.YMax + ") nie powiodło się. \nKomunikat(y) błędu(ów): ";
                }
                Console.WriteLine("Rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[0] + " (baza) " +
                    ile_r[0] + " (r)");
                blad = true;
                   
                bledy.Add("Rozbieznosc miedzy liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[0] + " (baza) " +
                    ile_r[0] + " (r)");

                if (resultDB[0] == resultRTree[0])
                {
                    Console.WriteLine("Obliczone wartości SĄ ZBIEŻNE");
                    bledy.Add("Obliczone wartości SĄ ZBIEŻNE");
                }
                else
                {
                    bledy.Add("Obliczone wartości: " + "Recznie: " + resultDB[0] + " vs " + "RMVB: " + resultRTree[0] + "\n");
                }
            }

            else if (resultDB[0] != resultRTree[0])
            {
                if (blad == false) //powinno wykonać się tylko raz :)
                {
                    komunikat_bledu = "Wyszukiwanie agregatu powierzchniowego dla obszaru: xMin(" + +szukany.XMin + "), " + "yMin(" + szukany.YMin + "), " +
                    "xMax(" + szukany.XMax + "), " + "yMax(" + szukany.YMax + ") nie powiodło się. \nKomunikat(y) błędu(ów): ";
                }

                Console.WriteLine("Rozbieznosc miedzy wartościami agregatu: " + resultDB[0] + " (baza) " +
                    resultRTree[0] + " (r)");
                //rmvb.szukajAgregatu(szukany);
                blad = true;
                    
                bledy.Add("Rozbieznosc miedzy wartościami agregatu: " + "Recznie: " + resultDB[0] + " vs " + "RMVB: " + resultRTree[0]);
                bledy.Add("Liczba pomiarow wykorzystanych do policzenia agregatu: " + ile[0]);

            }
            

            srednia = resultRTree[0]; // jezeli !blad to sa rowne

            sukces = true;
            Close();
        }

        private void Przeslij2_Click(object sender, RoutedEventArgs e)//wariant z id
        {
            wariant = 1;
            sukces = true;
            (Decimal, Decimal) wspolrzedne = (Convert.ToDecimal(dlugosc.Content), Convert.ToDecimal(szerokosc.Content));
            
            Decimal wynikBD = 0;
            Decimal wynikRMVB = 0;

            Stopwatch sw;
            int cnt_1 = 0;

            sw = Stopwatch.StartNew();
            int liczby = 0;

            using (var ctx = new Kontekst())
            {
                for (int i = 0; i < 10; i++)
                {
                    wynikBD = 0;
                    liczby = 0;

                    (Decimal xx, Decimal yy) = wspolrzedne;
                    
                    id = ctx.Urzadzenia
                        .AsNoTracking()
                        .Where(u => u.Szerokosc == yy)
                        .Where(u => u.Dlugosc == xx)
                        .First()
                        .UrzadzenieID;

                    if (id != -1)
                    {
                        List<Pomiar> pomiary = ctx.Pomiary
                                                .AsNoTracking()
                                                .Where(p => p.WersjeUrzadzenia.FirstOrDefault().UrzadzenieID == id)
                                                .ToList();
                        liczby += pomiary.Count;
                        foreach (Pomiar p in pomiary) wynikBD += p.Wartosc;

                        if (liczby != 0)
                            wynikBD /= liczby;
                        else
                            wynikBD = 0;
                    }
                    else
                    {
                        Console.WriteLine("Urzadzenie o wsp. " + xx + " " + yy + " nie istnieje w bazie");
                        blad = true;
                    }
                }
            }
            long czasBD = sw.ElapsedMilliseconds;

            sw = Stopwatch.StartNew();

            List<Urzadzenie> resDevices = new List<Urzadzenie>();
            int liczba = 0;
            sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                (Decimal xx, Decimal yy) = wspolrzedne;
                (liczba, wynikRMVB) =_rmvb.szukajAgregatuCzasowego(xx, yy);
            }
            long czas = sw.ElapsedMilliseconds;

            (Decimal x, Decimal y) = wspolrzedne;

            if (wynikBD != wynikRMVB || liczba != liczby)
            {
                if (!blad)
                {
                    komunikat_bledu = "Wyszukiwanie agregatu czasowego nie powiodło się. \nKomunikat(y) błędu(ów): ";
                }
                blad = true;

                if (wynikBD != wynikRMVB)
                {
                    bledy.Add("Mamy rozbieznosc miedzy obliczonymi wartościami: " + wynikRMVB + "(R) " + wynikBD + "(ręcznie)");
                }

                if (liczba != liczby)
                {
                    bledy.Add("Mamy rozbieznosc miedzy liczbą pomiarow wykorzystanych do policzenia agregatu czasowego urządzenia o współrzędnych: (" + wspolrzedne.Item1 + "," + wspolrzedne.Item2 + ") i id: " + id);
                    bledy.Add(liczby + " (baza) " + liczba + " (r)");

                }

            }
        
            srednia = wynikRMVB;

            sukces = !blad;
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private void id_urzadzenia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            using (var ctx = new Kontekst()) {
                Urzadzenie wybrane = ctx.Urzadzenia.Where(u => u.UrzadzenieID == (int)id_urzadzenia.Value).First();
                dlugosc.Content = wybrane.Dlugosc.ToString(); 
                szerokosc.Content = wybrane.Szerokosc.ToString();
            }
        }

        private DateTime? _poczatek = null;

        public DateTime? poczatek { 
            get { return _poczatek; }
            set
            {
                _poczatek = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("poczatek"));
            }
        }

        private DateTime? _koniec = null;
        public DateTime? koniec
        {
            get { return _koniec; }
            set
            {
                _koniec = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("koniec"));
            }
        }

        private void konca_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if ((bool)konca_checkbox.IsChecked)
            {
                ui_koniec.IsReadOnly = true;
                koniec = DateTime.MaxValue;
            }
            else
            {
                ui_koniec.IsReadOnly = false;
                koniec = ui_koniec.Value;
            }
        }

        private void poczatku_checkbox_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded)
                return;

            if ((bool)poczatku_checkbox.IsChecked)
            {
                ui_poczatek.IsReadOnly = true;
                poczatek = DateTime.MinValue;
            }
            else
            {
                ui_poczatek.IsReadOnly = false;
                poczatek = ui_poczatek.Value;
            }
        }

        private void Przeslij3_Click(object sender, RoutedEventArgs e)
        {
            wariant = 2;
            blad = false;
            szukany = new Rectangle(prostkatv2.Ymin, prostkatv2.Xmin, prostkatv2.Ymax, prostkatv2.Xmax);

            List<Rectangle> szukane_prostokaty = new List<Rectangle>();
            for (int i = 0; i < 10; i++)
                szukane_prostokaty.Add(szukany);

            if (poczatek == null || koniec == null) {
                return;
            }

            using (var ctx = new Kontekst())
            {
                Random rnd = new Random();
                
                List<decimal> wyniki_bd = new List<decimal>();
                List<int> liczby_pomiarow_bd = new List<int>();
                List<int> liczby_urzadzen_bd = new List<int>();

                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < 10; i++)
                {
                    Rectangle rect = szukane_prostokaty[i];
                    List<Urzadzenie> urzadzenia_w_prostokacie = ctx.Urzadzenia
                    .AsNoTracking()
                    .Where(u => rect.XMin <= u.Dlugosc)
                    .Where(u => rect.YMin <= u.Szerokosc)
                    .Where(u => rect.XMax >= u.Dlugosc)
                    .Where(u => rect.YMax >= u.Szerokosc)
                    .ToList();

                    List<int> id_urzadzen = urzadzenia_w_prostokacie.Select(u => u.UrzadzenieID).ToList();
                    liczby_urzadzen_bd.Add(id_urzadzen.Count);

                    List<Wersja> z_okresu = ctx.Wersje
                        .Where(w => id_urzadzen.Contains(w.UrzadzenieID))
                        .Where(p => p.dataOstatniejModyfikacji >= poczatek)
                        .Where(p => p.dataWygasniecia < koniec)
                        .GroupBy(w => w.UrzadzenieID) //ale tylko najnowsza wersja urządzenia spełniająca warunek
                        .Select(g => g.OrderByDescending(w => w.WersjaID).FirstOrDefault())
                        .ToList();

                    decimal suma = 0;
                    decimal liczba_pomiarow = 0;
                    decimal srednia = 0;

                    foreach (var wersja in z_okresu)
                    {
                        foreach (var pomiar in wersja.Pomiary)
                        {
                            if (pomiar.dtpomiaru >= poczatek && pomiar.dtpomiaru < koniec)
                            {
                                suma += pomiar.Wartosc;
                                liczba_pomiarow++;
                            }
                        }
                    }

                    if (liczba_pomiarow != 0)
                        srednia = suma / liczba_pomiarow;


                    wyniki_bd.Add(srednia);
                    liczby_pomiarow_bd.Add((int)liczba_pomiarow);
                }
                czasBD = sw.ElapsedMilliseconds;

                List<decimal> wyniki_rmvb = new List<decimal>();
                List<int> liczby_pomiarow_rmvb = new List<int>();
                List<int> liczby_urzadzen_rmvb = new List<int>();

                sw = Stopwatch.StartNew();
                for (int i = 0; i < 10; i++)
                {
                    (int liczba_urzadzen, int liczba_pomiarow, decimal srednia) = _rmvb.zwrocLiczbeUrzadzenPomiarowSrednia(szukane_prostokaty[i], (DateTime)poczatek, (DateTime)koniec);

                    wyniki_rmvb.Add(srednia);
                    liczby_pomiarow_rmvb.Add(liczba_pomiarow);
                    liczby_urzadzen_rmvb.Add(liczba_urzadzen);
                }
                czasRMVB = sw.ElapsedMilliseconds;
                srednia = wyniki_rmvb[0];

                bool pierwszy = false;
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Wyszukiwanie średniej od " + (DateTime)poczatek + " do " + (DateTime)koniec);

                    if (wyniki_bd[i] != wyniki_rmvb[i])
                    {
                        if (!pierwszy) {
                            komunikat_bledu = "Wyszukiwanie sredniej sie nie powiodlo."; //uzupelnic szczegoly
                            pierwszy = true;
                        }

                        Console.WriteLine("Wyniki sie nie zgadzaja " + wyniki_bd[i] + " vs " + wyniki_rmvb[i]); //poprawic zeby ten blad cokolwiek mowil
                        Console.WriteLine("Liczba pomiarow " + liczby_pomiarow_bd[i] + " vs " + liczby_pomiarow_rmvb[i]);
                        Console.WriteLine("Liczba urządzen " + liczby_urzadzen_bd[i] + " vs " + liczby_urzadzen_rmvb[i]);
                        blad = true;

                        Rectangle rect = szukane_prostokaty[i];
                        List<Urzadzenie> urzadzenia_w_prostokacie = ctx.Urzadzenia
                        .AsNoTracking()
                        .Where(u => rect.XMin <= u.Dlugosc)
                        .Where(u => rect.YMin <= u.Szerokosc)
                        .Where(u => rect.XMax >= u.Dlugosc)
                        .Where(u => rect.YMax >= u.Szerokosc)
                        .ToList();

                        List<int> id_urzadzen = urzadzenia_w_prostokacie.Select(u => u.UrzadzenieID).ToList();
                        liczby_urzadzen_bd.Add(id_urzadzen.Count);

                        List<Wersja> z_okresu = ctx.Wersje
                            .Where(w => id_urzadzen.Contains(w.UrzadzenieID))
                            .Where(p => p.dataOstatniejModyfikacji >= poczatek)
                            .Where(p => p.dataWygasniecia < koniec)
                            .GroupBy(w => w.UrzadzenieID) //ale tylko najnowsza wersja urządzenia spełniająca warunek
                            .Select(g => g.OrderByDescending(w => w.WersjaID).FirstOrDefault())
                            .ToList();

                        decimal suma = 0;
                        decimal liczba_pomiarow = 0;
                        decimal srednia = 0;

                        foreach (var wersja in z_okresu)
                        {
                            foreach (var pomiar in wersja.Pomiary)
                            {
                                if (pomiar.dtpomiaru >= poczatek && pomiar.dtpomiaru < koniec)
                                {
                                    suma += wersja.Pomiary.Sum(p => p.Wartosc);
                                    liczba_pomiarow += wersja.Pomiary.Count;
                                }
                            }
                        }

                        if (liczba_pomiarow != 0)
                            srednia = suma / liczba_pomiarow;


                        _rmvb.zwrocLiczbeUrzadzenPomiarowSrednia(szukane_prostokaty[i], (DateTime)poczatek, (DateTime)koniec);
                    }

                    Console.WriteLine("Szukanie sredniej z pomiarow z urzadzen znajdujacych sie na obszarze " + "xMin(" + szukane_prostokaty[i].XMin + "), " + "yMin(" + szukane_prostokaty[i].YMin + "), " +
                    "xMax(" + szukane_prostokaty[i].XMax + "), " + "yMax(" + szukane_prostokaty[i].YMax + "), z wersji aktualnych w czasie od " + (DateTime)poczatek + " do " + (DateTime)koniec);
                    Console.WriteLine("Wynik: " + wyniki_bd[i] + "\n");
                }

            }
            
            sukces = !blad;
            Close();
        }
    }
}
