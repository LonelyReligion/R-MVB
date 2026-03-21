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
    /// Logika interakcji dla klasy PasekNarzedzi.xaml
    /// </summary>
    public partial class PasekNarzedzi : UserControl
    {
        public PasekNarzedzi()
        {
            InitializeComponent();
            Powiekszenie.ItemsSource = new List<string> { "100%", "150%", "200%" };
            Powiekszenie.SelectedIndex = 0;
        }

    }
}
