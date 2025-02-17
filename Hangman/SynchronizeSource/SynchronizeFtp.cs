using Hangman.Helpers;
using Hangman.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hangman.SynchronizeSource;

class SynchronizeFtp : ISynchronizeResource
{
    public async Task<StateOfSynchronization> CheckChangesInSource(string dictionaryDirPath)
    {
        var ftpHelper = CreateFtpHelper();
        var ftpFiles = ftpHelper.GetListOfFiles();
        var localFiles = GetLocalFilesToUpdate(ftpFiles, dictionaryDirPath);
        var result = InitializeResult();
        var newestFiles = await DownloadNewFiles(ftpHelper, localFiles, dictionaryDirPath);

        if (newestFiles != null)
        {
            var filesToUpload = GetLocalFilesToUpload(ftpFiles, dictionaryDirPath, newestFiles);
            UpdateResultWithFilesStatus(filesToUpload, newestFiles, ref result);

            if (filesToUpload.Count == 0 && newestFiles.Count == 0)
            {
                return UpdateResultValue(Resource.InfoOnMsgBox, Resource.AllFilesAreUpToDate, ref result);
            }

            if (ftpHelper.UploadAllChoosenFileToFtp(filesToUpload))
            {
                UpdateResultValue(Resource.OKonMessageBox, Resource.AllFilesSynchronized, ref result);
            }
            else
            {
                UpdateResultValue(Resource.ErrorOnMsgBox, Resource.ErrorOnUpload, ref result);
            }
            return result;
        }

        return UpdateResultValue(Resource.ErrorOnMsgBox, Resource.ErrorOnDownload, ref result);
    }

    private static FtpHelper CreateFtpHelper()
    {
        return new FtpHelper(AppSettings.FTP_SERVER, AppSettings.FTP_USER, AppSettings.FTP_PASSWORD, AppSettings.FTP_PATH);
    }

    private static List<string> GetLocalFilesToUpdate(List<MyFile> ftpFiles, string dictionaryDirPath)
    {
        var localFiles = new List<string>();

        foreach (var file in ftpFiles)
        {
            if (!File.Exists(dictionaryDirPath + file.Name)
                || DateTime.Parse(file.LastModified) > File.GetLastWriteTime(dictionaryDirPath + file.Name))
            {
                localFiles.Add(file.Path);
            }
        }

        return localFiles;
    }

    private static StateOfSynchronization InitializeResult()
    {
        return new StateOfSynchronization()
        {
            IsSomeIssue = false,
        };
    }

    private static async Task<List<string>?> DownloadNewFiles(
        FtpHelper ftpHelper, List<string> localFiles, string dictionaryDirPath)
    {
        var newestFiles = new List<string>();
        var downloadSuccess = true;

        foreach (var file in localFiles)
        {
            if (await Task.Run(() => ftpHelper.DownloadFileFromFtp(file, dictionaryDirPath + Path.GetFileName(file))))
            {
                newestFiles.Add(Path.GetFileName(file));
            }
            else
            {
                downloadSuccess = false;
            }
        }

        if (!downloadSuccess)
        {
            return null;
        }

        return newestFiles;
    }

    private static List<string> GetLocalFilesToUpload(List<MyFile> ftpFiles, string dictionaryDirPath, List<string> newestFiles)
    {
        var filesToUpload = new List<string>();

        foreach (var file in Directory.GetFiles(dictionaryDirPath))
        {
            if (newestFiles.Contains(Path.GetFileName(file))) continue;

            var ftpFile = ftpFiles.Find(x => x.Name == Path.GetFileName(file));
            if (ftpFile == null || ConvertDateTimeWithoutMs(File.GetLastWriteTime(file))
                > ConvertDateTimeWithoutMs(DateTime.Parse(ftpFile.LastModified)))
            {
                filesToUpload.Add(file);
            }
        }

        return filesToUpload;
    }

    private static void UpdateResultWithFilesStatus(List<string> filesToUpload, List<string> newestFiles, ref StateOfSynchronization result)
    {
        result.IsSomeIssue = true;
        result.FilesToUpload = filesToUpload;
        result.NewestFiles = newestFiles;
    }

    private static StateOfSynchronization UpdateResultValue(string whatsUp, string message, ref StateOfSynchronization result)
    {
        result.WhatsUp = whatsUp;
        result.Info = message;

        return result;
    }

    private static DateTime ConvertDateTimeWithoutMs(DateTime dt)
    {
        return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
    }
}
