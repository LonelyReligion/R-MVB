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
    /// Logika interakcji dla klasy wyszukaj_srednia.xaml
    /// </summary>
    public partial class wyszukaj_srednia : Window
    {
        public int? maxId;
        private void _inicjujKontrolki() {
            using (var ctx = new Kontekst()) {
                maxId = ctx.Urzadzenia.Max(u => (int?)u.UrzadzenieID);

                if (maxId == null) {
                    Window dialog = (Window) new brak_urzadzen_w_bazie(this);
                    dialog.ShowDialog();
                    Close();
                }
            }
        }
        public wyszukaj_srednia()
        {
            InitializeComponent();
        }
    }
}
