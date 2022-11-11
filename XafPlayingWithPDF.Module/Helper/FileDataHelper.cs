using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafPlayingWithPDF.Module.Helper
{
    public static class FileDataHelper
    {

        public static FileData FromByteToFileData(Byte[] file, string fileName, UnitOfWork uow)
        {
            if (file == null)
                return null;

            try
            {
                Stream stream = new MemoryStream(file);

                FileData fd = new FileData(uow);

                fd.LoadFromStream(fileName, stream);

                return fd;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static Stream ConvertFileToStream(string filePath)
        {
            try
            {
                return new FileStream(filePath, FileMode.Open);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
        {
            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();
            }

            string path = Path.Combine(filePath, fileName);
            using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }
        }


        public static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

    }
}
