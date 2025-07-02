using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami

//jak zasymulować szybszy upływ czasu?

Console.WriteLine("Hello, World!");

Repo repo = new Repo();
Drzewo mvb = new Drzewo();


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
    ctx.Urzadzenia.Add(testowe2);
    ctx.SaveChanges();
    repo.dodajUrzadzenie(testowe2);

    mvb.dodajUrzadzenie(testowe2);

    ctx.Urzadzenia.Find(0, 0);
}

//przetestowac
Urzadzenie testowe1 = new Urzadzenie(1, repo);

using (var ctx = new Kontekst())
{
    ctx.Urzadzenia.Add(testowe1);
    ctx.SaveChanges();
}

mvb.wypiszDrzewo();