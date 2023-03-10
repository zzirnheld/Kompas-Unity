using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace KompasCore.Helpers
{
    public class DeckLoadHelper
    {
        private static string deckFilesFolderPath;
        public static string DeckFilesFolderPath 
        {
            get
            {
                if (deckFilesFolderPath == default) deckFilesFolderPath = Path.Combine(Application.persistentDataPath, "Decks");
                return deckFilesFolderPath;
            }
        }

        public static IList<string> CleanAndSplit(string deckList)
        {
            deckList = deckList.Replace("\u200B", "");
            deckList = deckList.Replace("\r", "");
            deckList = deckList.Replace("\t", "");
            var cardNames = new List<string>(deckList.Split('\n'));
            return cardNames;
        }

        public static IList<string> LoadDeck(string deckName)
        {
            string filePath = Path.Combine(deckFilesFolderPath, $"{deckName}.txt");
            string deckList = File.ReadAllText(filePath);
            return CleanAndSplit(deckList);
        }
    }
}