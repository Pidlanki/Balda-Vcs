using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Balda_Vcs {

	class Program {
		static void Main(string[] args) {
			char ch = default;

			MainLogic Balda = new MainLogic();

			Balda.LoadTitle();
			do {
				while (ch != 'a' && ch != 'b') {
					Balda.Show1Menu();
					ch = char.Parse(Console.ReadLine());
				}
				switch (ch) {
					case 'a':
						ch = Balda.GameMenu();
						break;
					case 'b':
						ch = default;
						while (ch != 'a' && ch != 'b') {
							Console.Write($"Choose difficulty: a - standart\tb - hard\n>>");
							ch = char.Parse(Console.ReadLine());
						}
						AiLogic AiBalda = new AiLogic((ch != 'a'));
						ch = AiBalda.GameMenu();
						break;
					default:
						break;
				}
			} while (ch != 'n');

		}
	}
}
