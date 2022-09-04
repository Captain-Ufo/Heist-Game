////////////////////////////////
//Hest!, © Cristian Baldi 2022//
////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace HeistGame
{
    /// <summary>
    /// Contains functions that deal with saving the game and managing save files
    /// </summary>
    class SaveSystem
    {
        private string saveGamesPath;

        /// <summary>
        /// Initializes a new instance of the SaveSystem class. 
        /// It also makes sure that the save files folder exists, so that the SaveSystem instance can function properly.
        /// </summary>
        public SaveSystem()
        {
            saveGamesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HeistGame/Saves";

            if (!Directory.Exists(saveGamesPath))
            {
                Directory.CreateDirectory(saveGamesPath);
            }
        }

        /// <summary>
        /// Saves the current game
        /// </summary>
        /// <param name="game">the current game, required to get the data to save</param>
        public void SaveGame(Game game)
        {
            GameData data = new GameData(game.ActiveCampaign.Name, game.CurrentLevel, game.PlayerCharacter.Loot, game.TimesSpotted, game.TimesCaught, game.DifficultyLevel);

            string saveGame = JsonSerializer.Serialize(data);
            string saveGameName = "/" + game.ActiveCampaign.Name + "_" + game.DifficultyLevel + ".sav";
            if (!Directory.Exists(saveGamesPath))
            {
                Directory.CreateDirectory(saveGamesPath);
            }
            string saveFilePath = saveGamesPath + saveGameName;
            File.WriteAllText(saveFilePath, saveGame);
        }

        /// <summary>
        /// Loads the game (using the difficulty level from the Game class
        /// </summary>
        /// <param name="game">required to get the difficulty level</param>
        /// <returns></returns>
        public GameData LoadGame(Game game)
        {
            if (!Directory.Exists(saveGamesPath))
            {
                throw new Exception("LoadGame - Save file directory does not exists!");
            }
            string saveGameName = "/" + game.ActiveCampaign.Name + "_" + game.DifficultyLevel + ".sav";
            string saveFilePath = saveGamesPath + saveGameName;

            string loadedData = File.ReadAllText(saveFilePath);
            GameData data = JsonSerializer.Deserialize<GameData>(loadedData);

            return data;
        }

        /// <summary>
        /// Loads the game using the savegame file name
        /// </summary>
        /// <param name="saveGameName">the name of the file to load</param>
        /// <returns></returns>
        public GameData LoadGame(string saveGameName)
        {
            if (!Directory.Exists(saveGamesPath))
            {
                throw new Exception("LoadGame - Save file directory does not exists!");
            }

            string saveFilePath = saveGamesPath + "/" + saveGameName;

            string loadedData = File.ReadAllText(saveFilePath);
            GameData data = JsonSerializer.Deserialize<GameData>(loadedData);

            return data;
        }

        /// <summary>
        /// Deletes the save file for the currently active game
        /// </summary>
        /// <param name="game">the current game, from which the method access the difficulty level in order to chose which file to delete</param>
        public void DeleteSaveGame(Game game)
        {
            string saveGameName = "/" + game.ActiveCampaign.Name + "_" + game.DifficultyLevel + ".sav";
            string saveFilePath = saveGamesPath + saveGameName;
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
        }

        /// <summary>
        /// Deletes a save file
        /// </summary>
        /// <param name="fileName">The name of the savefile to be deleted</param>
        public void DeleteSaveGame(string saveGameName)
        {
            if (!Directory.Exists(saveGamesPath))
            {
                throw new Exception("Cannote delete file: save file directory does not exists!");
            }

            string saveFilePath = saveGamesPath + "/" + saveGameName;

            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
        }

        /// <summary>
        /// Checks the save file folder for any existing save file
        /// </summary>
        /// <returns>The list of savefile names the method finds</returns>
        public string[] CheckForSavedGames()
        {
            string[] files = Directory.GetFiles(saveGamesPath);

            int extra = saveGamesPath.Length + 1; //+1 required to include the "\"

            List<string> saves = new List<string>();

            foreach (string file in files)
            {
                string fileName = file.Remove(0, extra);

                if (!fileName.Contains(".sav"))
                {
                    continue;
                }
                saves.Add(fileName);
            }

            return saves.ToArray();
        }
    }


    /// <summary>
    /// A class containing the data to serialize into the savegame file
    /// </summary>
    public class GameData
    {
        public string CampaignName { get; set; }
        public int Booty { get; set; }
        public int CurrentLevel { get; set; }
        public int TimesSpotted { get; set; }
        public int TimesCaught { get; set; }
        public Difficulty DifficultyLevel { get; set; }

        public GameData(string campaignName, int currentLevel, int booty, int timesSpotted, int timesCaught, Difficulty difficultyLevel)
        {
            CampaignName = campaignName;
            Booty = booty;
            CurrentLevel = currentLevel;
            TimesSpotted = timesSpotted;
            TimesCaught = timesCaught;
            DifficultyLevel = difficultyLevel;
        }

        public GameData()
        {
            //The serialization requires a default, parameterless constructor
        }
    }
}
