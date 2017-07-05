using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Hl7Parser
{
	public class DefaultParser
	{
		string processedFolder = "processed";
		string failureFolder = "failed";		
		string destinationFileName = "";

		public void Process(string filePath)
		{
			var sourcePath = Path.GetDirectoryName(filePath);					
			processedFolder = Path.Combine(sourcePath, processedFolder);
			failureFolder = Path.Combine(sourcePath, failureFolder);

			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			var fileExtension = Path.GetExtension(filePath);
			destinationFileName = string.Format("{0}-{1}{2}", fileNameWithoutExtension, DateTime.UtcNow.Ticks.ToString(), fileExtension);

			MoveToProcessed(filePath);
		}

		private void MoveToProcessed(string filePath)
		{
			try
			{
				Console.WriteLine("Processing {0} on thread {1}", filePath, Thread.CurrentThread.ManagedThreadId);
				WaitReady(filePath);
				var destinationFilePath = Path.Combine(processedFolder, destinationFileName);
				//Console.WriteLine("Destination path: {0}", destinationFilePath);
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

		private void MoveToFailed(string filePath)
		{
			try
			{
				Console.WriteLine("Moving failed {0} on thread {1}", filePath, Thread.CurrentThread.ManagedThreadId);
				//WaitReady(filePath);
				var destinationFilePath = Path.Combine(failureFolder, destinationFileName);
				//Console.WriteLine("Destination path: {0}", destinationFilePath);
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

		private void WaitReady(string fileName)
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
	}
}
