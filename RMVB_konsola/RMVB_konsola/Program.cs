using RMVB_konsola; //aby nie przejmować się folderami

Console.WriteLine("Hello, World!");

Urzadzenie testowe = new Urzadzenie();
testowe.Aktywne = true;

Pomiar testowy = new Pomiar();
testowy.Wartosc = 0;
testowy.dtpomiaru = DateTime.Now;
testowy.UrzadzeniePomiarowe = testowe;

using (var ctx = new Kontekst()) {
    ctx.Urzadzenia.Add(testowe);
    ctx.Pomiary.Add(testowy);
    ctx.SaveChanges();
}

Console.WriteLine(testowe.Pomiary.ElementAt(0).PomiarID);