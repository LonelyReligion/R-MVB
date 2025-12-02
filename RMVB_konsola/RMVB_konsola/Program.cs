using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;

//jak zasymulować szybszy upływ czasu?
Generatory.liczba_urzadzen = 100;

Random rnd = new Random();
Kontekst ctx = new Kontekst();
Wersja.ctx = ctx;
InDBStorage.ctx = ctx;
Repo.ctx = ctx;

RMVB rmvb = new RMVB(ctx);
Generatory generator = new Generatory(rmvb.zwrocRepo());

Urzadzenie.ctx = ctx;
Korzen.ctx = ctx;

// Urzadzenie 0v0
Urzadzenie testowe = new Urzadzenie(0, generator.generujWspolrzedne(), rmvb.zwrocRepo());
rmvb.dodajUrzadzenie(testowe);

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;

Wersja alfa = new Wersja(rmvb.zwrocRepo());
alfa.UrzadzenieID = testowe.UrzadzenieID; //czy mozna uzyc new Wersja(id, (Repo)repo);?
rmvb.dodajWersje(alfa);

alfa.dodajPomiar(testowy);
rmvb.dodajPomiar(testowe.UrzadzenieID, testowy);
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
        Urzadzenie testowe1 = new Urzadzenie(id, generator.generujWspolrzedne(), rmvb.zwrocRepo());
        rmvb.dodajUrzadzenie(testowe1);
    }
    Wersja tmp = new Wersja(id, rmvb.zwrocRepo());

    rmvb.dodajWersje(tmp);
}

//czemu jak to wkleje do petli wyzej to drzewo jest zdegenerowane i posiada tylko wersje urzadzenia o id = 0?
for (int j = 0; j < 12; j++)
{
    Pomiar losowy = generator.generujLosowyPomiar();

    int id_losowe = rnd.Next(rmvb.zwrocRepo().pobierzUrzadzenia().Count - 1);
    Wersja losowa = new Wersja(id_losowe, rmvb.zwrocRepo());
    losowa.UrzadzenieID = id_losowe; //nadmiarowe
    rmvb.dodajWersje(losowa);

    losowa.dodajPomiar(losowy);
    rmvb.dodajPomiar(losowa.UrzadzenieID, losowy);
}

rmvb.wypiszMVB();


Test jednostka_testujaca = new Test(rmvb.zwrocRepo(), ctx, rmvb, generator);
jednostka_testujaca.wykonajTesty(10);

ctx.Dispose();