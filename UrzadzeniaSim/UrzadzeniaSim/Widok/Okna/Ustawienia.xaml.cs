using System.Windows;
using UrzadzeniaSim.Model.RMVB.MVB;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using UrzadzeniaSim.Model.RMVB;

namespace UrzadzeniaSim.Widok.Okna
{
    /// <summary>
    /// Logika interakcji dla klasy Ustawienia.xaml
    /// </summary>
    public partial class Ustawienia : Window
    {
        public Ustawienia()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;

            granicaPrzezywalnosci.Value = Korzen.granica_przezywalnosci;
            minLiczbaUrzadzen.Value = Korzen.min_urzadzen_korzen;
            wybranaSciezka.Text = DrzewoRMVB.sciezkaFolderuWyjsciowego;
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {
            Korzen.granica_przezywalnosci = (decimal)granicaPrzezywalnosci.Value;
            Korzen.min_urzadzen_korzen = (int)minLiczbaUrzadzen.Value;
            Close();
        }

        private void przegladaj_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Wybierz folder plików wyjściowych";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Directory.GetCurrentDirectory();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                DrzewoRMVB.sciezkaFolderuWyjsciowego = folder;
                wybranaSciezka.Text = DrzewoRMVB.sciezkaFolderuWyjsciowego;
            }
        }
    }
}
