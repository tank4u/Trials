using Hl7Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace RxDotNet
{
	class Program
	{
		static string sourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Watched"); //@"E:\watched\";
		static string processedFolder = Path.Combine(sourcePath, "processed");
		static string failureFolder = Path.Combine(sourcePath, "failed");

		static void Main(string[] args)
		{
			//First();
			//Second();
			//Notifies subscriber when anything gets added to the subject
			SubjectSubscription();

			SubscribeToDirectory();

			Console.Read();
		}

		static void SetupFolders()
		{
			if (!Directory.Exists(sourcePath))
			{
				Directory.CreateDirectory(sourcePath);
			}
			if (!Directory.Exists(processedFolder))
			{
				Directory.CreateDirectory(processedFolder);
			}
			if (!Directory.Exists(failureFolder))
			{
				Directory.CreateDirectory(failureFolder);
			}
		}

		static void SetupObservables()
		{

		}

		static void SubscribeToDirectory()
		{
			//ensure Processed, Failed folders are present
			SetupFolders();

			var watcher = new System.IO.FileSystemWatcher(sourcePath);

			watcher.Filter = "*.txt";

			//watcher.InternalBufferSize = 16384; //16 KB, 32768, max - 64 KB			

			var createdFiles =
					   Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
						  dlgt => watcher.Created += dlgt,
						  dlgt => watcher.Created -= dlgt, NewThreadScheduler.Default);

			var schedulerTask = Observable.Timer(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));



			//var renamedFiles =
			//		   Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
			//			  dlgt => watcher.Renamed += dlgt,
			//			  dlgt => watcher.Renamed -= dlgt);


			//evt => ProcessFile(evt.Sender, evt.EventArgs)
			createdFiles.Subscribe(
				evt =>
				{
					Console.WriteLine("Processing file:{0}, change type: {1}", evt.EventArgs.FullPath, evt.EventArgs.ChangeType);

					new DefaultParser().Process(evt.EventArgs.FullPath);
					//MoveToProcessed(evt.EventArgs.FullPath);
				});

			//renamedFiles.Subscribe(
			//	evt => ProcessFile(evt.EventArgs.FullPath, evt.EventArgs.ChangeType));

			//Begin watcher
			watcher.EnableRaisingEvents = true;

			schedulerTask.Subscribe(
				(value) =>
				{
					Console.WriteLine("Check for remaining files, need to know if fileWatcher is completed or not");
				});

			//Process file

			//Move to processed/failed

			Console.WriteLine("Watching : {0}", sourcePath);
		}

		private void ProcessFile(string filePath, WatcherChangeTypes changeType)
		//private void ProcessFile(object source, FileSystemEventArgs args)
		{
			Console.WriteLine("file:{0}, change type: {1}", filePath, changeType);

			MoveToProcessed(filePath);
		}


		#region Helpers

		static void MoveToProcessed(string filePath)
		{
			try
			{
				Console.WriteLine("Processing {0} on thread {1}", filePath, Thread.CurrentThread.ManagedThreadId);
				WaitReady(filePath);
				var destinationFilePath = Path.Combine(processedFolder,
					string.Format("{0}-{1}.{2}", Path.GetFileNameWithoutExtension(filePath), DateTime.UtcNow.Ticks.ToString(), "txt"));
				Console.WriteLine("Destination path: {0}", destinationFilePath);
				if (File.Exists(filePath))
				{
					File.Move(filePath, destinationFilePath);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Failed to process: {0} \n {1}", filePath, ex.StackTrace);
			}
		}

		static void MoveToFailed(string filePath)
		{
			try
			{
				Console.WriteLine("Moving failed {0} on thread {1}", filePath, Thread.CurrentThread.ManagedThreadId);
				//WaitReady(filePath);
				var destinationFilePath = Path.Combine(failureFolder,
					string.Format("{0}-{1}.{2}", Path.GetFileNameWithoutExtension(filePath), DateTime.UtcNow.Ticks.ToString(), "txt"));
				Console.WriteLine("Destination path: {0}", destinationFilePath);
				if (File.Exists(filePath))
				{
					File.Move(filePath, destinationFilePath);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.TraceError("Failed to process: {0} \n {1}", filePath, ex.StackTrace);
			}
		}

		static void WaitReady(string fileName)
		{
			var retries = 0;
			while (retries < 3)
			{
				try
				{
					using (Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
					{
						if (stream != null)
						{
							System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} ready.", fileName));
							break;
						}
					}
				}
				catch (FileNotFoundException ex)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
				}
				catch (IOException ex)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
				}
				catch (UnauthorizedAccessException ex)
				{
					System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
				}
				Thread.Sleep(500);
				retries++;
			}

			if (retries == 3)
			{
				MoveToFailed(fileName);
			}
		}

		#endregion

		//notifies subscriber 
		static void SubjectSubscription()
		{
			var source = new Subject<string>();

			source.Subscribe((i) => Console.WriteLine(i));

			source.OnNext("Hello");
			for (int i = 0; i <= 5; i++)
			{
				source.OnNext(i.ToString());
			}
			source.OnNext("Bye!!");
		}

		//static list to observable
		static void Second()
		{
			var myList = new List<int>();
			var source = myList.ToObservable();
			//no values
			Subscribe(source);
			for (int i = 0; i <= 5; i++)
			{
				myList.Add(i);
			}
			//displays value
			Subscribe(source);
		}

		static void Subscribe(IObservable<int> source)
		{
			source.Subscribe((i) => Console.WriteLine(i));
		}

		static void First()
		{
			var source = Observable.Range(1, 10);

			source.Subscribe((i) => Console.WriteLine(i));
		}
	}
}
