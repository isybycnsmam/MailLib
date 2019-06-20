using System;
using System.Net.Http;
using System.Net.Mail;
using MailLib.BaseClasses;

namespace MailLib {
	public class Mail {

		public int Id { get; set; }
		public MailAddress SenderAdress { get; set; }
		public MailAddress MyAdress { get; set; }
		public string Topic { get; set; }
		public DateTime Date { get; set; }
		public string Content { get; private set; }
		public bool Deleted { get; private set; } = false;

		private HttpClient client;

		public Mail(ref HttpClient client, RefAction<int> _GetContent, Action<int> _Delete) {
			this.client = client;
			this._GetContent = _GetContent;
			this._Delete = _Delete;
		}


		/// <exception cref="ParsingException">Parsing data from some page generates any null, empty or unexpected value, contains inner exception</exception>
		/// <exception cref="HttpRequestException">Error in sending requests, contains inner exception</exception>
		public void GetContent() { Content = _GetContent(Id); }
		private RefAction<int> _GetContent;


		/// <exception cref="HttpRequestException">Error in sending requests, contains inner exception</exception>
		public void Delete() { _Delete(Id); Deleted = true; }
		private Action<int> _Delete;

	}
}