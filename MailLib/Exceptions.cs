using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MailLib {
	public sealed class MailInvalidLengthOrFormat	: Exception { }
	public sealed class MailIsAlreadyTaken		: Exception { }
	public sealed class ParsingException 		: Exception { public ParsingException(string msg, Exception inner) : base(msg, inner) { } }
}