using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text ;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Example
{
	[TestFixture]
	class Case1Tests
	{
		[Test]
		public void TestThreads()
		{
			var ll = new List<long>();
			var t1 = new Thread(() =>
			{
				try
				{
					long i = 0;
					while (true)
					{
						ll.Add(++i * 2);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					Environment.Exit(0);
				}
				
			});
			t1.Start();

			var t2 = new Thread(() =>
			{
				try
				{
					long i = 0;
					while (true)
					{
						ll.Add(++i * 2 + 1);
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					Environment.Exit(0);
				}
				
			});
			t2.Start();

			Thread.Sleep(TimeSpan.FromSeconds(100));
			t1.Abort();
			t2.Abort();
			Assert.Fail("No concurrenct erros detected, though expected");
		}
		
	}
}
