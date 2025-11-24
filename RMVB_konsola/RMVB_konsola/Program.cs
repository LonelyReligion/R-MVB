using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;

//jak zasymulować szybszy upływ czasu?
Kontekst ctx = new Kontekst();
Wersja.ctx = ctx;
InDBStorage.ctx = ctx;
Repo.ctx = ctx;

RMVB rmvb = new RMVB(ctx);

Urzadzenie.ctx = ctx;
Korzen.ctx = ctx;

// Urzadzenie 0v0
Urzadzenie testowe = new Urzadzenie(0, rmvb.zwrocRepo());
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



for (int i = 0; i < 8; i++)
{
    //do zdebugowania
    int id = i % 7;
    if (!rmvb.czyUrzadzenieIstnieje(id)) { 
        Urzadzenie testowe1 = new Urzadzenie(id, rmvb.zwrocRepo());
        rmvb.dodajUrzadzenie(testowe1);
    }
    Wersja tmp = new Wersja(id, rmvb.zwrocRepo());

    rmvb.dodajWersje(tmp);
}

rmvb.wypiszMVB();


Test jednostka_testujaca = new Test(rmvb.zwrocRepo(), ctx, rmvb);
jednostka_testujaca.wykonajTesty(10);

ctx.Dispose();