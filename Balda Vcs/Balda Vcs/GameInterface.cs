
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balda_Vcs {
	[Serializable]
	class GameInterface {
		
		protected (int PlPoints, List<string> PlWords) FirstPlayer;
		protected (int PlPoints, List<string> PlWords) SecondPlayer;
		[NonSerialized]
		protected readonly int ROW = 5;//variables are responsible for the size of the field
		[NonSerialized]
		protected readonly int COL = 5;
		protected  Random random = new Random();
		protected char[,] _table;

		protected GameInterface() {
			FirstPlayer.PlWords = new List<string>();
			SecondPlayer.PlWords = new List<string>();
			FillPlayTable();
		}
		
		public void Show1Menu() {
			Console.Write("Select a mode:\na. 2 players\tb. VS computer\n>>");
		}
		public void Show2Menu() {
			Console.Write("Select the AI level:\na. standart\tb. hard\n>>");
		}

		protected void SetColor(ConsoleColor text, ConsoleColor background) {
			Console.ForegroundColor = text;
			Console.BackgroundColor = background;
		}
		public void LoadTitle() {
			using (FileStream reader = new FileStream("title.txt", FileMode.Open, FileAccess.Read)) {
				using (StreamReader sr = new StreamReader(reader, Encoding.Default)) {
					while (!sr.EndOfStream) {
						string str = sr.ReadLine();
						foreach (char ch in str) {
							switch (ch) {
								case '*':
									SetColor(ConsoleColor.Green, ConsoleColor.Black);
									Console.Write(ch);
									break;
								case ':':
									SetColor(ConsoleColor.Yellow, ConsoleColor.Black);
									Console.Write(ch);
									break;
								default:
									SetColor(ConsoleColor.Blue, ConsoleColor.Black);
									Console.Write(ch);
									break;
							}
						}

						Console.WriteLine();
					}
				}
			}
		}

		protected void FillPlayTable() {
			_table = new char[ROW, COL];
			for (int i = 0; i < ROW; i++) {
				for (int j = 0; j < COL; j++) {
					_table[i, j] = '-';
				}
			}
		}

		protected void ShowPlayField() {
			Console.Clear();
			LoadTitle();
			Console.WriteLine("\t\t\t\t   0 1 2 3 4");
			for (int i = 0; i < ROW; i++) {
				SetColor(ConsoleColor.Green, ConsoleColor.Black);
				Console.Write($"\t\t\t\t{i}: ");
				for (int j = 0; j < COL; j++) {
					if (_table[i, j] != '-') SetColor(ConsoleColor.Black, ConsoleColor.Blue);
					else if (_table[i, j] == '-') SetColor(ConsoleColor.Blue, ConsoleColor.Blue);
					Console.Write($"{_table[i, j]} ");
				}

				Console.WriteLine();
			}
			SetColor(ConsoleColor.Blue, ConsoleColor.Black);
			Console.Write("1st player words: ");
			foreach (string plWord in FirstPlayer.PlWords) {
				Console.Write($"{plWord}, ");
			}

			Console.WriteLine("\b\b.");
			Console.WriteLine($"Points: {FirstPlayer.PlPoints}");

			Console.Write("2nd player words: ");
			foreach (string plWord in SecondPlayer.PlWords) {
				Console.Write($"{plWord}, ");
			}

			Console.WriteLine("\b\b.");
			Console.WriteLine($"Points: {SecondPlayer.PlPoints}\n");
		}

		protected bool CheckingFreePlaces() {
			for (int i = 0; i < ROW; i++) {
				for (int j = 0; j < COL; j++) {
					if (_table[i, j] == '-') return true;
				}
			}
			return false;
		}

		protected void CleanScrAndContinue() {
			Console.Write("Tap any key to continue...");
			Console.ReadKey(true);
			Console.Clear();
		}
	}
}
