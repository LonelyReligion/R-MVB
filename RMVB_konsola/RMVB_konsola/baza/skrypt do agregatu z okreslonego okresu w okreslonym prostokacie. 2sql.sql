DECLARE @poczatek datetime2(3) = '2026-09-24 20:34:38.262';
DECLARE @koniec datetime2(3) = '2026-09-24 20:34:38.403';

SELECT AVG(Wartosc)
FROM
	( 
	SELECT U.UrzadzenieID, MAX(W.WersjaID)WersjaID
	FROM
	(
		SELECT * 
		FROM Urzadzenies AS U 
		WHERE 15 <= U.Dlugosc 
		AND   22 >= U.Dlugosc 
		AND   49 <= U.Szerokosc 
		AND   52 >= U.Szerokosc
	) AS U
	JOIN Wersjas AS W ON W.UrzadzenieID = U.UrzadzenieID
	WHERE W.dataOstatniejModyfikacji >= @poczatek 
	AND 
	(
	W.dataWygasniecia < @koniec OR 
	(W.dataWygasniecia = '9999-12-31 23:59:59.9999999' AND @koniec = '9999-12-31 23:59:59.9999999'))
	GROUP BY U.UrzadzenieID
) AS W
JOIN WersjaPomiars AS WP ON (W.UrzadzenieID = WP.Wersja_UrzadzenieID AND WP.Wersja_WersjaID = W.WersjaID)
JOIN Pomiars AS P ON WP.Pomiar_PomiarID = P.PomiarID


AND P.dtpomiaru >= @poczatek
AND P.dtpomiaru <  @koniec;
