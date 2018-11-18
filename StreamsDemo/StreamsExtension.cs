using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StreamsDemo
{
    using System.Data;
    using System.Linq;
    using System.Security.AccessControl;

    // C# 6.0 in a Nutshell. Joseph Albahari, Ben Albahari. O'Reilly Media. 2015
    // Chapter 15: Streams and I/O
    // Chapter 6: Framework Fundamentals - Text Encodings and Unicode
    // https://msdn.microsoft.com/ru-ru/library/system.text.encoding(v=vs.110).aspx

    public static class StreamsExtension
    {

        #region Public members

        #region TODO: Implement by byte copy logic using class FileStream as a backing store stream .

        public static int ByByteCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);
            
            using (FileStream source = File.OpenRead(sourcePath),
                              destination = File.OpenWrite(destinationPath))
            {
                int count = 0;
                while (source.Position < source.Length)
                {
                    destination.WriteByte((byte)source.ReadByte());
                    count++;
                }

                return count;
            }
        }

        #endregion

        #region TODO: Implement by byte copy logic using class MemoryStream as a backing store stream.

        public static int InMemoryByByteCopy(string sourcePath, string destinationPath)
        {
            // TODO: step 1. Use StreamReader to read entire file in string

            // TODO: step 2. Create byte array on base string content - use  System.Text.Encoding class

            // TODO: step 3. Use MemoryStream instance to read from byte array (from step 2)

            // TODO: step 4. Use MemoryStream instance (from step 3) to write it content in new byte array

            // TODO: step 5. Use Encoding class instance (from step 2) to create char array on byte array content

            // TODO: step 6. Use StreamWriter here to write char array content in new file

            InputValidation(sourcePath, destinationPath);

            const int MEMORY_STREAM_CAPASITY = 1024;

            using (StreamReader source = new StreamReader(File.OpenRead(sourcePath)))
            using (MemoryStream mStream = new MemoryStream(MEMORY_STREAM_CAPASITY))
            using (StreamWriter destination = new StreamWriter(File.OpenWrite(destinationPath)))
            {
                string text = source.ReadToEnd();

                byte[] sourceTextBytes = Encoding.Default.GetBytes(text);

                foreach (var b in sourceTextBytes)
                {
                    mStream.WriteByte(b);
                }

                mStream.Seek(0, SeekOrigin.Begin);

                byte[] destinationTextBytes = new byte[mStream.Length];
                int countOfBytes = 0;
                while (countOfBytes < mStream.Length)
                {
                    destinationTextBytes[countOfBytes++] = (byte)mStream.ReadByte();
                }

                char[] charArray = Encoding.Default.GetChars(destinationTextBytes);

                destination.Write(charArray);

                return countOfBytes;
            }
        }

        #endregion

        #region TODO: Implement by block copy logic using FileStream buffer.

        public static int ByBlockCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int BLOCK_SIZE = 1024;
            const int BUFFER_SIZE = 5000;

            using (FileStream source = File.OpenRead(sourcePath),
                              destination = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.Read, BUFFER_SIZE))
            {
                byte[] block = new byte[BLOCK_SIZE];

                while (source.Read(block, 0, block.Length) > 0)
                {
                    destination.Write(block, 0, block.Length);
                }
                destination.Flush();
               
                return (int)destination.Length;
            }
        }

        #endregion

        #region TODO: Implement by block copy logic using MemoryStream.

        public static int InMemoryByBlockCopy(string sourcePath, string destinationPath)
        {
            // TODO: Use InMemoryByByteCopy method's approach

            const int MEMORY_STREAM_CAPASITY = 1024;

            InputValidation(sourcePath, destinationPath);
            
            using (StreamReader source = new StreamReader(File.OpenRead(sourcePath)))
            using (MemoryStream mStream = new MemoryStream(MEMORY_STREAM_CAPASITY))
            using (StreamWriter destination = new StreamWriter(File.OpenWrite(destinationPath)))
            {
                string text = source.ReadToEnd();
                byte[] sourceTextBytes = Encoding.Unicode.GetBytes(text);

                mStream.Write(sourceTextBytes, 0, sourceTextBytes.Length);
                mStream.Seek(0, SeekOrigin.Begin);

                byte[] destinationTextBytes = new byte[mStream.Length];
                mStream.Read(destinationTextBytes, 0, (int)mStream.Length);

                char[] charArray = Encoding.Unicode.GetChars(destinationTextBytes);
                destination.Write(charArray);

                return (int)mStream.Length;
            }
        }

        #endregion

        #region TODO: Implement by block copy logic using class-decorator BufferedStream.

        public static int BufferedCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            const int BLOCK_SIZE = 1024;
            const int BUFFER_SIZE = 5000;

            using (FileStream source = File.OpenRead(sourcePath),
                              destionation = File.OpenWrite(destinationPath))
            using (BufferedStream bufferedDestination = new BufferedStream(destionation, BUFFER_SIZE))
            {
                byte[] block = new byte[BLOCK_SIZE];
                int count = 0;

                while (source.Read(block, 0, block.Length) > 0)
                {
                    bufferedDestination.Write(block, 0, block.Length);
                    count += block.Length;
                }

                bufferedDestination.Flush();

                return count;
            }
        }

        #endregion

        #region TODO: Implement by line copy logic using FileStream and classes text-adapters StreamReader/StreamWriter

        public static int ByLineCopy(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            using (StreamReader source = new StreamReader(File.OpenRead(sourcePath)))
            using (StreamWriter destination = new StreamWriter(File.OpenWrite(destinationPath)))
            {
                string line;
                int count = 0;

                while (!string.IsNullOrEmpty(line = source.ReadLine()))
                {
                    destination.WriteLine(line);
                    count++;
                }

                return count;
            }
        }

        #endregion

        #region TODO: Implement content comparison logic of two files 

        public static bool IsContentEquals(string sourcePath, string destinationPath)
        {
            InputValidation(sourcePath, destinationPath);

            if (new FileInfo(sourcePath).Length != new FileInfo(destinationPath).Length)
                return false;

            using (FileStream source = File.OpenRead(sourcePath),
                              destination = File.OpenRead(destinationPath))
            {
                byte[] blockA = new byte[sizeof(long)];
                byte[] blockB = new byte[sizeof(long)];

                while (source.Read(blockA, 0, blockA.Length) > 0)
                {
                    destination.Read(blockB, 0, blockB.Length);

                    if (BitConverter.ToInt64(blockA, 0) != BitConverter.ToInt64(blockB, 0))
                        return false;
                }

                return true;
            }
        }

        #endregion

        #endregion

        #region Private members

        #region TODO: Implement validation logic

        private static void InputValidation(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentNullException(nameof(sourcePath));

            if (string.IsNullOrEmpty(destinationPath))
                throw new ArgumentNullException(nameof(destinationPath));
            
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException(sourcePath);

            var fi = new FileInfo(destinationPath);

            if(fi.Exists && fi.IsReadOnly)
                throw new ReadOnlyException(destinationPath); //???
        }

        #endregion

        #endregion

    }
}
