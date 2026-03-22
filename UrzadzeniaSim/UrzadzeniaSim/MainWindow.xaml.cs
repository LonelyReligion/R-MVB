using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UrzadzeniaSim
{
    public partial class MainWindow : Window
    {
       
        double wysokosc_okna;
        double szerokosc_okna;

        bool klik = false;
        public MainWindow()
        {
            InitializeComponent();

            okno.SizeChanged += (s, e) =>
            {
                wysokosc_okna = okno.ActualHeight;
                wysokosc_okna = okno.ActualWidth;
            };

        }

    }
    
}