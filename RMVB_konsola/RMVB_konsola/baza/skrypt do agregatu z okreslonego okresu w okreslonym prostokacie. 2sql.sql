SELECT AVG(Wartosc)
FROM
(
	SELECT * 
	FROM Urzadzenies AS U 
	WHERE 14 <= U.Dlugosc 
	AND   21 >= U.Dlugosc 
	AND   50 <= U.Szerokosc 
	AND   51 >= U.Szerokosc
) AS U

JOIN WersjaPomiars AS WP ON U.UrzadzenieID = WP.Wersja_UrzadzenieID
JOIN Wersjas AS W ON W.WersjaID = WP.Wersja_WersjaID
JOIN Pomiars AS P ON WP.Pomiar_PomiarID = P.PomiarID

WHERE W.dataOstatniejModyfikacji >= '2026-09-23 17:02:49.351' 
AND (W.dataWygasniecia < '2026-09-23 17:02:49.419' OR W.dataWygasniecia = '9999-12-31 23:59:59.9999999')

AND P.dtpomiaru >= '2026-09-23 17:02:49.351' 
AND P.dtpomiaru < '2026-09-23 17:02:49.419';
