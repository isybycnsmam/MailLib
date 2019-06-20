using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net.Mail;
using System.Globalization;

namespace MailLib.BaseClasses {
	public class Ovh : Account {

		public enum DomainName {
			Jmail,
			Next,
			Uu2
		}
		
		public Ovh(DomainName domain) {
			switch (domain) {
				case DomainName.Jmail:
					Domain = "jmail.ovh";
					break;
				case DomainName.Next:
					Domain = "next.ovh";
					break;
				case DomainName.Uu2:
					Domain = "uu2.ovh";
					break;
			}
		}

		/// <exception cref="MailBadLengthOrFormat">Length is lower than 1 or higher than 20</exception>
		/// <exception cref="HttpRequestException">Error in sending requests, contains inner exception</exception>
		/// <param name="length">Length of mail base 1 - 20 characters</param>
		public void SetMail(int length = 20) {

			if (length < 1 || length > 20)
				throw new MailBadLengthOrFormat();

			MailAdress = new MailAddress($"{RandomPart.GenerateString(length)}@{Domain}");

			setMailBase();

		}


		/// <exception cref="MailBadLengthOrFormat">Length is lower than 1 or higher than 20</exception>
		/// <exception cref="HttpRequestException">Error in sending requests, contains inner exception</exception>
		/// <param name="oldMails">List of previously generated mails to make shure that evry mail is generated only once</param>
		/// <param name="length">Length of mail base 1 - 20 characters</param>
		public void SetMail(Ovh[] oldMails, int length = 16) {

			if (length < 1 || length > 20)
				throw new MailBadLengthOrFormat();

			string buf = RandomPart.GenerateString(length);

			foreach (Ovh acc in oldMails)
				if (buf == acc.MailAdress.User)
					throw new MailIsAlreadyTaken();

			MailAdress = new MailAddress($"{buf}@{Domain}");

			setMailBase();

		}


		/// <exception cref="MailBadLengthOrFormat">Mil adress base is not 1-20 length or contains unallowed characterr</exception>
		/// <exception cref="HttpRequestException">Error in sending requests, contains inner exception</exception>
		/// <param name="customMail">Allowed characters a-z, A-Z, 0-9 with length 1-20</param>
		public void SetMail(string customMail) {

			if (!Regex.IsMatch(customMail, @"^[a-zA-Z0-9]{1,20}$"))
				throw new MailBadLengthOrFormat();

			MailAdress = new MailAddress($"{customMail}@{Domain}");

			setMailBase();

		}


		/// <exception cref="ParsingException">Parsing data from some page generates any null, empty or unexpected value, contains inner exception</exception>
		/// <exception cref="HttpRequestException">Error in sending requests, contains inner exception</exception>
		public void UpdateInbox() {

			var responseString = "";

			MatchCollection matches;

			try {

				var request = new HttpRequestMessage(HttpMethod.Post, "http://www.jmail.ovh/mailBoxListAjax.php");

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

				responseString = x.Content.ReadAsStringAsync().Result;

				x?.Dispose();

				if (string.IsNullOrEmpty(responseString))
					throw new ArgumentNullException("responseString is null or empty");

			}
			catch (Exception ex) { throw new HttpRequestException("UpdateInbox", ex); }


			try {
				matches = Regex.Matches(responseString, @"goDeleteMail\((?<Id>\d*)", RegexOptions.None, TimeSpan.FromSeconds(10));
			}
			catch (Exception ex) { throw new ParsingException("UpdateInbox", ex); }


			Mails = new List<Mail>();

			foreach (Match match in matches) {

				var id = 0;

				try {
					id = Convert.ToInt32(match.Groups["Id"].Value);
				}
				catch (Exception ex) { throw new ParsingException("UpdateInbox", ex); }

				Mails.Add(getMailDetails(id));

			}


		}


		private void setMailBase() {

			handler = new HttpClientHandler();
			client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(5) };

			setMailDomain();

			setPolishLanguage();
			
			try {

				if (string.IsNullOrEmpty(MailAdress.User))
					throw new ArgumentNullException("MailAdress.User is null or empty");

				var request = new HttpRequestMessage(HttpMethod.Post, "http://www.jmail.ovh/mailBox.php");

				request.Content = new StringContent($"mailBox={MailAdress.User}", Encoding.UTF8, "application/x-www-form-urlencoded");

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

				x?.Dispose();

			}
			catch (Exception ex) { throw new HttpRequestException("setMailBase", ex); }

		}

		private void setMailDomain() {
		
			try {

				if (string.IsNullOrEmpty(MailAdress.Host))
					throw new ArgumentNullException("MailAdress.Host is null or empty");

				var request = new HttpRequestMessage(HttpMethod.Get, $"http://www.jmail.ovh/welcome.php?at=@{MailAdress.Host}");

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

				x?.Dispose();

			}
			catch (Exception ex) { throw new HttpRequestException("setMailDomain", ex); }

		}

		private void setPolishLanguage() {

			try {

				var request = new HttpRequestMessage(HttpMethod.Get, $"http://www.jmail.ovh/cl.php?l=pl");

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

				x?.Dispose();

			}
			catch (Exception ex) { throw new HttpRequestException("setPolishLanguage", ex); }

		}

		private Mail getMailDetails(int id) {

			var responseString = "";

			Match match;

			try {

				var request = new HttpRequestMessage(HttpMethod.Get, "http://www.jmail.ovh/loadMailDetailsAjax.php?mailId=" + id.ToString());

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

				responseString = x.Content.ReadAsStringAsync().Result;

				x?.Dispose();

				if (string.IsNullOrEmpty(responseString))
					throw new ArgumentNullException("responseString is null or empty");

			}
			catch (Exception ex) { throw new HttpRequestException("getMailDetails", ex); }


			var mail = new Mail(ref client, GetContentMail, DeleteMail) { Id = id, MyAdress = MailAdress };


			try {
				match = Regex.Match(responseString, "\"(?<SenderBase>.+?)\".*?;(?<SenderMail>.+?)&gt", RegexOptions.None, TimeSpan.FromSeconds(10));
				if (string.IsNullOrEmpty(match.Groups["SenderBase"].Value) || string.IsNullOrEmpty(match.Groups["SenderMail"].Value))
					throw new ArgumentNullException("SenderBase or SenderMail is null or empty");
				mail.SenderAdress = new MailAddress(match.Groups["SenderMail"].Value, match.Groups["SenderBase"].Value);

				match = Regex.Match(responseString, @"Temat(.|\s|\n)*?col-md-11.>[\s]*(?<Topic>.+)\t{17}", RegexOptions.None, TimeSpan.FromSeconds(10));
				if (match.Groups["Topic"].Value == null)
					throw new ArgumentNullException("Topic is null");
				mail.Topic = match.Groups["Topic"].Value;
			}
			catch (Exception ex) { throw new ParsingException("getMailDetails", ex); }


			try {

				var dateMatch = Regex.Match(responseString, @"Data(.|\s|\n)*?col-md-11.>\W*(?<Day>\d+)\s(?<Month>[a-z]+)\s(?<Year>\d+),\s(?<Time>[\d:]+)", RegexOptions.None, TimeSpan.FromSeconds(10));

				if (string.IsNullOrEmpty(dateMatch.Groups["Day"].Value) || string.IsNullOrEmpty(dateMatch.Groups["Month"].Value) || string.IsNullOrEmpty(dateMatch.Groups["Year"].Value) || string.IsNullOrEmpty(dateMatch.Groups["Time"].Value))
					throw new ArgumentNullException("Day, Month, Year or Time value maybe null or empty");

				string month = "0";

				string[] months = { "sty", "lut", "mar", "kwi", "maj", "cze", "lip", "sie", "wrz", "paź", "lis", "gru" };

				for (int i = 0; i < months.Length; i++)
					if (months[i] == dateMatch.Groups["Month"].Value)
						month = (i + 1).ToString();

				if (month == "0")
					throw new ArgumentNullException("reading month error");

				if (month.Length != 2)
					month = "0" + month;

				mail.Date = DateTime.ParseExact($"{dateMatch.Groups["Day"].Value}:{month.ToString()}:{dateMatch.Groups["Year"].Value} {dateMatch.Groups["Time"].Value}", "dd:MM:yyyy HH:mm:ss", CultureInfo.InvariantCulture);

			}
			catch (Exception ex) { throw new ParsingException("getMailDetails", ex); }

			return mail;

		}


		#region Mail functions

		private void DeleteMail(int id) {
		
			try {

				var request = new HttpRequestMessage(HttpMethod.Post, "http://www.jmail.ovh/deleteMailAjax.php?mailId=" + id);

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

			}
			catch (Exception ex) { throw new HttpRequestException("DeleteMail", ex); }

		}

		private string GetContentMail(int id) {
		
			var responseString = "";

			try {

				var request = new HttpRequestMessage(HttpMethod.Get, "http://www.jmail.ovh/loadMailDetailsAjax.php?mailId=" + id);

				var x = client.SendAsync(request).Result;

				if (!x.IsSuccessStatusCode)
					throw new HttpRequestException("x.IsSuccessStatusCode = false");

				x?.Dispose();


				request = new HttpRequestMessage(HttpMethod.Get, "http://www.jmail.ovh/mailBody.php?mail=" + id);

				var y = client.SendAsync(request).Result;

				if (!y.IsSuccessStatusCode)
					throw new HttpRequestException("y.IsSuccessStatusCode = false");

				responseString = y.Content.ReadAsStringAsync().Result;

				y?.Dispose();

				if (string.IsNullOrEmpty(responseString))
					throw new ArgumentNullException("responseString is null or empty");

			}
			catch (Exception ex) { throw new HttpRequestException("DeleteMail", ex); }


			try {
				var match = Regex.Match(responseString, @"\W*.*\s*(?<Content>(\n|.|\s)*)<\/div>", RegexOptions.None, TimeSpan.FromSeconds(10));
				if (match.Groups["Content"].Value == null)
					throw new ArgumentNullException("Content is null");
				return HttpUtility.HtmlDecode(match.Groups["Content"].Value);
			}
			catch (Exception ex) { throw new ParsingException("GetContentMail", ex); }

		}

		#endregion

	}
}