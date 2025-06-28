using RMVB_konsola; //aby nie przejmować się folderami

Console.WriteLine("Hello, World!");

Urzadzenie testowe = new Urzadzenie();
testowe.Aktywne = true;
testowe.UrzadzenieID = 0;

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
testowy.UrzadzeniePomiarowe = testowe;

using (var ctx = new Kontekst()) {
    ctx.Urzadzenia.Add(testowe);
    ctx.Pomiary.Add(testowy);
    ctx.SaveChanges();

    //jak zrobic zeby poprzednie wersje byly oznaczone jako nieaktywne automatycznie?
    ctx.Urzadzenia.Add(testowe);
    ctx.SaveChanges();
}