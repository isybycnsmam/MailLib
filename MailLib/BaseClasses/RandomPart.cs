using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailLib.BaseClasses {
	internal static class RandomPart {

		public static Random rnd = new Random();

		public static string GenerateString(int length = 16) {

			var result = "";

			for (int i = 0; i < length; i++)
				result += rnd.Next(0, 2) == 0 ? (char)rnd.Next('0', '9' + 1) : (char)rnd.Next('a', 'z' + 1);

			return result;

		}

	}
}