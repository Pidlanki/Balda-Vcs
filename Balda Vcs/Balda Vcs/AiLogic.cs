using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Balda_Vcs {
	[Serializable]
	class AiLogic : MainLogic {
		private class TempAiWords {
			public List<string> words = new List<string>();
			public List<(int x, int y)> coordinates = new List<(int x, int y)>();
			public List<(int x, int y)> AiMoves = new List<(int x, int y)>();
			public int MaxLength = 10;
			public int CurrentMaxLength;
		}
		private class AiWord {
			public List<string> Words = new List<string>();
			public List<(int x, int y)> LetterCoordinates = new List<(int x, int y)>();
			public List<char> Letters = new List<char>();
		}


		private bool difficulty;
		private TempAiWords tempAiWords;
		private AiWord aiWord;

		private char[,] matrix;
		//int moves_count;//counts the number of movements in the turn

		public AiLogic(bool difficulty) {
			this.difficulty = difficulty;
			//moves_count = default;
			CleanAi();
		}

		void CleanAi() {
			matrix = new char[ROW, COL];
			tempAiWords = new TempAiWords();
			aiWord = new AiWord();
			currentWord = "";
		}
		protected override void Game() {
			FirstWordInitialization();
			do {
				CleanAi();
				ShowPlayField();
				step_count++;
				SetColor(ConsoleColor.Red, ConsoleColor.Black);
				Console.WriteLine(step_count % 2 == 1 ? "The player turn:" : "AI player turn:");
				SetColor(ConsoleColor.Blue, ConsoleColor.Black);
				var isEndMoving = step_count % 2 == 1 ? AiMoving() : AiMoving();
				if (isEndMoving) CountingPoints();
				CleanScrAndContinue();
			} while (CheckingFreePlaces());
			CheckForWinner();
		}

		private bool AiMoving() {
			FormingMatrix();
			string tempWord;
			int newX, newY;
			for (int i = 0; i < ROW; i++) {
				for (int j = 0; j < COL; j++) {
					if (matrix[i, j] == 48) continue; //if 0 continue
					newX = -1; newY = -1; // coordinates of new letter -1 - mean new letter dosn't used yet
					tempWord = "";
					tempAiWords.AiMoves.Clear();

					FindWords(i, j, newX, newY, tempWord);
				}
			}

			Filter();
			if (aiWord.Words.Count > 0) ChooseWord();
			else {
				Console.WriteLine("I'm passing this turn");
				CleanScrAndContinue();

			}

			return true;
		}

		private void ChooseWord() {
			int randomIndex = random.Next(aiWord.Words.Count - 1);
			if (difficulty) {
				int bigestInd = default;
				for (int i = 0; i < aiWord.Words.Count; i++) {
					if (aiWord.Words[i].Length > currentWord.Length) {
						currentWord = aiWord.Words[i];
						bigestInd = i;
					}
				}
				_table[aiWord.LetterCoordinates[bigestInd].x, aiWord.LetterCoordinates[bigestInd].y] =
					aiWord.Letters[bigestInd];

			}
			else {
				_table[aiWord.LetterCoordinates[randomIndex].x, aiWord.LetterCoordinates[randomIndex].y] =
					aiWord.Letters[randomIndex];
				currentWord = aiWord.Words[randomIndex];
			}
			aiWord.Words = new List<string>();
			aiWord.LetterCoordinates = new List<(int x, int y)>();
			aiWord.Letters = new List<char>();
		}

		private void Filter() {
			/*
			string[] splitedWord = new string[2];
			string let;
			while (tempAiWords.words.Count > 0) {
				splitedWord = tempAiWords.words[tempAiWords.words.Count - 1].Split('?');
				Regex regex = new Regex($"[{splitedWord[0]}][a-z][{splitedWord[1]}]");
				foreach (string key in Library.Keys) {
					if (regex.IsMatch(key) && !FirstPlayer.PlWords.Contains(key) && !SecondPlayer.PlWords.Contains(key) && key != f_word) 
					aiWord.Words.Add(key);
				}
			}
			*/
			string tempWord;
			char letter;
			while (tempAiWords.words.Count > 0) {
				if (!tempAiWords.words[0].Contains("?")) {
					tempAiWords.words.RemoveAt(0);
					tempAiWords.coordinates.RemoveAt(0);
					continue;
				}
				for (int i = 97; i <= 122; i++) {
					letter = (char)i;
					tempWord = tempAiWords.words[0];
					tempWord = tempWord.Replace('?', letter);

					if (!FirstPlayer.PlWords.Contains(tempWord) &&
						!SecondPlayer.PlWords.Contains(tempWord) && tempWord != f_word) {
						if (!aiWord.Words.Contains(tempWord) && Library.ContainsKey(tempWord)) {
							aiWord.Words.Add(tempWord);
							aiWord.Letters.Add((char)i);
							aiWord.LetterCoordinates.Add((tempAiWords.coordinates[0]
								.x, tempAiWords.coordinates[0]
								.y));

						}
					}
				}
				tempAiWords.words.RemoveAt(0);
				tempAiWords.coordinates.RemoveAt(0);
			}
		}

		void MakeRecursiveCall(int x, int y, int newX, int newY, string tempWord) {
			int nextPos;
			nextPos = CheckNextPosition(x + 1, y, newX);
			if (nextPos == 1 || nextPos == 2)
				FindWords(x + 1, y, newX, newY, tempWord);
			nextPos = CheckNextPosition(x - 1, y, newX);
			if (nextPos == 1 || nextPos == 2)
				FindWords(x - 1, y, newX, newY, tempWord);
			nextPos = CheckNextPosition(x + 1, y, newX);
			if (nextPos == 1 || nextPos == 2)
				FindWords(x, y + 1, newX, newY, tempWord);
			nextPos = CheckNextPosition(x + 1, y, newX);
			if (nextPos == 1 || nextPos == 2)
				FindWords(x, y - 1, newX, newY, tempWord);
		}

		private void FindWords(int x, int y, int newX, int newY, string tempWord) {
			if (tempWord.Length >= tempAiWords.CurrentMaxLength) return;
			int nextPos = CheckNextPosition(x, y, newX);
			if (nextPos == 0) return;
			if (nextPos == 2) {
				newX = x;
				newY = y;
				tempWord += matrix[x, y];
			}
			else tempWord += matrix[x, y];

			if (tempWord.Length > 1) {
				tempAiWords.words.Add(tempWord);
				tempAiWords.coordinates.Add((newX, newY));
			}
			tempAiWords.AiMoves.Add((x, y));
			MakeRecursiveCall(x, y, newX, newY, tempWord);
		}

		private int CheckNextPosition(int x, int y, int newX) {
			if (x < ROW && y < COL && x >= 0 && y >= 0) {
				if (matrix[x, y] != (char)63 && matrix[x, y] != (char)48 &&
					!tempAiWords.AiMoves.Contains((x, y))) return 1;
				if (matrix[x, y] == (char)63 && newX == -1 &&
					!tempAiWords.AiMoves.Contains((x, y))) return 2;
			}
			return 0;
		}

		private void FormingMatrix() {
			for (int i = 0; i < ROW; i++) {
				for (int j = 0; j < COL; j++) {
					if (_table[i, j] == '-')
						if (!CheckNeighbors(i, j)) matrix[i, j] = (char)48;
						else matrix[i, j] = (char)63;
					else if (_table[i, j] != '-') {
						matrix[i, j] = _table[i, j];
						//max length of word can't be more then number of letters in the field + 1 or 10
						if (tempAiWords.CurrentMaxLength < tempAiWords.MaxLength - 1) tempAiWords.CurrentMaxLength++;
					}
				}
			}
			tempAiWords.CurrentMaxLength++;
		}

		private bool CheckNeighbors(int x, int y) {
			bool r = false, l = false, u = false, d = false;
			if (x + 1 < COL)
				if (_table[x + 1, y] != '-') u = true;
			if (x - 1 >= 0)
				if (_table[x - 1, y] != '-') d = true;
			if (y + 1 < ROW)
				if (_table[x, y + 1] != '-') r = true;
			if (y - 1 >= 0)
				if (_table[x, y - 1] != '-') l = true;
			return u || d || l || r;
		}
	}
}
