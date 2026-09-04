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

//Setup
string sciezkaFolderuWyjsciowego;
sciezkaFolderuWyjsciowego = ConfigurationManager.AppSettings.Get("sciezka_folderu_wyjsciowego");


Directory.CreateDirectory(sciezkaFolderuWyjsciowego);

if (!Directory.Exists(sciezkaFolderuWyjsciowego))
{
    Console.WriteLine("Podana ścieżka jest niepoprawna.");
    return 0;
}
Console.WriteLine("Pliki wyjściowe znajdziesz pod adresem: " + Path.GetFullPath(sciezkaFolderuWyjsciowego));


string liczbaUrzadzenStr = ConfigurationManager.AppSettings.Get("liczba_urzadzen");
int liczbaUrzadzen = 0;
try
{
    liczbaUrzadzen = int.Parse(liczbaUrzadzenStr);
    Generatory.liczba_urzadzen = liczbaUrzadzen;
}
catch
{
    Console.WriteLine("Podana liczba urządzeń nie jest liczbą całkowitą.");
    Console.WriteLine("Podaj poprawną liczbę urządzeń id spróbuj ponownie.");
    return 0;
}

string granicaPrzezywalnosciStr = ConfigurationManager.AppSettings.Get("granica_przezywalnosci");
double granicaPrzezywalnosci = 0;
CultureInfo kultura = CultureInfo.CreateSpecificCulture("pl-PL");
try
{
    granicaPrzezywalnosci = Double.Parse(granicaPrzezywalnosciStr, kultura);
    Korzen.granica_przezywalnosci = (decimal)granicaPrzezywalnosci;
}
catch
{
    Console.WriteLine("Podana granica przeżywalności urządzeń nie jest poprawna.");
    Console.WriteLine("Czy użyłeś/aś kropki (.) zamiast przecinka (,)?");
    Console.WriteLine("Podaj poprawną granicę przeżywalności id spróbuj ponownie.");
    return 0;
}

string minimalnaLiczbaUrzadzenWKorzeniu = ConfigurationManager.AppSettings.Get("min_urzadzen_korzen");
try
{
    int minimalnaLiczbaUrzadzenWKorzeniu_int = int.Parse(minimalnaLiczbaUrzadzenWKorzeniu);
    Korzen.min_urzadzen_korzen = minimalnaLiczbaUrzadzenWKorzeniu_int;
}
catch
{
    Console.WriteLine("Minimalna liczba urządzeń w korzeniu nie jest liczbą całkowitą.");
    Console.WriteLine("Podaj poprawną liczbę urządzeń w korzeniu id spróbuj ponownie.");
    return 0;
}

using (var ctx = new Kontekst())
{
    ctx.Urzadzenia.FirstOrDefault();
}

//
RMVB rmvb = new RMVB();
Generatory generator = new Generatory(rmvb.zwrocRepo());

Test.repo = rmvb.zwrocRepo();
Test.rmvb = rmvb;
Test.generator = generator;

Urzadzenie.repo = rmvb.zwrocRepo();
Symulacja sym = new Symulacja(liczbaUrzadzen,rmvb,generator);
//

Console.WriteLine("Uwaga, wszystkie pliki znajdujące się w folderze " + sciezkaFolderuWyjsciowego + " zostaną trwale usunięte.");
string[] sciezkiPlikow = Directory.GetFiles(sciezkaFolderuWyjsciowego);
foreach (var plik in sciezkiPlikow)
{
    File.Delete(plik);
}
//

sym.Symuluj();


rmvb.wypiszMVB();

Test jednostka_testujaca = Test.pobierzInstancje();
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

rmvb.Reset();
rmvb.zapiszMVB(sciezkaFolderuWyjsciowego);

return 0;