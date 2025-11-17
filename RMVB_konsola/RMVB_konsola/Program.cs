using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;

//jak zasymulować szybszy upływ czasu?
Kontekst ctx = new Kontekst();
Wersja.ctx = ctx;
InDBStorage.ctx = ctx;
Repo.ctx = ctx;

TreeRepository repo = new Repo();
DrzewoMVB mvb = new DrzewoMVB(repo, ctx);

// WIP
RTree rtree = new RTree((Repo)repo, ctx);
//

Urzadzenie.ctx = ctx;
Korzen.ctx = ctx;

// Urzadzenie 0v0
Urzadzenie testowe = new Urzadzenie(0, (Repo)repo);
Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
Wersja alfa = new Wersja((Repo)repo);
alfa.UrzadzenieID = testowe.UrzadzenieID; //czy mozna uzyc new Wersja(id, (Repo)repo);?
alfa.dodajPomiar(testowy);
mvb.dodajUrzadzenie(alfa);
repo.saveDevice(testowe);
ctx.Pomiary.Add(testowy);
repo.saveVersion(alfa);
///

// Urzadzenie 0v1
Wersja beta = new Wersja(alfa, (Repo)repo); //to deazktywuje alfe
beta.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie
mvb.dodajUrzadzenie(beta); //musi zostac zapisana najpierw
mvb.usunUrzadzenie(beta); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow
repo.saveVersion(beta);
//



for (int i = 0; i < 8; i++)
{
    //do zdebugowania
    int id = i % 7;
    if (!((Repo)repo).czyUrzadzenieIstnieje(id)) { 
        Urzadzenie testowe1 = new Urzadzenie(id, (Repo)repo);
        repo.saveDevice(testowe1);
    }
    Wersja tmp = new Wersja(id, (Repo)repo);

    mvb.dodajUrzadzenie(tmp);

}

mvb.wypiszDrzewo();

/*
Test.ctx = ctx;
Test.repo = (Repo)repo;
Test.mvb = mvb;
Test jednostka_testujaca = new Test();

Console.WriteLine("Wyszukiwanie po dacie i id");
jednostka_testujaca.testDataId(10);

Console.WriteLine("\nWyszukiwanie po id");
jednostka_testujaca.testId(10);

Console.WriteLine("\nWyszukiwanie po id i wersji");
jednostka_testujaca.testIdV(10);

Console.WriteLine("\nWyszukiwanie po dacie i dacie");
jednostka_testujaca.testDataData(10);
*/
ctx.Dispose();