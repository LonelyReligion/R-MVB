using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami

//jak zasymulować szybszy upływ czasu?

Console.WriteLine("Hello, World!");

Repo repo = new Repo();
Drzewo mvb = new Drzewo(repo);


Urzadzenie testowe = new Urzadzenie(0, repo);

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
testowe.dodajPomiar(testowy);

using (var ctx = new Kontekst()) {
    ctx.Urzadzenia.Add(testowe);
    ctx.Pomiary.Add(testowy);
    ctx.SaveChanges();
    mvb.dodajUrzadzenie(testowe);
    repo.dodajUrzadzenie(testowe);

    Urzadzenie testowe2 = new Urzadzenie(testowe, repo);
    testowe.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie
    ctx.Urzadzenia.Add(testowe2);
    ctx.SaveChanges();
    repo.dodajUrzadzenie(testowe2);
    mvb.dodajUrzadzenie(testowe2);

    //testowe2.dezaktywuj();
    //mvb.usunUrzadzenie(testowe2); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow
}


using (var ctx = new Kontekst())
{
    for (int i = 0; i < 5; i++)
    {
        int id = i % 3;
        Urzadzenie testowe1 = new Urzadzenie(id, repo);
        ctx.Urzadzenia.Add(testowe1);
        ctx.SaveChanges(); //wyjac z petli?

        repo.dodajUrzadzenie(testowe1);
        mvb.dodajUrzadzenie(testowe1);
    }

    //wyszukiwanie ostatniej wersji po id
    var szukane = ctx.Urzadzenia
        .Where(u => u.UrzadzenieID == 2)
        .OrderByDescending(u => u.Wersja)
        .FirstOrDefault();
    Console.WriteLine(szukane.UrzadzenieID + "v" + szukane.Wersja);
    szukane = mvb.szukaj(2);
    Console.WriteLine(szukane.UrzadzenieID + "v" + szukane.Wersja);

    //wyszukiwanie po id i wersji
    szukane = ctx.Urzadzenia.Find(2, 0);
    Console.WriteLine(szukane.UrzadzenieID + "v" + szukane.Wersja);
    szukane = mvb.szukaj(2, 0);
    Console.WriteLine(szukane.UrzadzenieID + "v" + szukane.Wersja);

}

mvb.wypiszDrzewo();