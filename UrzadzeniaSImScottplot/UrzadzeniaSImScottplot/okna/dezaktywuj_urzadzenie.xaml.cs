using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy dezaktywuj_urzadzenie.xaml
    /// </summary>
    public partial class dezaktywuj_urzadzenie : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private RMVB _rmvb;
        
        private int? _maxId;
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
            Urzadzenie_DF.AutoGeneratingColumn += generowanieKolumn;
            using (var ctx = new Kontekst())
            {
                //maksymalne id urzadzenia
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);
                List<Wersja> wersja = new List<Wersja>(){ctx.Wersje
                        .Where(u => u.UrzadzenieID == (int)IdUrzadzenia.Value)
                        .OrderByDescending(w => w.WersjaID)
                        .ToList()[0]};
                Urzadzenie_DF.ItemsSource = wersja;
            }
        }

        public dezaktywuj_urzadzenie(RMVB rmvb)
        {
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
            _rmvb = rmvb;
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

        public void generowanieKolumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "UrzadzenieID" ||
                e.PropertyName == "Pomiary" || e.PropertyName == "UrzadzenieRodzic")
            {
                e.Column.Visibility = Visibility.Collapsed;
            }

            if (e.PropertyName == "Aktywne")
            {
                e.Column.IsReadOnly = false;
            }
            else {
                e.Column.IsReadOnly = true;
            }

        }

        private void IdUrzadzenia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!IsLoaded)
                return;

            if (IdUrzadzenia.Value == null)
                return;

            using (var ctx = new Kontekst())
            {
                List<Wersja> wersja = new List<Wersja>(){ctx.Wersje
                        .Where(u => u.UrzadzenieID == (int)IdUrzadzenia.Value)
                        .OrderByDescending(w => w.WersjaID)
                        .ToList()[0]};
                Urzadzenie_DF.ItemsSource = wersja;
            }

        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {
            //logika zapisywania
            using (var ctx = new Kontekst()) {
                Wersja w = ctx.Wersje
                        .Where(u => u.UrzadzenieID == (int)IdUrzadzenia.Value)
                        .OrderByDescending(w => w.WersjaID)
                        .First();

                DateTime data_zm = DateTime.Now;
                w.dezaktywuj(data_zm); //tu nie dezaktywuj bo moze byc tez aktywowanie zczytac tick
                _rmvb.szukaj(w.UrzadzenieID, w.WersjaID).dezaktywuj(data_zm);

                ctx.SaveChanges();


                Wersja mvb = _rmvb.szukaj(w.UrzadzenieID, w.WersjaID);
            }

            Close();
        }
    }
}
