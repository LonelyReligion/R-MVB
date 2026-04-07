using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy Urządzenie.xaml
    /// </summary>
    public partial class Urządzenie : UserControl
    {
        public enum STATUS { 
            NIEAKTYWNY,//wyszarzony?
            AKTYWNY,
            AKTYWNY_NADAJE
        };

        public decimal dlugosc;
        public decimal szerokosc;
        public Urządzenie(decimal dlugosc, decimal szerokosc)
        {
            InitializeComponent();
            this.dlugosc = dlugosc;
            this.szerokosc = szerokosc;
        }
    }
}
