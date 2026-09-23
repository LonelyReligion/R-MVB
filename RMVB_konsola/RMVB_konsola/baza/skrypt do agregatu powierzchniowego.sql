SELECT COALESCE(AVG(P.Wartosc), 0)
FROM(
	SELECT UrzadzenieID
	FROM Urzadzenies AS U
	WHERE 16 <= U.Dlugosc
	AND   22 >= U.Dlugosc
	AND   49 <= U.Szerokosc 
	AND   52 >= U.Szerokosc
) AS U
INNER JOIN (
    SELECT UrzadzenieID, MAX(WersjaID) WersjaID
    FROM Wersjas AS W
    GROUP BY W.UrzadzenieID
) AS W ON U.UrzadzenieID = W.UrzadzenieID
JOIN WersjaPomiars AS WP ON (WP.Wersja_UrzadzenieID = U.UrzadzenieID AND WP.Wersja_WersjaID = W.WersjaID)
JOIN Pomiars AS P ON P.PomiarID = WP.Pomiar_PomiarID