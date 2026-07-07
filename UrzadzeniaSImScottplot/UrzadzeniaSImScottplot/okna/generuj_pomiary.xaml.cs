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
    /// Logika interakcji dla klasy generuj_pomiary.xaml
    /// </summary>
    public partial class generuj_pomiary : Window, INotifyPropertyChanged
    {
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

            if ((bool)PodajWartosc.IsChecked)
            {
                Close();
            }
            else { 
            
            }

            
            
        }

        private void ZmienionoWartoscPola(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Waliduj();
        }

        private void PodajWartosc_Checked(object sender, RoutedEventArgs e)
        {
            Waliduj();
        }

        private void PodajLiczbe_Checked(object sender, RoutedEventArgs e)
        {
            Waliduj();
        }

        private void Waliduj() {
            if (!IsLoaded)
                return;

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


    }
}
