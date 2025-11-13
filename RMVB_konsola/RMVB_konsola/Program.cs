using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;

//jak zasymulować szybszy upływ czasu?
Kontekst ctx = new Kontekst();
Wersja.ctx = ctx;
InDBStorage.ctx = ctx;

TreeRepository repo = new Repo();
DrzewoMVB mvb = new DrzewoMVB(repo, ctx);

// WIP
RTree rtree = new RTree((Repo)repo, ctx);
//

Urzadzenie.ctx = ctx;
Korzen.ctx = ctx;

Urzadzenie testowe = new Urzadzenie(0, (Repo)repo);

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
Wersja alfa = new Wersja((Repo)repo);
alfa.UrzadzenieID = testowe.UrzadzenieID;
alfa.dodajPomiar(testowy);


ctx.Urzadzenia.Add(testowe);
ctx.Wersje.Add(alfa);
ctx.Pomiary.Add(testowy);

mvb.dodajUrzadzenie(alfa);
repo.saveDevice(testowe);
repo.saveVersion(alfa);

/*Urzadzenie testowe2;

testowe2 = new Urzadzenie(testowe, (Repo)repo); //konstruktor kopiujący
testowe.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie
ctx.Urzadzenia.Add(testowe2);

mvb.dodajUrzadzenie(testowe2);
repo.saveDevice(testowe2);

//testowe2.dezaktywuj();
//mvb.usunUrzadzenie(testowe2); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow

for (int i = 0; i < 8; i++)
{
    int id = i % 7;
    Urzadzenie testowe1 = new Urzadzenie(id, (Repo)repo);
    ctx.Urzadzenia.Add(testowe1); 

    mvb.dodajUrzadzenie(testowe1);
    repo.saveDevice(testowe1);
}

mvb.wypiszDrzewo();

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