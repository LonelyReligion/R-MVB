//xml
using System.Xml;
//

using RMVB_konsola;
using RMVB_konsola.R;

using System.Diagnostics;
using System.Configuration;
using System.Globalization;
using RMVB_konsola.Indeks.MVB;
using RMVB_konsola.Indeks;
using System.Drawing;
using Rectangle = RMVB_konsola.Indeks.R.Rectangle;
using RMVB_konsola.baza;

//Setup
Pamiec pamiec = new Pamiec();
string sciezkaFolderuWyjsciowego="", generujemyStr = "";
int liczbaUrzadzen = 0;
bool generujemy = false;

if (!pamiec.zaladujZmienne(ref sciezkaFolderuWyjsciowego, ref liczbaUrzadzen, ref generujemy))
    return 0;

pamiec.zwrocRepo().przygotujBaze();
//

RMVB rmvb = pamiec.zwrocRMVB();
Generatory generator = new Generatory(rmvb.zwrocRepo());

Test.repo = rmvb.zwrocRepo();
Test.rmvb = rmvb;
Test.generator = generator;

Urzadzenie.repo = rmvb.zwrocRepo();
Symulacja sym = new Symulacja(liczbaUrzadzen,pamiec,generator);
//

Console.WriteLine("Uwaga, wszystkie pliki znajdujące się w folderze " + sciezkaFolderuWyjsciowego + " zostaną trwale usunięte.");
string[] sciezkiPlikow = Directory.GetFiles(sciezkaFolderuWyjsciowego);
foreach (var plik in sciezkiPlikow)
{
    File.Delete(plik);
}
//

if (generujemy)
{
    sym.Symuluj();
}
else 
{
    if (!pamiec.odczytajEncje()) return 0 ;
}

pamiec.wypiszMVB();

Test jednostka_testujaca = Test.pobierzInstancje();
if (rmvb.zwrocRepo().pobierzUrzadzenia().Count == 0)
{
    Console.WriteLine("W bazie nie ma żadnych urządzeń. Testy nie zostaną wykonane.");
}
else
{ 
    if (jednostka_testujaca.wykonajTesty(100))
    {
        Console.WriteLine("W czasie wykonywania testów wystąpiły błędy. Szczegóły wyżej."); //dopisać oraz w pliku (ścieżka i nazwa z pliku konfiguracyjnego)
        Console.WriteLine("Scenariusz testowy zakładał dodanie urządzeń o podanych id w poniższej kolejności:");

        jednostka_testujaca.zapiszBledy(sciezkaFolderuWyjsciowego);
    }
    else
    {
        jednostka_testujaca.zapiszWyniki(sciezkaFolderuWyjsciowego); //osobne logowanie błędów do innego pliku wyżej powinno nastąpić
    }
}

pamiec.zapiszMVB(sciezkaFolderuWyjsciowego);
pamiec.zapiszEncje(sciezkaFolderuWyjsciowego);
pamiec.Reset();

return 0;