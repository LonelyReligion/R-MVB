using RMVB_konsola; //aby nie przejmować się folderami

Console.WriteLine("Hello, World!");

Urzadzenie testowe = new Urzadzenie(0);

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
testowy.UrzadzeniaPomiarowe.Add(testowe);

using (var ctx = new Kontekst()) {
    ctx.Urzadzenia.Add(testowe);
    ctx.Pomiary.Add(testowy);
    ctx.SaveChanges();

    Urzadzenie testowe2 = new Urzadzenie(testowe);
    ctx.Urzadzenia.Add(testowe2);
    ctx.SaveChanges();

    ctx.Urzadzenia.Find(0, 0);
}

Urzadzenie testowe1 = new Urzadzenie(1);

using (var ctx = new Kontekst())
{
    ctx.Urzadzenia.Add(testowe1);
    ctx.SaveChanges();
}