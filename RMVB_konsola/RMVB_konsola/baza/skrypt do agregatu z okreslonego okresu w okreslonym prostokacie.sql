SELECT AVG(P.Wartosc)
FROM (
	SELECT DISTINCT P.PomiarID, P.Wartosc, P.dtpomiaru 
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
	JOIN Pomiars AS P ON P.PomiarID = WP.Pomiar_PomiarID
) AS P
WHERE P.dtpomiaru >= '2026-09-23 17:02:49.351' 
AND P.dtpomiaru < '2026-09-23 17:02:49.419';