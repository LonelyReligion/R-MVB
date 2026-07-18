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

        public wyszukiwanie_wersji()
        {
            DataContext = this;
            InitializeComponent();
            _inicjujKontrolki();
        }

        private void id_urzadzenia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}
