using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;
using UrzadzeniaSim.Model.RMVB;
using UrzadzeniaSim.Model.RMVB.MVB;

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

            granicaPrzezywalnosci.Value = Korzen.s_GranicaPrzezywalnosci;
            minLiczbaUrzadzen.Value = Korzen.s_MinUrzadzenKorzen;
            wybranaSciezka.Text = DrzewoRMVB.s_SciezkaFolderuWyjsciowego;
        }

        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {
            Korzen.s_GranicaPrzezywalnosci = (decimal)granicaPrzezywalnosci.Value;
            Korzen.s_MinUrzadzenKorzen = (int)minLiczbaUrzadzen.Value;
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
                DrzewoRMVB.s_SciezkaFolderuWyjsciowego = folder;
                wybranaSciezka.Text = DrzewoRMVB.s_SciezkaFolderuWyjsciowego;
            }
        }
    }
}
