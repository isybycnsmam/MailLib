﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailLib {
	public static class MailSites {

		public sealed class JmailOvh : BaseClasses.Ovh { public JmailOvh() : base(BaseClasses.Ovh.DomainName.Jmail) { } }

		public sealed class NextOvh : BaseClasses.Ovh { public NextOvh() : base(BaseClasses.Ovh.DomainName.Next) { } }

		public sealed class Uu2Ovh : BaseClasses.Ovh { public Uu2Ovh() : base(BaseClasses.Ovh.DomainName.Uu2) { } }

	}
}