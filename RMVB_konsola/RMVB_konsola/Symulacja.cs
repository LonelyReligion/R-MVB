using RMVB_konsola.Indeks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMVB_konsola
{
    internal class Symulacja
    {
        int _liczba_urzadzen;
        Pamiec _pamiec;
        Generatory _generator;

        Random rnd = new Random();

        public Symulacja(int liczba_urzadzen, Pamiec pamiec, Generatory generator) {
            _liczba_urzadzen = liczba_urzadzen;
            _generator = generator;
            _pamiec = pamiec;
        }


        public void Symuluj() {
            for (int i = 0; i < _liczba_urzadzen; i++) 
            {
                Urzadzenie urzadzenie = new Urzadzenie(_generator.generujWspolrzedne());
                _pamiec.dodajUrzadzenie(urzadzenie);

                int liczba_pomiarow = rnd.Next(1,10);
                for (int j = 0; j < liczba_pomiarow; j++) 
                {
                    Pomiar pomiar = _generator.generujLosowyPomiar();
                    Wersja wersja = new Wersja(urzadzenie.UrzadzenieID, _pamiec.zwrocRMVB(), pomiar.dtpomiaru);

                    _pamiec.dodajWersje(wersja);
                    _pamiec.dodajPomiar(urzadzenie.UrzadzenieID, pomiar, wersja);
                }
            }

            Console.WriteLine("Zakonczono generowanie danych");
        }

    }
}
