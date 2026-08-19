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
using UrzadzeniaSImScottplot.narzedzia;
using UrzadzeniaSImScottplot.repo;

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy DodajUrzadzenia.xaml
    /// </summary>
    public partial class DodajUrzadzenia : Window, INotifyPropertyChanged
    {
        public bool sukces = false;
        public List<Urzadzenie> wygenerowane = new List<Urzadzenie>();
        private Generatory _generator;
        private Repo _repo;
        public DodajUrzadzenia(Repo repo)
        {
            DataContext = this;
            InitializeComponent();
            _repo = repo;
            _generator = new Generatory(_repo);
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            sukces = false;
            Close();
        }

        private int _liczba_urzadzen = 1;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int liczba_urzadzen { 
            get => _liczba_urzadzen;
            set { 
                _liczba_urzadzen = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("liczba_urzadzen"));
            }
        }

        //losowe
        private void Przeslij0_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < liczba_urzadzen; i++) {
                (decimal dlugosc, decimal szerokosc) = _generator.generujWspolrzedne();
                Urzadzenie u = new Urzadzenie((dlugosc, szerokosc));
                wygenerowane.Add(u);
            }

            sukces = true;
            Close();
        }

        private decimal _dlugosc = 14.07m;
        public decimal dlugosc { 
            get => _dlugosc;
            set {
                _dlugosc = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("dlugosc"));
            }
        }
        private decimal _szerokosc = 49m;
        public decimal szerokosc { 
            get => _szerokosc;
            set { 
                _szerokosc=value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("szerokosc"));
            }
        } 
        //konkretny
        private void Przeslij1_Click(object sender, RoutedEventArgs e)
        {
            //Urzadzenie nowe = new Urzadzenie();

            sukces = true;
            Close();
        }
    }
}
