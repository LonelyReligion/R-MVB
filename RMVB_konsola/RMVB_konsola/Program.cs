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
Urzadzenie testowe = new Urzadzenie(0, generator.generujWspolrzedne());
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



for (int i = 0; i < 100; i++)
{
    //do zdebugowania
    int id = i % 100;
    if (!rmvb.czyUrzadzenieIstnieje(id)) { 
        Urzadzenie testowe1 = new Urzadzenie(id, generator.generujWspolrzedne());
        rmvb.dodajUrzadzenie(testowe1);
    }
    Wersja tmp = new Wersja(id, rmvb.zwrocRepo());

    rmvb.dodajWersje(tmp);

    Pomiar losowy = new Pomiar();
    int id_losowe = 0;
    Wersja losowa = new Wersja(0, rmvb.zwrocRepo());
    losowa.UrzadzenieID = id_losowe; //nadmiarowe
    losowa.dodajPomiar(losowy);

    rmvb.dodajWersje(losowa);
    rmvb.dodajPomiar(losowa.UrzadzenieID, losowy, losowa);
}

//czemu jak to wkleje do petli wyzej to drzewo jest zdegenerowane i posiada tylko wersje urzadzenia o id = 0?
/*for (int j = 0; j < 12; j++)
{
    Pomiar losowy = generator.generujLosowyPomiar();

    int id_losowe = rnd.Next(rmvb.zwrocRepo().pobierzUrzadzenia().Count - 1);
    Wersja losowa = new Wersja(id_losowe, rmvb.zwrocRepo());
    losowa.UrzadzenieID = id_losowe; //nadmiarowe
    rmvb.dodajWersje(losowa);

    losowa.dodajPomiar(losowy);
    rmvb.dodajPomiar(losowa.UrzadzenieID, losowy);
}*/

rmvb.wypiszMVB();

Test jednostka_testujaca = Test.pobierzInstancje();
jednostka_testujaca.wykonajTesty(10);

ctx.Dispose();