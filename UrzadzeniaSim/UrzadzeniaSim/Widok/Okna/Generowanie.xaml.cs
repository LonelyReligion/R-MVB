using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using UrzadzeniaSim.Model;

namespace UrzadzeniaSim.Widok.Okna
{
    /// <summary>
    /// Logika interakcji dla klasy Generowanie.xaml
    /// </summary>
    public partial class Generowanie : Window, INotifyPropertyChanged
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token;

        private Urzadzenie_Model _urzadzenie;
        private int _id;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int id
        {
            get { return _id; }
            set { _id = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("id")); }
        }
        public Generowanie(Urzadzenie_Model urzadzenie)
        {
            DataContext = this;
            id = urzadzenie.UrzadzenieID;
            token = cancellationTokenSource.Token;

            InitializeComponent();
        }
        private bool pracaWtoku = false;
        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            PasekPostepu.IsIndeterminate = true;
            await Task.Yield();//potrzebne żeby UI się zaktualizowało
            
            cancellationTokenSource = new CancellationTokenSource();
            token = cancellationTokenSource.Token;

            pracaWtoku = true;
            Task.Run(async () => 
                {
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1000); //tyle ile w updown
                    }
                    Trace.WriteLine("Zadanie zostało anulowane przez użytkownika lub zakończyło się pomyślnie.");
                }
            );
            Start.IsEnabled = false;
            Stop.IsEnabled = true;


        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel(); //to nie jest zatrzymanie tylko uprzejma prośba

            PasekPostepu.IsIndeterminate = false;

            pracaWtoku = false;

            if (sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
            else 
                Start.IsEnabled = false;
            Stop.IsEnabled = false;

        }

        private void sekundy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (pracaWtoku == false && (sekundy.Value == null || liczbaCykli.Value == null)) {
                Start.IsEnabled = false;
                Stop.IsEnabled = false;
            }

            if(pracaWtoku == false && sekundy.Value != null && liczbaCykli.Value != null)
                Start.IsEnabled = true;
        }

    }
}
