import java.lang.*;
import java.io.*;
import java.security.*;

//
// Credit for the security manager code:
//
// Java Notes by Stuart D. Gathman
// http://www.bmsi.com/java/
//


/**
* This Exception is raised to cancel the compiler's System.Exit() call
*/
class ExitSecurityException extends SecurityException
{
	public int exitcode;
	public ExitSecurityException(int c) { exitcode = c; }
}

/**
* We run the flex 2 compiler in security-managed mode
*/
class SystemSecurityManager extends java.lang.SecurityManager
{
	PrintStream err;
	SystemSecurityManager(PrintStream e) { err = e; }
	public void checkAccept(String host, int port) { return; }
	public void checkAccess(Thread g) { return; }
	public void checkConnect(String host, int port) {
		//err.println("Connecting to: " + host + ":" + port);
	}
	public void checkListen(int port) {
		//err.println("Listening on port: " + port);
	}
	public void checkCreateClassLoader() {
		//err.println("Create Class Loader");
	}
	public void checkLink(String lib) {
		//err.println("Loading library: " + lib);
	}
	public void checkPackageAccess(String pkg) {
		//err.println("Using package: " + pkg);
	}
	public void checkExec(String cmd) {
		//err.println("Exec: " + cmd);
	}
	public void checkPermission(Permission perm) { return; }
	public void checkPermission(Permission perm, Object context) { return; }
	public void checkRead(FileDescriptor fd) { return; }
	public void checkRead(String file) {
		//err.println("Reading file: '" + file + "'");
	}
	public void checkWrite(FileDescriptor fd) { return; }
	public void checkWrite(String file) {
		//err.println("Writing file: '" + file + "'");
	}
	public void checkPropertiesAccess() { return; }
	public void checkPropertyAccess(String key) { return; }
	public void checkSecurityAccess(String path) { return; }
	public void checkSetFactory() { return; }
	public void checkExit(int code) {
		throw new ExitSecurityException(code);
	}
}

/**
* Flex 2 compiler host
*/
class MxmlcShell
{
	static public void main(String[] args)
	{
		// switch to managed security
		SystemSecurityManager sm = new SystemSecurityManager(System.err);
      System.setSecurityManager(sm);

		try
		{
			BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
			while(true)
			{
				// get command line
				String cmd = in.readLine();
				if (cmd == null) break;
				try
				{
					// run compiler
					flex2.tools.Compiler.main(cmd.split(";"));
				}
				catch (ExitSecurityException e) {
					System.out.println("Done(" + e.exitcode + ")");
				}
				catch (SecurityException e) {
					System.err.println(e);
					System.out.println("Done(99)");
				}
				// force garbace collection, the compiler is heavy on memory
				System.gc();
			}
		}
		catch (Exception ex)
		{
			ex.printStackTrace();
		}
	}
}