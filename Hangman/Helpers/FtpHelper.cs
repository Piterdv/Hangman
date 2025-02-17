using Hangman.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using WinSCP;

namespace Hangman.Helpers;

public class FtpHelper
{
    List<MyFile> _files = new List<MyFile>();
    private static string _ftpServer = "";
    private static string _ftpUser = "";
    private static string _ftpPassword = "";
    private static string _ftpPath = "";
    private SessionOptions _sessionOptions;
    private static string _scpExePath = Directory.GetCurrentDirectory() + "\\WinSCP\\WinSCP.exe";

    public FtpHelper(string ftpServer, string ftpUser, string ftpPassword, string ftpPath)
    {
        _ftpServer = ftpServer;
        _ftpUser = ftpUser;
        _ftpPassword = ftpPassword;
        _ftpPath = ftpPath;
        _sessionOptions = new SessionOptions
        {
            Protocol = Protocol.Ftp,
            HostName = _ftpServer,
            UserName = _ftpUser,
            Password = _ftpPassword,
        };
    }

    public List<MyFile> GetListOfFiles()
    {

        _files.Clear();

        File.Create(_scpExePath + "eeee").Close();

        try
        {
            using (Session session = new Session())
            {
                session.ExecutablePath = _scpExePath;

                session.Open(_sessionOptions);

                RemoteDirectoryInfo directory = session.ListDirectory(_ftpPath);

                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    if (fileInfo.IsDirectory)
                    {
                        continue;
                    }

                    _files.Add(MapToMyFile(fileInfo));
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        return _files;
    }

    private static MyFile MapToMyFile(RemoteFileInfo fileInfo)
    {
        return new MyFile
        {
            Name = fileInfo.Name,
            Path = fileInfo.FullName,
            LastModified = fileInfo.LastWriteTime.ToString(),
            Size = fileInfo.Length.ToString(),
            Type = fileInfo.FileType.ToString(),
            IsReadOnly = fileInfo.FilePermissions.UserWrite,
            Permissions = fileInfo.FilePermissions.ToString(),
            MyFileOrDir = fileInfo.IsDirectory.ToString(),
        };
    }

    public bool UploadAllChoosenFileToFtp(List<string> path)
    {
        bool result = true;

        try
        {
            using (Session session = new Session())
            {
                session.ExecutablePath = _scpExePath;

                session.Open(_sessionOptions);

                TransferOptions transferOptions = new TransferOptions();

                foreach (var item in path)
                {
                    var res = session.PutFiles(item, "/" + _ftpPath + "/");

                    if (!res.IsSuccess)
                    {
                        result = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            var a = ex.Message;
        }

        return result;
    }

    public bool DownloadFileFromFtp(string path, string fileName)
    {
        bool result = true;

        try
        {
            using (Session session = new Session())
            {
                session.ExecutablePath = _scpExePath;

                session.Open(_sessionOptions);

                TransferOptions transferOptions = new TransferOptions();

                var res = session.GetFiles(path, fileName);

                if (!res.IsSuccess)
                {
                    result = false;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        return result;
    }
}
