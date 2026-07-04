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

namespace UrzadzeniaSImScottplot
{
    /// <summary>
    /// Logika interakcji dla klasy wyszukaj_srednia.xaml
    /// </summary>
    public partial class wyszukaj_srednia : Window, INotifyPropertyChanged
    {
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
        public wyszukaj_srednia()
        {
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
            this.ContentRendered += _sprawdzCzyMamyUrzadzenia;
        }

        private void Przeslij_Click(object sender, RoutedEventArgs e)
        {
            sukces = true;
            Close();
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }
    }
}
