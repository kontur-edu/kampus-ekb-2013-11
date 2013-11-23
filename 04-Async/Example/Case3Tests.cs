using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Example
{
	class Case3Tests
	{
		[Test]
		public async void TestAsyncPLINQ()
		{
			using (var ms = new MemoryStream())
			{
				Enumerable.Range(0, 1000000)
					.AsParallel()
					.Select(i => new { num = i, val = Math.Sin(i) })
					.Where(elem => elem.val > 0)
					.OrderBy(elem => elem.val)
					.ForAll(elem => 
						{
							var buf = Encoding.GetEncoding(1251).GetBytes(elem.num + "\t" + elem.val + "\r\n");
							lock(ms)
								ms.Write(buf, 0, buf.Length);
						});

				var semaphore = new Semaphore(0,1);
				using (FileStream fStream = new FileStream("sins.txt", FileMode.Create,
					FileAccess.ReadWrite, FileShare.None, 4096, true))
				{
					Console.WriteLine("File was {0}opened asynchronously", fStream.IsAsync ? "" : "NOT ");

					fStream.BeginWrite(ms.GetBuffer(), 0, (int)ms.Position,
						asyncResult =>
						{
							var fs = (FileStream)asyncResult.AsyncState;
							fs.EndWrite(asyncResult);
							Console.WriteLine("Done with I/O!");
							semaphore.Release();
						}, fStream);
					Console.WriteLine("Free for work! But nothing to do...");
					semaphore.WaitOne();

//					ms.Position = 0;
//					var copyToAsyncTask = ms.CopyToAsync(fStream);
//					Console.WriteLine("Free for work! But nothing to do...");
//					await copyToAsyncTask;
//					Console.WriteLine("Done with I/O!");
				}
			}
		}

		[Test]
		public async void AwaitTest()
		{
			var sw = new Stopwatch();
			sw.Start();
			var result = await Task.Run(async () =>
			{
				double cur = 1;
				for (int i = 0; i < 10000000; i++)
				{
					cur = Math.Cos(cur);
				}
				return await Task.Delay(1000).ContinueWith(_ => cur);
			});
			
			Console.WriteLine("Finished with {0} in {1}ms", result, sw.Elapsed.TotalMilliseconds);
		}

		[Test]
		public async void AwaitHttp()
		{
			try
			{
				using (var client = new HttpClient())
				{
					var stpw = new Stopwatch();
					stpw.Start();
					var task = client.GetStringAsync("http://cs.usu.edu.ru/langs/");
					Console.WriteLine("Issued request to remote host. Can do some processing");
					await task;
					Console.WriteLine("Done in {0}ms", stpw.Elapsed);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
