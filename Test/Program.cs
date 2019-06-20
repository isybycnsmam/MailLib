using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using MailLib;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Test {
	class Program {

		static void Main(string[] args) {
		
			OvhNormalTest();
			
			//OvhUpdateTest();

			Console.ReadKey();

		}

		public static void OvhNormalTest() {
			MailSites.JmailOvh jmail = new MailSites.JmailOvh();
			
			jmail.SetMail("xd");
			Console.WriteLine("SetMail Ok");

			Console.WriteLine("MailAdress: " + jmail.MailAdress.Address);
			Console.WriteLine("MailAdress.DisplayName: " + jmail.MailAdress.DisplayName);
			Console.WriteLine("MailAdress.Host: " + jmail.MailAdress.Host);
			Console.WriteLine("MailAdress.User: " + jmail.MailAdress.User);
			Console.WriteLine("Cookies:");

			foreach (Cookie cookie in jmail.Cookies)
				Console.WriteLine(cookie.Name + "   " + cookie.Value);

			Console.Write("\n\nPress any key!");
			Console.ReadKey();
			Console.WriteLine("\n\n");

			
			jmail.UpdateInbox();
			Console.WriteLine("UpdateInbox Ok");

			Console.WriteLine("Count: " + jmail.Mails.Count + "\n\n");

			foreach (Mail mail in jmail.Mails) {
				Console.WriteLine("\t\tId: " + mail.Id);
				Console.WriteLine("\t\tDate: " + mail.Date);
				Console.WriteLine("\t\tSenderAdress: " + mail.SenderAdress.Address);
				Console.WriteLine("\t\tSenderAdress.DisplayName: " + mail.SenderAdress.DisplayName);
				Console.WriteLine("\t\tSenderAdress.Host: " + mail.SenderAdress.Host);
				Console.WriteLine("\t\tSenderAdress.User: " + mail.SenderAdress.User);
				Console.WriteLine("\t\tMyEmail: " + mail.MyAdress);
				Console.WriteLine("\t\tTopic: " + mail.Topic);
				
				mail.GetContent();
				Console.WriteLine("\t\tGetContent Ok");
				Console.WriteLine("\t\tContent length: " + mail.Content.Length+"\n\n\n");
			}

			Console.Write("\nPress any key!");
			Console.ReadKey();
			Console.WriteLine("\n\n");

			foreach (Mail mail in jmail.Mails) {
				Console.Write($"Delete({mail.Id.ToString()}) ");

				mail.Delete();
				Console.Write("Ok");

				Console.WriteLine("  ->  " + mail.Deleted);
			}


			jmail.UpdateInbox();
			Console.WriteLine("UpdateInbox Ok");

			Console.WriteLine("Count: " + jmail.Mails.Count);
		}

		public static void OvhUpdateTest() {
			MailSites.Uu2Ovh jmail = new MailSites.Uu2Ovh();
			
			jmail.SetMail();
			Console.WriteLine("SetMail Ok");

			Console.WriteLine("MailAdress: " + jmail.MailAdress);
			Console.WriteLine("MailAdress.DisplayName: " + jmail.MailAdress.DisplayName);
			Console.WriteLine("MailAdress.Host: " + jmail.MailAdress.Host);
			Console.WriteLine("MailAdress.User: " + jmail.MailAdress.User);
			Console.WriteLine("Cookies: " + jmail.Cookies.Count);

			foreach (Cookie cookie in jmail.Cookies)
				Console.WriteLine(cookie.Name + "   " + cookie.Value);


			for (; ; ) {

				jmail.UpdateInbox();
				Console.WriteLine("UpdateInbox Ok");

				Console.WriteLine("Count: " + jmail.Mails.Count + "\n\n");

				foreach (Mail mail in jmail.Mails) {
					Console.WriteLine("\t\tId: " + mail.Id);
					Console.WriteLine("\t\tDate: " + mail.Date);
					Console.WriteLine("\t\tSenderAdress: " + mail.SenderAdress.Address);
					Console.WriteLine("\t\tSenderAdress.DisplayName: " + mail.SenderAdress.DisplayName);
					Console.WriteLine("\t\tSenderAdress.Host: " + mail.SenderAdress.Host);
					Console.WriteLine("\t\tSenderAdress.User: " + mail.SenderAdress.User);
					Console.WriteLine("\t\tMyEmail: " + mail.MyAdress);
					Console.WriteLine("\t\tTopic: " + mail.Topic);

					mail.GetContent();
					Console.WriteLine("\t\tGetContent Ok");
					Console.WriteLine("\t\tContent length: " + mail.Content.Length + "\n\n\n");
				}

				Console.Write("\nPress any key!");
				Console.ReadKey();
				Console.WriteLine("\n\n");

			}
		}

	}
}