using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Balda_Vcs {
	class SerializerMenu : SerializeGame {
		public void Menu() {
			char ch = default;
			while (ch != 'a' && ch != 'b' && ch != 'c') {

				Console.Write(" a - Continue last pvp game;\n b - continue last vs computer game;\n c - start new game;\n>>");
				ch = char.Parse(Console.ReadLine());
			}
			switch (ch) {
				case 'a':
					if (File.Exists("PVPSaveGame.bin")) {
						DeserializePVP();
					}
					else Console.WriteLine("Sory, there no saved PVP game...");
					break;
				case 'b':
					if (File.Exists("AISaveGame.bin")) {
						DeserializeAI();
					}
					else Console.WriteLine("Sory, there no saved vs Computer game...");
					break;
				case 'c':
					break;
			}
		}
	}
}
