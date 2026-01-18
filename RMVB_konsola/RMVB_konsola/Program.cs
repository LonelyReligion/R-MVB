using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;

//Setup
Generatory.liczba_urzadzen = 100;

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

Generatory generator = new Generatory(rmvb.zwrocRepo());
Test.generator = generator;

// Urzadzenie 0v0
Urzadzenie testowe = new Urzadzenie(0, generator.generujWspolrzedneDeterministycznie()); // new Urzadzenie(0, generator.generujWspolrzedne());
rmvb.dodajUrzadzenie(testowe);

Pomiar testowy = new Pomiar(0, DateTime.Now);

Wersja alfa = new Wersja(rmvb.zwrocRepo());
alfa.UrzadzenieID = testowe.UrzadzenieID; //czy mozna uzyc new Wersja(id, (Repo)repo);?

alfa.dodajPomiar(testowy);
rmvb.dodajWersje(alfa);
rmvb.dodajPomiar(testowe.UrzadzenieID, testowy, alfa);
///

// Urzadzenie 0v1
Wersja beta = new Wersja(alfa, rmvb.zwrocRepo()); //to deazktywuje alfe
beta.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie

rmvb.usunWersje(beta);
//
List<Wersja> losowe = new List<Wersja>(); //do debuggowania, potrzebne nam do odtworzenia scenariusza

for (int i = 0; i < 100; i++)
{
    int id = i % 100;
    if (!rmvb.czyUrzadzenieIstnieje(id)) { 
        Urzadzenie testowe1 =  new Urzadzenie(id, generator.generujWspolrzedne());
        rmvb.dodajUrzadzenie(testowe1);
    }
    Wersja tmp = new Wersja(id, rmvb.zwrocRepo());

    rmvb.dodajWersje(tmp);

    for (int j = 0; j < 20; j++)
    {
        Decimal losowaTemp = Math.Truncate((Decimal)(rnd.NextDouble() * (41.0 - (-41.0)) - 41.0) * 100) / 100;
        Pomiar losowy = new Pomiar(losowaTemp, DateTime.Now);

        int id_losowe = rnd.Next(rmvb.zwrocRepo().pobierzUrzadzenia().Count - 1);
        Wersja losowa = new Wersja(id_losowe, rmvb.zwrocRepo());
        rmvb.dodajWersje(losowa);
        /*losowa.dodajPomiar(losowy);*/
        rmvb.dodajPomiar(losowa.UrzadzenieID, losowy, losowa);
        losowe.Add(losowa);
    }

    Console.WriteLine(id + ": " + rmvb.zwrocMVB().zwrocLiczbeWpisowKorzenia(0));
    //rmvb.wypiszMVB();
}


/*for (int i = 0; i < rmvb.zwrocRepo().pobierzUrzadzenia().Count(); i++)
{
    Decimal losowaTemp = Math.Truncate((Decimal)(rnd.NextDouble() * (41.0 - (-41.0)) - 41.0)) / 100;
    Pomiar losowy = new Pomiar(losowaTemp, DateTime.Now);

    Wersja losowa = new Wersja(i, rmvb.zwrocRepo());
    losowa.dodajPomiar(losowy);
    rmvb.dodajWersje(losowa);
    rmvb.dodajPomiar(losowa.UrzadzenieID, losowy, losowa);
}*/
rmvb.obliczAgregaty();

rmvb.wypiszMVB();

Test jednostka_testujaca = Test.pobierzInstancje();
if (jednostka_testujaca.wykonajTesty(10)) {
    Console.WriteLine("W czasie wykonywania testów wystąpiły błędy. Szczegóły wyżej.");
    Console.WriteLine("Scenariusz testowy zakładał dodanie urządzeń o podanych id w poniższej kolejności:");

    String wynikowa = "[";
    foreach (Wersja w in losowe) {
        wynikowa += w.UrzadzenieID + ",";
    };
    wynikowa.Substring(0,wynikowa.Length-1);
    wynikowa += "]";
    Console.WriteLine(wynikowa);
}

ctx.Dispose();