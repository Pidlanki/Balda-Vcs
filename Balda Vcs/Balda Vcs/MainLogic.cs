using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda_Vcs {
	[Serializable]
	class MainLogic : GameInterface {

		protected Dictionary<string, int> Library;
		protected string f_word;
		protected string currentWord; //the current word collected
		[NonSerialized]
		string wordsFileName = "words.txt";

		private List<int> moves;
		protected int step_count = 0;//counter moves
		protected int x, y; //coordinates of active table cell
		protected int temp_x, temp_y; // temporary variables to remember the cell with the letter inserted

		public MainLogic() {
			moves = new List<int>();
			LoadWords();
		}

		protected void LoadWords() {
			Library = new Dictionary<string, int>();
			int i = 0;
			using (FileStream reader = new FileStream(wordsFileName, FileMode.Open, FileAccess.Read)) {
				using (StreamReader sr = new StreamReader(reader, Encoding.Default)) {
					while (!sr.EndOfStream) {
						string word = sr.ReadLine();
						if (Library.ContainsKey(word)) continue;
						Library.Add(word, i++);
					}
				}

			}

		}

		public char GameMenu() {
			FillPlayTable();
			char ch = 'y';
			//string f_word;

			Console.Write("Enter the first five-letter word, or press \"r\" to select a random word\n>>");
			bool isCorrect = false;
			do {
				//if (ch == 'n') Console.Write($"Please re - enter the first word(it must contain exactly {COL} leters)\n>>");
				f_word = Console.ReadLine();
				if (Library.ContainsKey(f_word) && f_word.Length == 5) {
					isCorrect = true;
				}
				else if (f_word == "r") isCorrect = true;
				else Console.Write($"The word \"{f_word}\" can't be find in library. Try again\n>>");
			} while (!isCorrect);
			Game();

			Console.Write("Want to play again?\n(y/n)>>");
			ch = char.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
			return ch;
		}
		protected virtual void Game() {
			FirstWordInitialization();
			do {
				currentWord = "";
				ShowPlayField();
				step_count++;
				SetColor(ConsoleColor.Red, ConsoleColor.Black);
				Console.WriteLine(step_count % 2 == 1 ? "The 1st player turn:" : "The 2nd player turn:");
				SetColor(ConsoleColor.Blue, ConsoleColor.Black);
				var isEndMoving = Moving();
				if (isEndMoving) CountingPoints();
			} while (CheckingFreePlaces());
			CheckForWinner();
		}

		protected void FirstWordInitialization() {
			if (f_word == "r") {
				while (f_word.Length != COL) {
					f_word = Library.ToArray()[random.Next(Library.Count - 1)].Key;
				}
			}
			for (int i = 0; i < COL; i++) {
				_table[ROW / 2, i] = f_word[i];
			}
		}

		protected void FirstMove(int count) {
			if (count == 0) {
				var isCorrect = true;
				do {
					isCorrect = true;
					try {
						Console.Write("Coordinates of X: ");
						x = int.Parse(Console.ReadLine());
						Console.Write("Coordinates of Y: ");
						y = int.Parse(Console.ReadLine());
					}
					catch {
						isCorrect = false;
						Console.Write("Invalid coordinates, enter the coordinates of the cell next to another letter to form a word\n");
					}

					//if (CheckPos()) Console.Write("Invalid coordinates, enter the coordinates of the cell next to another letter to form a word\n>> ");

				} while (CheckPos() || !isCorrect);
			}
		}
		protected bool Moving() {
			int count = 0;
			bool isNewLatUsed = false;
			do {
				FirstMove(count);
				//чи не занята клітинка
				if (count == 0) {
					isNewLatUsed = EnterFirstLetter();
					count++;
				}

				Console.Write("Next move\n>>");
				var move = char.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
				switch (move)//процес переміщення по дошці
				{
					case 'w':
						if (MakeMove(x - 1, y, ref isNewLatUsed)) {
							count++;
							x--;
						}
						else Console.Write("Invalid coordinates, or this cell is already in use. Try again!\n");
						break;
					case 'd':
						if (MakeMove(x, y + 1, ref isNewLatUsed)) {
							count++;
							y++;
						}
						else Console.Write("Invalid coordinates, or this cell is already in use. Try again!\n");

						break;
					case 's':
						if (MakeMove(x + 1, y, ref isNewLatUsed)) {
							count++;
							x++;
						}
						else Console.Write("Invalid coordinates, or this cell is already in use. Try again!\n");
						break;
					case 'a':
						if (MakeMove(x, y - 1, ref isNewLatUsed)) {
							count++;
							y--;
						}
						else Console.Write("Invalid coordinates, or this cell is already in use. Try again!\n");
						break;
					case 'Q':
						if (EndTurn(ref isNewLatUsed)) {
							count = 0;
							moves = new List<int>();
							return true;
						}
						else {
							count = 0;
							moves = new List<int>();
							step_count--;
							return false;
						}
						break;
					default:
						break;
				}
			} while (true);
		}

		protected void CountingPoints() {
			if (step_count % 2 == 1) { // визначає чій був хід
				FirstPlayer.PlWords.Add(currentWord);
				FirstPlayer.PlPoints += currentWord.Length;

			}
			else {
				SecondPlayer.PlWords.Add(currentWord);
				SecondPlayer.PlPoints += currentWord.Length;
			}
		}

		protected void CheckForWinner() {
			if (FirstPlayer.PlPoints > SecondPlayer.PlPoints) Console.WriteLine("The first player wins!");
			else if (FirstPlayer.PlPoints < SecondPlayer.PlPoints) Console.WriteLine("The second player wins!");
			else Console.WriteLine("Draw!");
		}
		/// <summary>
		/// Checking neighbors for moving to them
		/// </summary>
		/// <returns> return true if cell have a posible moves, else false</returns>
		bool CheckPos() {
			bool r = false, l = false, u = false, d = false;
			if (x < COL && x >= 0 && y < ROW && y >= 0) {
				if (x + 1 < COL)
					if (_table[x + 1, y] != '-' && !CheckPastPos(x + 1, y)) u = true;
				if (x - 1 >= 0)
					if (_table[x - 1, y] != '-' && !CheckPastPos(x - 1, y)) d = true;
				if (y + 1 < ROW)
					if (_table[x, y + 1] != '-' && !CheckPastPos(x, y + 1)) r = true;
				if (y - 1 >= 0)
					if (_table[x, y - 1] != '-' && !CheckPastPos(x, y - 1)) l = true;
				return u != true && d != true && l != true && r != true;
			}
			else return true;
		}

		bool CheckPastPos(int x, int y) {
			if (moves.Count != 0) {
				for (int i = 0; i < moves.Count; i += 2) {
					if (moves[i] == x && moves[i + 1] == y) return true;
				}
			}
			return false;
		}

		bool EnterFirstLetter() {
			if (_table[x, y] == '-') {
				Console.Write("Enter the first letter\n>> ");
				var letter = char.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
				currentWord += letter;
				_table[x, y] = letter;
				temp_x = x;
				temp_y = y;

				moves.Add(x);
				moves.Add(y);
				return true;
			}
			else if (_table[x, y] != '-') {
				currentWord += _table[x, y];
				moves.Add(x);
				moves.Add(y);
			}

			return false;
		}

		bool MakeMove(int x, int y, ref bool isNewLatUsed) {
			char letter;
			if (x < ROW && y < COL && x >= 0 && y >= 0) {
				if (_table[x, y] != '-' && x >= 0 && !CheckPastPos(x, y)) {
					currentWord += _table[x, y];
					Console.WriteLine(currentWord);
					moves.Add(x);
					moves.Add(y);
					return true;
				}

				if (_table[x, y] == '-' && x >= 0 && !isNewLatUsed) {
					Console.Write("Enter a letter\n>>");
					letter = char.Parse(Console.ReadLine() ?? throw new InvalidOperationException());
					currentWord += letter;
					_table[x, y] = letter;
					Console.WriteLine(currentWord);
					temp_x = x;
					temp_y = y;
					isNewLatUsed = true;
					moves.Add(x);
					moves.Add(y);
					return true;
				}
			}
			return false;
		}

		bool EndTurn(ref bool isNewLatUsed) {
			bool incorrectPos = false;
			if (FirstPlayer.PlWords.Contains(currentWord) || SecondPlayer.PlWords.Contains(currentWord) || currentWord == f_word) {
				Console.Write("This word has already been used, think further! To skip the turn type(y/n)\n>>");
				incorrectPos = true;
			}
			else if (!Library.ContainsKey(currentWord)) {
				Console.Write($"The word \"{currentWord}\" can't be find in library. To skip the turn type(y/n)\n>>");
				incorrectPos = true;
			}
			if (incorrectPos) {
				var move = char.Parse(Console.ReadLine());
				if (isNewLatUsed) _table[temp_x, temp_y] = '-';
				isNewLatUsed = false;
				currentWord = "";
				ShowPlayField();
				while (move != 'y' && move != 'n') {
					Console.Write("Make a correct choice by typing \"y\" to skip, or \"n\" to try again)\n>>");
					move = char.Parse(Console.ReadLine());

				}
				return move == 'y';
			}
			CleanScrAndContinue();
			return true;
		}
	}

}
