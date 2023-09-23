using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aria
{
	public interface IAriaLogger
	{
		void Trace(string message);
		void Debug(string message);
		void Info(string message);
		void Warning(string message);
		void Error(string message);
		void Error(object sender, Exception exception);
		void Fatal(string message);
		void Fatal(object sender, Exception exception);
	}
}
