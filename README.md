# RMVB_konsola
Aplikacja konsolowa symulująca działanie urządzeń mierzących temperaturę przechowująca dane symulowanych urządzeń w lokalnej bazie danych (SQL Server Express LocalDB) oraz za pomocą indeksu [R-MVB](https://link.springer.com/chapter/10.1007/978-3-540-68111-3_22). 
Aplikacja umożliwia porównanie czasów realizacji typowych zapytań w bazie danych oraz za pomocą indeksu.

## Technologie
* EF6
* C#
* LINQ

## Instrukcja obsługi
### Pliki wynikowe
Efektem działania programu są następujące pliki:
* Test.txt &mdash; wyniki przeprowadzonych pomiarów czasowych
* MVB.txt &mdash; wynikowe drzewo MVB
* Bledy.txt &mdash; komunikaty błędów ostatniego zrealizowanego testu
  
</br>W zależności od tego, czy testy się powiodły pojawi się plik Bledy.txt lub Test.txt.

### Pliki wejściowe
* App.config &mdash; domyślne wartości konfiguracyjne

# UrzadzeniaSim
(ta część jest obecnie w budowie)
