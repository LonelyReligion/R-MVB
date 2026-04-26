//xml
using System.Xml;
//

using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;
using System.Configuration;
using System.Globalization;

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
}
catch {
    Console.WriteLine("Podana liczba urządzeń nie jest liczbą całkowitą.");
    Console.WriteLine("Podaj poprawną liczbę urządzeń i spróbuj ponownie.");
    return 0;
}

string granicaPrzezywalnosciStr = ConfigurationManager.AppSettings.Get("granica_przezywalnosci");
double granicaPrzezywalnosci = 0;
CultureInfo kultura = CultureInfo.CreateSpecificCulture("pl-PL");
try
{
    granicaPrzezywalnosci = Double.Parse(granicaPrzezywalnosciStr, kultura);
}
catch
{
    Console.WriteLine("Podana granica przeżywalności urządzeń nie jest poprawna.");
    Console.WriteLine("Czy użyłeś/aś kropki (.) zamiast przecinka (,)?");
    Console.WriteLine("Podaj poprawną granicę przeżywalności i spróbuj ponownie.");
    return 0;
}

string minimalnaLiczbaUrzadzenWKorzeniu = ConfigurationManager.AppSettings.Get("min_urzadzen_korzen");
try
{
    int minimalnaLiczbaUrzadzenWKorzeniu_int = int.Parse(minimalnaLiczbaUrzadzenWKorzeniu);
    Korzen.min_urzadzen_korzen = minimalnaLiczbaUrzadzenWKorzeniu_int;
}
catch {
    Console.WriteLine("Minimalna liczba urządzeń w korzeniu nie jest liczbą całkowitą.");
    Console.WriteLine("Podaj poprawną liczbę urządzeń w korzeniu i spróbuj ponownie.");
    return 0;
}

//
Generatory.liczba_urzadzen = liczbaUrzadzen;
Korzen.granica_przezywalnosci = (decimal)granicaPrzezywalnosci;
//

Random rnd = new Random();
Kontekst ctx = new Kontekst();
Wersja.ctx = ctx;
InDBStorage.ctx = ctx;
Repo.ctx = ctx;
Test.ctx = ctx;
Urzadzenie.ctx = ctx;
Korzen.ctx = ctx;

RMVB rmvb = new RMVB(ctx);
Test.repo = rmvb.zwrocRepo();
Test.rmvb = rmvb;
Urzadzenie.repo = rmvb.zwrocRepo();


Generatory generator = new Generatory(rmvb.zwrocRepo());
Test.generator = generator;


Console.WriteLine("Uwaga, wszystkie pliki znajdujące się w folderze " + sciezkaFolderuWyjsciowego + " zostaną trwale usunięte.");
string[] sciezkiPlikow = Directory.GetFiles(sciezkaFolderuWyjsciowego);
foreach (var plik in sciezkiPlikow) 
{
    File.Delete(plik);
}
//

// Urzadzenie 0v0
ctx.Urzadzenia.FirstOrDefault();

Urzadzenie testowe = new Urzadzenie(generator.zwrocNoweWspolrzedneDeterministyczne());
rmvb.dodajUrzadzenie(testowe);


Pomiar testowy = new Pomiar(0, DateTime.Now);

Wersja alfa = new Wersja(rmvb.zwrocRepo());
alfa.UrzadzenieID = testowe.UrzadzenieID; //czy mozna uzyc new Wersja(id, (Repo)repo);?

//alfa.dodajPomiar(testowy);
rmvb.dodajWersje(alfa);
rmvb.dodajPomiar(testowe.UrzadzenieID, testowy, alfa);
///

// Urzadzenie 0v1
Wersja beta = new Wersja(alfa, rmvb.zwrocRepo()); //to deazktywuje alfe
beta.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie

rmvb.usunWersje(beta);
//
List<Wersja> losowe = new List<Wersja>(); //do debuggowania, potrzebne nam do odtworzenia scenariusza
for (int i = 0; i < liczbaUrzadzen; i++)
{
    int id = i % liczbaUrzadzen;
    if (!rmvb.czyUrzadzenieIstnieje(id))
    {
        Urzadzenie testowe1 = new Urzadzenie(generator.zwrocNoweWspolrzedneDeterministyczne());//new Urzadzenie(id, generator.generujWspolrzedne());
        rmvb.dodajUrzadzenie(testowe1);
    }
    Wersja tmp = new Wersja(id, rmvb.zwrocRepo());

    rmvb.dodajWersje(tmp);

    for (int j = 0; j < liczbaUrzadzen * 2 / 10; j++)
    {
        Decimal losowaTemp = Math.Truncate((Decimal)(rnd.NextDouble() * (41.0 - (-41.0)) - 41.0) * 100) / 100;
        Pomiar losowy = new Pomiar(losowaTemp, DateTime.Now);

        int id_losowe = rnd.Next(rmvb.zwrocRepo().pobierzUrzadzenia().Count - 1);
        Wersja losowa = new Wersja(id_losowe, rmvb.zwrocRepo());

        rmvb.dodajWersje(losowa);
        rmvb.dodajPomiar(losowa.UrzadzenieID, losowy, losowa);
        losowe.Add(losowa);
    }
}


for (int i = 0; i < rmvb.zwrocRepo().pobierzUrzadzenia().Count(); i++)
{
    Decimal losowaTemp = Math.Truncate((Decimal)(rnd.NextDouble() * (41.0 - (-41.0)) - 41.0)) / 100;
    Pomiar losowy = new Pomiar(losowaTemp, DateTime.Now);

    Wersja losowa = new Wersja(i, rmvb.zwrocRepo());

    rmvb.dodajWersje(losowa);
    rmvb.dodajPomiar(losowa.UrzadzenieID, losowy, losowa);
    losowe.Add(losowa);
}

rmvb.obliczAgregaty();

rmvb.wypiszMVB();

Test jednostka_testujaca = Test.pobierzInstancje();
if (jednostka_testujaca.wykonajTesty(100))
{
    Console.WriteLine("W czasie wykonywania testów wystąpiły błędy. Szczegóły wyżej."); //dopisać oraz w pliku (ścieżka i nazwa z pliku konfiguracyjnego)
    Console.WriteLine("Scenariusz testowy zakładał dodanie urządzeń o podanych id w poniższej kolejności:");

    String wynikowa = "[";
    foreach (Wersja w in losowe)
    {
        wynikowa += w.UrzadzenieID + ",";
    };
    wynikowa.Substring(0, wynikowa.Length - 1);
    wynikowa += "]";
    Console.WriteLine(wynikowa);

    jednostka_testujaca.zapiszBledy(sciezkaFolderuWyjsciowego);
}
else
{
    jednostka_testujaca.zapiszWyniki(sciezkaFolderuWyjsciowego); //osobne logowanie błędów do innego pliku wyżej powinno nastąpić
}

rmvb.Reset();
rmvb.zapiszMVB(sciezkaFolderuWyjsciowego);

ctx.Dispose();
return 0;