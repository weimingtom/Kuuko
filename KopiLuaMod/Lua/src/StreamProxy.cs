using System;
using System.IO;

namespace KopiLua
{
    public class StreamProxy
    {
        public bool isOK = false;
        public Stream file;

        private StreamProxy()
        {
            this.isOK = false;
        }

        public StreamProxy(string path, string modeStr)
        {
            isOK = true;
            FileMode filemode = FileMode.Open;
            FileAccess fileaccess = (FileAccess)0;
            char[] mode = modeStr.ToCharArray();
            for (int i = 0; i < mode.Length; i++)
            {
                switch (mode[i])
                {
                    case 'r':
                        {
                            fileaccess = fileaccess | FileAccess.Read;
                            if (!File.Exists(path))
                            {
                                isOK = false;
                            }
                            break;
                        }

                    case 'w':
                        {
                            filemode = FileMode.Create;
                            fileaccess = fileaccess | FileAccess.Write;
                            break;
                        }
                }
                if (isOK == false)
                {
                    break;
                }
            }
            if (isOK == true)
            {
                this.file = new FileStream(path, filemode, fileaccess);
            }
        }

        public void Flush()
        {
            if (this.isOK && this.file != null)
            {
                this.file.Flush();
            }
        }

        public void Close()
        {
            if (this.isOK && this.file != null)
            {
                this.file.Close();
            }
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (this.isOK && this.file != null)
            {
                this.file.Write(buffer, offset, count);
            }
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (this.isOK && this.file != null)
            {
                return this.file.Read(buffer, offset, count);
            }
            else
            {
                return 0;
            }
        }

        public int Seek(long offset, int origin)
        {
            try
			{
				this.file.Seek(offset, (SeekOrigin)origin);
				return 0;
			}
			catch
			{
				return 1;
			}
        }

        public int ReadByte()
        {
            if (this.isOK && this.file != null)
            {
                return this.file.ReadByte();
            }
            else
            {
                return 0;
            }
        }

        public void ungetc(int c)
        {
            if (this.file.Position > 0)
            {
                this.file.Seek(-1, SeekOrigin.Current);
            }
        }

        public long getPosition()
        {
            return this.file.Position;
        }

        public bool isEof()
        {
            return this.file.Position >= this.file.Length;
        }

        //--------------------------------------

        public static StreamProxy tmpfile()
        {
            StreamProxy result = new StreamProxy();
            result.file = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite);
            result.isOK = true;
            return result;
        }

        public static StreamProxy OpenStandardOutput()
        {
            StreamProxy result = new StreamProxy();
            result.file = Console.OpenStandardOutput();
            result.isOK = true;
            return result;
        }

        public static StreamProxy OpenStandardInput()
        {
            StreamProxy result = new StreamProxy();
            result.file = Console.OpenStandardInput();
            result.isOK = true;
            return result;
        }


        public static StreamProxy OpenStandardError()
        {
            StreamProxy result = new StreamProxy();
            result.file = Console.OpenStandardError();
            result.isOK = true;
            return result;
        }

        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public static void Delete(string path)
        {
            File.Delete(path);
        }

        public static void Move(string path1, string path2)
        {
            File.Move(path1, path2);
        }

        public static string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        public static void Write(string str)
        {
            Console.Write(str);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static void ErrorWrite(string str)
        {
            Console.Error.Write(str);
            Console.Error.Flush();
        }
    }
}

//FIXME: LuaIOLib.cs:
//			else
//			{
//				LuaAPI.lua_pushfstring(L, CharPtr.toCharPtr("file (%p)"), f);
//			}

