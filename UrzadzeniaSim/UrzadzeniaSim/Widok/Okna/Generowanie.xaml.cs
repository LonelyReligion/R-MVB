using System.ComponentModel;
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

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () => 
                {
                    while (!token.IsCancellationRequested)
                    {
                        //PasekPostepu.IsIndeterminate = True;
                        await Task.Delay(1000);
                    }
                }
            );
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
