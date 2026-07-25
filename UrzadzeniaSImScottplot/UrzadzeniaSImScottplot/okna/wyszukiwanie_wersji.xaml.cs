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
    /// Logika interakcji dla klasy wyszukiwanie_wersji.xaml
    /// </summary>
    public partial class wyszukiwanie_wersji : Window, INotifyPropertyChanged
    {
        public bool sukces = false;

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


        private void _inicjujKontrolki()
        {
            using (var ctx = new Kontekst())
            {
                //maksymalne id urzadzenia
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);
                Urzadzenie? oMaxId = null;

                if (maxId != null)
                {
                    oMaxId = ctx.Urzadzenia.Where(u => u.UrzadzenieID == maxId).First();
                    //maksymalny numer wersji urzadzenia o maxid
                    maxVer = oMaxId.Wersje.Last().WersjaID;
                }

                int? minId = ctx.Urzadzenia.Min(u => (int?)u.UrzadzenieID);

                if (minId != null) {
                    minVer = oMaxId.Wersje.First().WersjaID;

                    idWersji.Value = minVer;
                }

            }
        }

        public wyszukiwanie_wersji()
        {
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

            using (var ctx = new Kontekst())
            {
                //maksymalne id urzadzenia
                int Id = (int)IdUrzadzenia2.Value;
                Urzadzenie? oId = null;

                if (Id != null)
                {
                    oId = ctx.Urzadzenia.Where(u => u.UrzadzenieID == Id).First();
                    //maksymalny numer wersji urzadzenia o maxid
                    maxVer = oId.Wersje.Last().WersjaID;
                }

                int? minId = ctx.Urzadzenia.Min(u => (int?)u.UrzadzenieID);

                if (minId != null)
                {
                    minVer = oId.Wersje.First().WersjaID;

                    idWersji.Value = minVer;
                }
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

        private void Przeslij1_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            Close();
        }

        private void Przeslij2_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            Close();
        }

        private void Przeslij3_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            Close();
        }
    }
}
