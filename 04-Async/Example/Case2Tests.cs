using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Example
{
	class Case2Tests
	{
		[Test]
		public void TestWaitAllTasks()
		{
			var tasks = new Task[10];
			for (int i = 0; i < tasks.Length; i++)
			{
				var num = i;
				var firstTask = new Task(() =>
				{
					Console.WriteLine("{0}\tTask {1} starting...",
						DateTime.Now.ToString("s"), num);
					Thread.SpinWait(200000000);
					Console.WriteLine("{0}\tTask {1} finishing...",
						DateTime.Now.ToString("s"), num);
				});
				tasks[i] = firstTask;
				firstTask.Start();
			}

			Task.WaitAll(tasks);
			Console.WriteLine("All tasks finished");
		}

		[Test]
		public void TestParent()
		{
			var parent = Task.Factory.StartNew(() =>
				{
					Console.WriteLine("Outer task executing.");

					Task.Factory.StartNew(
						() =>
							{
								Console.WriteLine("Nested task starting.");
								Thread.SpinWait(500000);
								Console.WriteLine("Nested task completing.");
							}, TaskCreationOptions.AttachedToParent);
				});

			parent.Wait();
			Console.WriteLine("Outer has completed.");
		}

		[Test]
		public void TestFuture()
		{
			Task<int> deepThoughtTask = Task.Run(() => 42);
			deepThoughtTask.Wait();
			Console.WriteLine("Answer to the Ultimate Question of Life, the Universe, and Everything is {0}", deepThoughtTask.Result);
		}

		[Test]
		public void TestContinueWith()
		{
			var tasksChain = Task.Run(() =>
				Console.WriteLine("Asking Deep Thought..."))
			.ContinueWith(previousTask => Thread.SpinWait(500000000))
			.ContinueWith(previousTask => Console.WriteLine("Processing is done!"))
			.ContinueWith(previousTask => 42)
			.ContinueWith(
				previousTask =>
				Console.WriteLine(
					"Answer to the Ultimate Question of Life, the Universe, and Everything is {0}",
					previousTask.Result));

			tasksChain.Wait();
		}

		[Test]
		public void TestExceptionSimple()
		{
			var task = Task.Factory.StartNew(() =>
			{
				throw new Exception("I failed, sorry!");
			});

			try
			{
				Thread.Sleep(3000);
				task.Wait();
			}
			catch (AggregateException ae)
			{
				foreach (var e in ae.InnerExceptions)
				{
					Console.WriteLine(e);
				}
			}
		}

		[Test]
		public void TestContinueWithException()
		{
			var tasksChain = Task.Run(() =>
				{
					throw new Exception("haha!");
					return 1;
				})
				.ContinueWith(previousTask =>
					{
						try
						{
							Console.WriteLine("I'm in continuation");
							Console.WriteLine(previousTask.Result);
						}
						catch(Exception e)
						{
							Console.WriteLine(e);
						}
					});

			tasksChain.Wait();
		}


		[Test]
		public void TestNestedAggregateException()
		{
			var task = Task.Run(() =>
			{
				Task.Run(() =>
				{
					throw new Exception("Nested failed");
				});
			});

			task.Wait();
			Console.WriteLine("Outer task completed");
		}


		[Test]
		public void TestChildAggregateException()
		{
			var task = Task.Factory.StartNew(() =>
			{
				Task.Factory.StartNew(() =>
				{
					Task.Factory.StartNew(() =>
					{
						throw new Exception("Attached child2 faulted.");
					}, TaskCreationOptions.AttachedToParent);

					throw new Exception("Attached child1 faulted.");
				}, TaskCreationOptions.AttachedToParent);
				throw new Exception("parent failed");
			});

			try
			{
				task.Wait();
			}
			catch(AggregateException ae)
			{
//				Console.WriteLine(task.Exception);
				// ae.Flatten().Handle((ex) => ex is MyCustomException);
				foreach(var e in ae.Flatten().InnerExceptions)
					Console.WriteLine(e);
			}
		}




		
	}
}
