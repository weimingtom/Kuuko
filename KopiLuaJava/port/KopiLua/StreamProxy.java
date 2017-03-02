package KopiLua;

import java.io.*;

public class StreamProxy {
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
		
	}

	public final int Read(byte[] buffer, int offset, int count) {
		return 0;
	}

	public final int Seek(long offset, int origin) {
		return 0;
	}

	public final int ReadByte() {
		return 0;
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
		return result;
	}

	public static StreamProxy OpenStandardInput() {
		StreamProxy result = new StreamProxy();
		return result;
	}

	public static StreamProxy OpenStandardError() {
		StreamProxy result = new StreamProxy();
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
