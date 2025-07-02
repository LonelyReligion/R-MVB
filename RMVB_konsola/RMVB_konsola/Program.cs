using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami

//jak zasymulować szybszy upływ czasu?

Console.WriteLine("Hello, World!");
Drzewo mvb = new Drzewo();

Urzadzenie testowe = new Urzadzenie(0);

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
testowe.dodajPomiar(testowy);

using (var ctx = new Kontekst()) {
    ctx.Urzadzenia.Add(testowe);
    ctx.Pomiary.Add(testowy);
    ctx.SaveChanges();
    mvb.dodajUrzadzenie(testowe);

    Urzadzenie testowe2 = new Urzadzenie(testowe);
    ctx.Urzadzenia.Add(testowe2);
    ctx.SaveChanges();
    mvb.dodajUrzadzenie(testowe2);

    ctx.Urzadzenia.Find(0, 0);
}

Urzadzenie testowe1 = new Urzadzenie(1);

using (var ctx = new Kontekst())
{
    ctx.Urzadzenia.Add(testowe1);
    ctx.SaveChanges();
}

mvb.wypiszDrzewo();