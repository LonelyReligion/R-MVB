using RMVB_konsola;

using RMVB_konsola.MVB; //aby nie przejmować się folderami
using RMVB_konsola.R;

using System.Diagnostics;

//jak zasymulować szybszy upływ czasu?

Repo repo = new Repo();
Drzewo mvb = new Drzewo(repo);

// WIP
TreeRepository repo_R = new InDBStorage();
RTree rtree = new RTree(repo_R);
//

Kontekst ctx = new Kontekst();

Urzadzenie.ctx = ctx;
Korzen.ctx = ctx;

Urzadzenie testowe = new Urzadzenie(0, repo);

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
testowe.dodajPomiar(testowy);

Urzadzenie testowe2;

ctx.Urzadzenia.Add(testowe);
ctx.Pomiary.Add(testowy);
ctx.SaveChanges();
mvb.dodajUrzadzenie(testowe);
repo.dodajUrzadzenie(testowe);

testowe2 = new Urzadzenie(testowe, repo); //konstruktor kopiujący
testowe.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie
ctx.Urzadzenia.Add(testowe2);
ctx.SaveChanges();
repo.dodajUrzadzenie(testowe2);
mvb.dodajUrzadzenie(testowe2);
    
//testowe2.dezaktywuj();
//mvb.usunUrzadzenie(testowe2); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow

for (int i = 0; i < 8; i++)
{
    int id = i % 7;
    Urzadzenie testowe1 = new Urzadzenie(id, repo);
    ctx.Urzadzenia.Add(testowe1);
    ctx.SaveChanges(); 

    repo.dodajUrzadzenie(testowe1);
    mvb.dodajUrzadzenie(testowe1);
}

mvb.wypiszDrzewo();

Test.ctx = ctx;
Test.repo = repo;
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

ctx.Dispose();