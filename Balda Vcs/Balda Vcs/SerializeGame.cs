using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Balda_Vcs {
	class SerializeGame {


		public void SerializePVP(MainLogic game) {
			try {
				BinaryFormatter formatter = new BinaryFormatter();
				using (Stream st = File.Create("PVPSaveGame.bin")) {
					formatter.Serialize(st, game);

				}
				Console.WriteLine("Game was saved...");

			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		protected void DeserializePVP() {
			MainLogic game = new MainLogic();
			try {
				BinaryFormatter formatter = new BinaryFormatter();
				game = null;
				using (Stream st = File.OpenRead("PVPSaveGame.bin")) {
					game = (MainLogic)formatter.Deserialize(st);
				}

				Console.WriteLine(game);
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}

			game.ContinueMenu();

		}
		public void SerializeAI(AiLogic game) {
			try {
				BinaryFormatter formatter = new BinaryFormatter();
				using (Stream st = File.Create("AISaveGame.bin")) {
					formatter.Serialize(st, game);

				}
				Console.WriteLine("Game was saved...");
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
		}

		protected void DeserializeAI() {
			AiLogic game = new AiLogic();
			try {
				BinaryFormatter formatter = new BinaryFormatter();
				game = null;
				using (Stream st = File.OpenRead("AISaveGame.bin")) {
					game = (AiLogic)formatter.Deserialize(st);
				}

				Console.WriteLine(game);
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
			}
			game.ContinueMenu();

		}
	}
}
