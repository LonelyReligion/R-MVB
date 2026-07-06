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
using System.Windows.Shapes;

namespace UrzadzeniaSImScottplot.okna
{
    /// <summary>
    /// Logika interakcji dla klasy generuj_pomiary.xaml
    /// </summary>
    public partial class generuj_pomiary : Window
    {
        Generatory _gen;
        public  List<(int,Pomiar)> wygenerowane = new List<(int,Pomiar)>();
        public bool sukces = false;
        public generuj_pomiary(Generatory generator)
        {
            InitializeComponent();
        }

    }
}
