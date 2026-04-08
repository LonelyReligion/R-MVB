using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UrzadzeniaSim.Model;
using UrzadzeniaSim.Narzędzia;

namespace UrzadzeniaSim.Widok.Kontrolki
{
    /// <summary>
    /// Logika interakcji dla klasy PanelBoczny.xaml
    /// </summary>
    public partial class PanelBoczny : UserControl
    {
        public event Action DodajUrzadzenie;
        public PanelBoczny()
        {
            InitializeComponent();
        }

        public void ZlecGenerowanie(object sender, RoutedEventArgs e) {
            DodajUrzadzenie?.Invoke();
        }
    }
}
