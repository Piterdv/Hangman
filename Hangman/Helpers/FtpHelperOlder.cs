using Hangman.Enums;
using Hangman.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Hangman.Helpers
{
    public class FtpHelperOlder
    {
        List<MyFile> _files = new List<MyFile>();
        private string _ftpServer = "";
        private string _ftpUser = "";
        private string _ftpPassword = "";
        private string _ftpPath = "";

        public FtpHelperOlder(string ftpServer, string ftpUser, string ftpPassword, string ftpPath)
        {
            _ftpServer = ftpServer;
            _ftpUser = ftpUser;
            _ftpPassword = ftpPassword;
            _ftpPath = ftpPath;
        }

        //TODO: it's obsolete - use FluentFTP and System.Net.FtpClient - TO NET, inne płatne więc bez przesady:)?
        public async Task<List<MyFile>> GetListOfFilesAsync()
        {
            _files.Clear();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpServer + _ftpPath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(_ftpUser, _ftpPassword);

            FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[3];
                string permissions = tokens[2];
                string fileOrDir = tokens[0][0].ToString();
                string size = tokens[1];
                string date = tokens[0].Substring(1, tokens[0].Length - 1) + " " + tokens[4] + " " + tokens[5];

                _files.Add(new MyFile
                {
                    Name = name,
                    Permissions = permissions,
                    MyFileOrDir = fileOrDir,
                    Size = size,
                    Date = date
                });

                line = reader.ReadLine();
            }

            reader.Close();
            response.Close();

            return _files;
        }

        //TODO: it's obsolete - use FluentFTP and System.Net.FtpClient?
        public List<MyFile> GetListOfFiles()
        {
            _files.Clear();

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpServer + _ftpPath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(_ftpUser, _ftpPassword);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string name = GetFileName(tokens);
                if (name.Length <= 2)
                {
                    line = reader.ReadLine();
                    continue;
                }
                string permissions = tokens[2];
                string fileOrDir = tokens[0][0].ToString();
                string size = tokens[4];
                string date = GetDate(tokens);//tokens[0].Substring(1, tokens[0].Length - 1) + " " + tokens[4] + " " + tokens[5];

                _files.Add(new MyFile
                {
                    Name = name,
                    Permissions = permissions,
                    MyFileOrDir = fileOrDir,
                    Size = size,
                    Date = date
                });

                line = reader.ReadLine();
            }

            reader.Close();
            response.Close();

            return _files;
        }

        //OK, o kant... - przejdę na fluentFTP:)
        private static string GetDate(string[] tokens)
        {
            DateTime date = DateTime.MinValue;
            int year = int.Parse(tokens[4]) + 1421; //o ile mam rację - to jest wielkość pliku:)
            int mt = (int)(AllMonthsIn3signs)Enum.Parse(typeof(AllMonthsIn3signs), tokens[5]);
            DateTime.TryParse(tokens[5] + " " + tokens[6] + " " + tokens[7], out date);
            return date.AddHours(-3).ToString();
        }

        private static string GetFileName(string[] tokens)
        {
            string n = "";

            for (int i = 8; i < tokens.Length; i++)
            {
                n += tokens[i] + " ";
            }

            return n.TrimEnd(' ');
        }

        public bool SendFile(string fullPath, string date)
        {
            string MyFileName = Path.GetFileName(fullPath);
            string dirPath = Path.GetDirectoryName(fullPath);

            if (!FileExistsAndIsNewer(MyFileName, date))
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                //TODO: BAD - za każdym wywołaniem nowy obiekt WebClient?
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(_ftpUser, _ftpPassword);
                    client.UploadFile(_ftpServer + _ftpPath + MyFileName, fullPath);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //public Task<bool> FileExistsAndIsNewer(string MyFileName, string date)
        //{
        //    var files = await GetListOfFilesAsync();

        //    if (files.Where(x => x.Name == MyFileName).Count() > 0)
        //    {
        //        var file = files.Where(x => x.Name == MyFileName).First();
        //        if ( DateTime.Parse(file.Date) <= DateTime.Parse(date))
        //            return false;
        //        else
        //            return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //check if file exists and is newer in _files then in local directory
        public bool FileExistsAndIsNewer(string MyFileName, string date)
        {
            if (_files.Where(x => x.Name == MyFileName).Count() > 0)
            {
                var file = _files.Where(x => x.Name == MyFileName).First();
                if (DateTime.Parse(file.Date) <= DateTime.Parse(date))
                    return false;
                else
                    return true;
            }
            else
            {
                return false;
            }
        }


    }
}
