using System;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace ProjectManager.Building
{
	public class ProcessRunner
	{
		public static bool Run(string fileName, string arguments, bool ignoreExitCode, bool mergeErrors)
		{
			//if (!File.Exists(fileName))
			//	throw new FileNotFoundException("The program '"+fileName+"' was not found.",fileName);

			Process process = new Process();
            process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.Default;
            process.StartInfo.StandardErrorEncoding = Encoding.Default;
            process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = Environment.ExpandEnvironmentVariables(fileName);
			process.StartInfo.Arguments = Environment.ExpandEnvironmentVariables(arguments);
			process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
			process.Start();

			// capture output in a separate thread
			LineFilter stdoutFilter = new LineFilter(process.StandardOutput, Console.Out, false);
			LineFilter stderrFilter = new LineFilter(process.StandardError, Console.Error, mergeErrors);

			Thread outThread = new Thread(new ThreadStart(stdoutFilter.Filter));
			Thread errThread = new Thread(new ThreadStart(stderrFilter.Filter));

			outThread.Start();
			errThread.Start();

			process.WaitForExit();

			outThread.Join(1000);
			errThread.Join(1000);
			
			return (ignoreExitCode) ? stderrFilter.Lines == 0 : process.ExitCode == 0;
		}
	}

	class LineFilter
	{
        static Regex reSplitError = new Regex(@"\.[a-z]+:[0-9]+$");
		TextReader reader;
        TextWriter writer;
        bool mergeErrors;
        string unsplit = null;
		public int Lines;

		public LineFilter(TextReader reader, TextWriter writer, bool mergeErrors)
		{
			this.reader = reader;
			this.writer = writer;
            this.mergeErrors = mergeErrors;
		}

		public void Filter()
		{
			while (true)
			{
				string line = reader.ReadLine();
				if (line == null) break;

                if (mergeErrors)
                {
                    if (unsplit != null) { line = unsplit + ":" + line; unsplit = null; }
                    else if (reSplitError.IsMatch(line)) { unsplit = line; continue; }
                }
				writer.WriteLine(line);
				writer.Flush();
				
				if (line.Length > 0) Lines++;
			}
		}
	}
}