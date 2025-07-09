using RMVB_konsola;
using RMVB_konsola.MVB; //aby nie przejmować się folderami
using System.Diagnostics;

//jak zasymulować szybszy upływ czasu?

Repo repo = new Repo();
Drzewo mvb = new Drzewo(repo);
Kontekst ctx = new Kontekst();
Urzadzenie.ctx = ctx;

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

testowe2 = new Urzadzenie(testowe, repo);
testowe.usunPomiar(testowy); // sytuacja usuwamy pomiar w nowej wersji urzadzenia, ale zachowujemy go w bazie
ctx.Urzadzenia.Add(testowe2);
ctx.SaveChanges();
repo.dodajUrzadzenie(testowe2);
mvb.dodajUrzadzenie(testowe2);
    
//testowe2.dezaktywuj();
//mvb.usunUrzadzenie(testowe2); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow

for (int i = 0; i < 5; i++)
{
    int id = i % 3;
    Urzadzenie testowe1 = new Urzadzenie(id, repo);
    ctx.Urzadzenia.Add(testowe1);
    ctx.SaveChanges(); //wyjac z petli?

    repo.dodajUrzadzenie(testowe1);
    mvb.dodajUrzadzenie(testowe1);
}

//wzory testow
Stopwatch sw;
//wyszukiwanie po dacie i id
DateTime data = testowe2.dataOstatniejModyfikacji;
int szukane_id = 0;
Urzadzenie? szukane = null;
sw = Stopwatch.StartNew();
for (int i = 0;  i < 10; i++)
    szukane = ctx.Urzadzenia
        .AsNoTracking()
        .Where(u => u.dataOstatniejModyfikacji <= data)
        .Where(u => u.dataWygasniecia > data)
        .Where(u => u.UrzadzenieID == szukane_id)
        .FirstOrDefault();
long czas_baza = sw.ElapsedMilliseconds;
if (szukane == null)
    Console.WriteLine("Blad: Baza nie odnalazla rekordu.");
else
    Console.WriteLine("Baza: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_baza + " ms.");
sw = Stopwatch.StartNew();
for (int i = 0; i < 10; i++)
    szukane = mvb.szukaj(0, data);
long czas_mvb = sw.ElapsedMilliseconds;
Console.WriteLine("MVB: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_mvb + "ms.");

//wyszukiwanie ostatniej wersji po id
sw = Stopwatch.StartNew();
for (int i = 0; i < 10; i++)
{
    szukane = ctx.Urzadzenia
        .AsNoTracking() //nie uzywamy zbuforowanych (wynikow poprzednich wykonan)
        .Where(u => u.UrzadzenieID == 2)
        .OrderByDescending(u => u.Wersja)
        .FirstOrDefault();
}
czas_baza = sw.ElapsedMilliseconds;
Console.WriteLine("Baza: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_baza + " ms.");
sw = Stopwatch.StartNew();
for (int i = 0; i < 10; i++)
    szukane = mvb.szukaj(2);
czas_mvb = sw.ElapsedMilliseconds;
Console.WriteLine("MVB: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " +  czas_mvb + " ms.");

//wyszukiwanie po id i wersji
sw = Stopwatch.StartNew();
for (int i = 0; i < 10; i++)
    szukane = ctx.Urzadzenia
    .AsNoTracking()
    .FirstOrDefault(u => u.UrzadzenieID == 2 && u.Wersja == 0);

czas_baza = sw.ElapsedMilliseconds;
Console.WriteLine("Baza: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_baza + " ms.");
sw = Stopwatch.StartNew();
for (int i = 0; i < 10; i++) 
    szukane = mvb.szukaj(2, 0);
czas_mvb = sw.ElapsedMilliseconds;
Console.WriteLine("MVB: " + szukane.UrzadzenieID + "v" + szukane.Wersja + " w czasie: " + czas_mvb + " ms.");


mvb.wypiszDrzewo();
ctx.Dispose();