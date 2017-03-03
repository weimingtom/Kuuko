package KopiLua;

import java.io.*;

public class StreamProxy {
	private final static int TYPE_FILE = 0;
	private final static int TYPE_STDOUT = 1;
	private final static int TYPE_STDIN = 2;
	private final static int TYPE_STDERR = 3;
	public int type = TYPE_FILE;
	public boolean isOK = false;

	private StreamProxy() {
		this.isOK = false;
	}

	public StreamProxy(String path, String modeStr) {
		
	}

	public final void Flush() {
		
	}

	public final void Close() {
		
	}

	public final void Write(byte[] buffer, int offset, int count) {
		if (this.type == TYPE_STDOUT) {
			System.out.print(new String(buffer, offset, count));
		} else if (this.type == TYPE_STDERR) {
			System.err.print(new String(buffer, offset, count));
		} else {
			//FIXME:TODO
		}
	}

	public final int Read(byte[] buffer, int offset, int count) {
		return 0;
	}

	public final int Seek(long offset, int origin) {
		return 0;
	}

	public final int ReadByte() {
		if (type == TYPE_STDIN) {
			try {
				return System.in.read();
			} catch (IOException e) {
				e.printStackTrace();
			}
			return 0;
		} else {
			return 0;
		}
	}

	public final void ungetc(int c) {

	}

	public final long getPosition() {
		return 0;
	}

	public final boolean isEof() {
		return true;
	}

	//--------------------------------------

	public static StreamProxy tmpfile() {
		StreamProxy result = new StreamProxy();
		return result;
	}

	public static StreamProxy OpenStandardOutput() {
		StreamProxy result = new StreamProxy();
		result.type = TYPE_STDOUT;
		result.isOK = true;
		return result;
	}

	public static StreamProxy OpenStandardInput() {
		StreamProxy result = new StreamProxy();
		result.type = TYPE_STDIN;
		result.isOK = true;
		return result;
	}

	public static StreamProxy OpenStandardError() {
		StreamProxy result = new StreamProxy();
		result.type = TYPE_STDERR;
		result.isOK = true;
		return result;
	}

	public static String GetCurrentDirectory() {
		File directory = new File("");
		return directory.getAbsolutePath();
	}

	public static void Delete(String path) {
		
	}

	public static void Move(String path1, String path2) {
		
	}

	public static String GetTempFileName() {
		try {
			return File.createTempFile("abc", ".tmp").getAbsolutePath();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return null;
	}

	public static String ReadLine() {
		BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
		try {
			return in.readLine();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return null;
	}

	public static void Write(String str) {
		System.out.print(str);
	}

	public static void WriteLine() {
		System.out.println();
	}

	public static void ErrorWrite(String str) {
		System.err.print(str);
		System.err.flush();
	}
}
