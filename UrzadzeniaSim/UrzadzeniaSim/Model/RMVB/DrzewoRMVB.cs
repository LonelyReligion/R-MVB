using System.Collections.ObjectModel;
using System.IO;
using UrzadzeniaSim.Model.DB;
using UrzadzeniaSim.Model.RMVB.MVB;
using UrzadzeniaSim.Model.RMVB.R;

namespace UrzadzeniaSim.Model.RMVB;

public class DrzewoRMVB
{
    private Kontekst _ctx;
    private DrzewoMVB _mvb;
    private RTreeAdapter _r;
    private Repo _repo;
    public static string s_SciezkaFolderuWyjsciowego = Directory.GetCurrentDirectory();
    internal DrzewoRMVB(Kontekst ctx)
    {
        this._ctx = ctx;
        _repo = new Repo();
        _mvb = new DrzewoMVB(_repo, ctx);
        _r = new RTreeAdapter(new RTree(_repo, ctx));
    }

    internal Repo zwrocRepo() { return _repo; }
    internal bool czyUrzadzenieIstnieje(int id) { return _repo.czyUrzadzenieIstnieje(id); }
    internal DrzewoMVB zwrocMVB() { return _mvb; }
    internal void wypiszMVB()
    {
        foreach (String linijka in _mvb.drukujDrzewo())
            Console.WriteLine(linijka);
    }
    //dodaj
    internal void dodajUrzadzenie(Urzadzenie_Model u)
    {
        _repo.saveDevice(u);
        _r.dodajUrzadzenie(u);
    }

    internal void dodajUrzadzenia(List<Urzadzenie_Model> urzadzenia)
    {
        _repo.saveDevices(urzadzenia);
        
        foreach(Urzadzenie_Model u in urzadzenia)
            _r.dodajUrzadzenie(u);
    }


    internal void dodajWersje(Wersja w)
    {
        _repo.saveVersion(w);
        _mvb.dodajUrzadzenie(w);
    }

    internal void dodajWieleWersji(List<Wersja> w)
    {
        _repo.saveVersions(w);

        foreach(Wersja wersja in w)
            _mvb.dodajUrzadzenie(wersja);

    }
    internal void dodajPomiar(int UrzadzenieID, Pomiar p, Wersja alfa)
    {

        _ctx.Wersje.Attach(alfa);
        _ctx.Entry(alfa).Collection(x => x.Pomiary).Load();

        alfa.Pomiary.Add(p);
        _ctx.Pomiary.Add(p);
        _ctx.SaveChanges();

        _r.dodajPomiar(UrzadzenieID, p);
    }

    //usun
    internal void usunWersje(Wersja w)
    {
        _mvb.dodajUrzadzenie(w); //musi zostac zapisana najpierw
        _mvb.usunUrzadzenie(w); //jawnie dezaktywujemy urzadzenie, sprawdzamy czy nie nastpil weakVersionUnderflow
        _repo.saveVersion(w);
    }

    //szukaj
    //wyszukiwanie wersji o UrządzenieID równym id i WersjaID równym v
    internal Wersja szukaj(int id, int v)
    {
        return _mvb.szukaj(id, v);
    }

    //wyszukiwanie wersji urządzenia o UrzadzenieID aktualnej w chwili dt
    internal Wersja szukaj(int id, DateTime dt)
    {
        return _mvb.szukaj(id, dt);
    }

    //wyszukiwanie ostatniej wersji o UrzadzenieID równym id
    internal Wersja szukaj(int id)
    {
        return _mvb.szukaj(id);
    }

    //wyszukiwanie wersji aktualnych w podanym przedziale czasowym
    internal List<Wersja> szukaj(DateTime poczatek, DateTime koniec)
    {
        return _mvb.szukaj(poczatek, koniec);
    }

    //zwraca listę urządzeń znajdujących się w zadanym prostokącie
    internal List<Urzadzenie_Model> szukaj(Rectangle rect)
    {
        return _r.szukaj(rect);
    }

    //zwraca urządzenie w podanym punkcie
    internal Urzadzenie_Model szukaj(Decimal x, Decimal y)
    {
        return _r.szukaj((Decimal)x, (Decimal)y);
    }

    //zwraca liczbę pomiarów i agregat czasowy (z czego?)
    internal (Decimal, Decimal) szukajAgregatu(Rectangle rect)
    {
        return _r.szukajAgregatuPowierzchniowego(rect);
    }

    //zwraca agregat czasowy urzadzenia
    internal Decimal szukajAgregatuCzasowego(Decimal x, Decimal y)
    {
        return _r.szukajAgregatuCzasowego(x, y);
    }

    //oblicza agregaty powierzchniowe
    internal void obliczAgregaty()
    {
        _r.obliczAgregaty();
    }

    internal void zapiszMVB(string v)
    {
        List<string> linijki = _mvb.drukujDrzewo();
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(v, "_mvb.txt")))
        {
            foreach (string linijka in linijki)
                outputFile.WriteLine(linijka);
        }
    }
    public void Reset()
    {
        MainWindow.s_UrzadzeniaUI = new ObservableCollection<Urzadzenie_Model>();
        _repo.Reset();
        _mvb = new DrzewoMVB(_repo, _ctx);
        _r = new RTreeAdapter(new RTree(_repo, _ctx));
    }
}
