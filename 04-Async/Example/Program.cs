using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var listener = new HttpListener();
			listener.Prefixes.Add(string.Format("http://+:10000/"));
			listener.Start();
			while (true)
			{
				var context = listener.GetContext();
				double start = 0;
				for (int i = 0; i < 1000000; i++)
				{
					start = Math.Cos(start);
				}
				var result = Encoding.UTF8.GetBytes("Hello from listener: " + start);
//				context.Response.ContentLength64 = result.Length;
				context.Response.OutputStream.Write(result, 0, result.Count());
				context.Response.Close();
			}
		}
	}
}
