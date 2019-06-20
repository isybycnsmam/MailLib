using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Net.Mail;

namespace MailLib.BaseClasses {
	public class Account {

		protected HttpClientHandler handler = new HttpClientHandler();
		protected HttpClient client;

		protected string Domain;
		public MailAddress MailAdress { get; protected set; }

		public List<Mail> Mails { get; protected set; }

		public CookieCollection Cookies => GetAllCookies(handler.CookieContainer);
		protected CookieCollection GetAllCookies(CookieContainer container) {
			var allCookies = new CookieCollection();
			var domainTableField = container.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
			var domains = (IDictionary)domainTableField.GetValue(container);

			foreach (var val in domains.Values) {
				var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
				var values = (IDictionary)type.GetValue(val);
				foreach (CookieCollection cookies in values.Values) {
					allCookies.Add(cookies);
				}
			}
			return allCookies;
		}

	}
}