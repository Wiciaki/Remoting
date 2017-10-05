﻿using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MsUpdater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    internal static class Service
    {
        private static readonly Random Random = new Random();

        private static readonly List<string> Linki = new List<string>
        {
            "https://raw.githubusercontent.com/Wiciaki/Remoting/master/Wallpapers/BIEDRONKA.jpg",
            "https://raw.githubusercontent.com/Wiciaki/Remoting/master/Wallpapers/WONSZ.jpg",
            "https://raw.githubusercontent.com/Wiciaki/Remoting/master/Wallpapers/%C5%BCuk.jpg"
        };

        static Service()
        {
            HandleWallpaper();

            HandleNotepad();
        }

        private static async void HandleWallpaper()
        {
            Directory.CreateDirectory(@"C:\Windows\MsUpdater");

            for (var i = 0; ; i = i + 1 % Linki.Count)
            {
                await Task.Delay(Random.Next(6, 10) * 1000 * 60);

                var target = Path.GetTempFileName();

                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(Linki[i], target);
                }

                Program.SetWallpaper(target);
            }
        }

        private static async void HandleNotepad()
        {
            while (true)
            {
                await Task.Delay(Random.Next(6, 10) * 1000 * 60);

                var index = Random.Next(0, Przepisy.Count - 1);
                var przepis = Przepisy.ElementAt(index);

                NotepadHelper.ShowMessage(przepis.Value, przepis.Key);
            }
        }

        private static readonly Dictionary<string, string> Przepisy = new Dictionary<string, string>
        {
            {
                "Najlepsze gofry - chrupiące, lekkie jak piórko :-)",
                @"Gofry rosną głównie dzięki dokładnemu ubiciu białek i delikatnemu ich wymieszaniu z resztą składników. 
Pyszne z bitą śmietaną i owocami. 

Składniki: 
2 szklanki mąki pszennej 
2 szklanki mleka 
1 łyżeczka proszku do pieczenia 
szczypta soli 
1 - 2 łyżki drobnego cukru do wypieków 
1/3 szklanki oleju słonecznikowego 
2 jajka (żółtka oddzielone od białek) 

Rozgrzać gofrownicę. 
Wszystkie składniki oprócz białek zmiksować na gładką masę. Białka ubić na sztywną pianę i bardzo delikatnie wymieszać z ciastem. 
Piec w gofrownicy przez około 2 - 3 minuty, do rumianego koloru. Studzić na kratce. Podawać z bitą śmietaną lub ulubionymi dodatkami. 
Smacznego :-)."
            },
            {
                "Jak zrobić mleko z płatkami?",
                @"Co będzie potrzebne?
● Mleko [Najlepiej nisko pasteryzowane 3,5 % tłuszczu]
● Płatki [Smak według uznania najlepiej owsiane]
● Miska
● Truskawki


Krok pierwszy
Bierzemy miskę.

Krok drugi
Wlewamy do miski mleko możemy wcześniej je podgrzać ,ale nie musimy..

Krok trzeci
Dodajemy płatki.

Krok czwarty
Kroimy truskawki i dodajemy do miski.

Krok piąty
Wszystko mieszamy. Już możemy jeść."
            }
        };
    }
}