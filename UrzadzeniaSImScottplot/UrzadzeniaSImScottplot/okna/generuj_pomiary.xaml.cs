using System.ComponentModel;
using System.Windows;
using UrzadzeniaSImScottplot.narzedzia;

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy generuj_pomiary.xaml
    /// </summary>
    public partial class generuj_pomiary : Window, INotifyPropertyChanged
    {
        public static event Action aktualizujTabele;

        Generatory _gen;
        public  List<(int,Pomiar)> wygenerowane = new List<(int,Pomiar)>();
        public bool sukces = false;

        private int? _maxId = null;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int? maxId
        {
            get { return _maxId; }
            set
            {
                _maxId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("maxId"));
            }

        }
       
        private void _inicjujKontrolki()
        {
            using (var ctx = new Kontekst())
            {
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);

                if (maxId == null)
                    return;

                _aktualizujStatus(0, true);


                if (!ctx.Wersje
                        .Where(u => u.UrzadzenieID == IdUrzadzenia.Value)
                        .OrderByDescending(w => w.WersjaID)
                        .First()
                        .Aktywne)
                {
                    Przeslij.IsEnabled = false;
                    //wyswietl komunikat
                    komunikat.Text = "Przed generowaniem pomiarów upewnij się, że urządzenie jest \naktywne.";
                }
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

        public generuj_pomiary(Generatory generator)
        {
            InitializeComponent();
            DataContext = this;

            _inicjujKontrolki();
            _gen = generator;
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;

            Close();
        }

        private void Przeslij_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            int id = (int)IdUrzadzenia.Value;

            if ((bool)PodajWartosc.IsChecked)
            {
                Pomiar nowy = new Pomiar((decimal)Wartosc.Value, DateTime.Now);
                wygenerowane.Add((id, nowy));   
            }
            else {
                for (int i = 0; i < Liczba.Value; i++) {        
                    wygenerowane.Add((id,_gen.generujLosowyPomiar()));
                }
            }

            Close();

        }

        private void _aktualizujStatus(int id, bool wartosc) {
            using (var ctx = new Kontekst())
            {
                if (ctx.Wersje
                .Where(u => u.UrzadzenieID == IdUrzadzenia.Value)
                .OrderByDescending(w => w.WersjaID)
                .First()
                .Aktywne)
                {
                    ctx.Urzadzenia.Where(u => u.UrzadzenieID == id).First().Generujemy = wartosc;
                    ctx.SaveChanges();
                    aktualizujTabele.Invoke();
                }
            }
        }

        int poprzednie_id = 0;
        //to jest ID
        private void ZmienionoWartoscPola(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _aktualizujStatus(poprzednie_id, false);
            Waliduj();
            _aktualizujStatus((int)IdUrzadzenia.Value, true);
            poprzednie_id = (int)IdUrzadzenia.Value;
        }

        private void PodajWartosc_Checked(object sender, RoutedEventArgs e)
        {
            Waliduj();
        }

        private void PodajLiczbe_Checked(object sender, RoutedEventArgs e)
        {
            Waliduj();
        }

        private void Waliduj()
        {
            if (!IsLoaded)
                return;

            using (var ctx = new Kontekst()) {
                
                if (ctx.Wersje
                        .Where(u => u.UrzadzenieID == IdUrzadzenia.Value)
                        .OrderByDescending(w => w.WersjaID)
                        .First()
                        .Aktywne)
                {

                    bool mamy_id = (IdUrzadzenia.Value != null);
                    bool mamy_wartosc = (bool)(PodajWartosc.IsChecked == true) && Wartosc.Value != null;
                    bool mamy_liczbe = (bool)(PodajLiczbe.IsChecked == true) && Liczba.Value != null;

                    if (!mamy_id || (!mamy_wartosc && !mamy_liczbe))
                    {
                        Przeslij.IsEnabled = false;
                    }
                    else
                    {
                        Przeslij.IsEnabled = true;
                    }
                }
                else {
                    Przeslij.IsEnabled = false;
                    //wyswietl komunikat
                    komunikat.Text = "Przed generowaniem pomiarów upewnij się, że urządzenie jest \naktywne.";
                }

            }
        
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _aktualizujStatus((int)IdUrzadzenia.Value, false);
        }
    }
}
