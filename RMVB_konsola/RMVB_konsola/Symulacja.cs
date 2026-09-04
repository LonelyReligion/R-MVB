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
        RMVB _rmvb;
        Generatory _generator;

        Random rnd = new Random();

        public Symulacja(int liczba_urzadzen, RMVB rmvb, Generatory generator) {
            _liczba_urzadzen = liczba_urzadzen;
            _generator = generator;
            _rmvb = rmvb;
        }


        public void Symuluj() {
            for (int i = 0; i < _liczba_urzadzen; i++) 
            {
                Urzadzenie urzadzenie = new Urzadzenie(_generator.generujWspolrzedne());
                _rmvb.dodajUrzadzenie(urzadzenie);

                int liczba_pomiarow = rnd.Next(1,10);
                for (int j = 0; j < liczba_pomiarow; j++) 
                {
                    Pomiar pomiar = _generator.generujLosowyPomiar();
                    Wersja wersja = new Wersja(urzadzenie.UrzadzenieID, _rmvb.zwrocRepo(), _rmvb, pomiar.dtpomiaru);

                    _rmvb.dodajWersje(wersja);
                    _rmvb.dodajPomiar(urzadzenie.UrzadzenieID, pomiar, wersja);
                }
            }

        }

    }
}
