using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace WinConnector
{
    // FtpWebRequest docs > https://www.c-sharpcorner.com/UploadFile/0d5b44/ftp-using-C-Sharp-net/
    class FtpConnector : Connector
    {
        Uri serverUri;
        String currentPath;
        public FtpConnector()
        {
            serverUri = null;
            currentPath = null;
        }
        // function used to quickly prpare ftp connession on new request
        private FtpWebRequest prepareConnection(string additionalPath = "")
        {
            FtpWebRequest connession = (FtpWebRequest) WebRequest.Create("ftp://" + serverUri.Host + ":" + serverUri.Port + "/" + additionalPath);
            if(serverUri.UserInfo != "")
            {
                // splitting username and password
                // splitted as follow:
                // <username>:<password> (0 and 1 index)
                string[] credentials = serverUri.UserInfo.Split(':');
                // verifying if split was successfull otherwise ignoring credentials
                if(credentials.Length == 2)
                {
                    connession.Credentials = new NetworkCredential(credentials[0], credentials[1]);
                }
            }
            connession.KeepAlive = false;
            connession.UseBinary = true;
            connession.UsePassive = true;
            return connession;
        }
        public override bool Connect(Uri uri)
        {
            bool success = true;
            serverUri = uri;
            currentPath = "/";
            FtpWebRequest request = prepareConnection();
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            try
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                success = false;
                serverUri = null;
            }
            return success;
        }

        public override void Disconnect()
        {
            serverUri = null;
        }

        public override bool IsConnected()
        {
            return serverUri != null;
        }

        public override ArrayList GetListOfCurrentDirectory()
        {
            ArrayList directories = new ArrayList();
            if (IsConnected())
            {
                FtpWebRequest request = prepareConnection();

                request.Method = WebRequestMethods.Ftp.ListDirectory;
                try
                {
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);
                    while (!reader.EndOfStream)
                    {
                        directories.Add(reader.ReadLine());
                    }
                }
                catch (Exception ex) { }
            }

            return directories;
        }

        public override async Task<bool> Download(string filePath)
        {
            bool success = false;

            FtpWebRequest request = prepareConnection(filePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse response = (FtpWebResponse) request.GetResponse();

            Stream responseStream = response.GetResponseStream();

            string[] fileName = filePath.Split(".");

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add(fileName[0], new List<string>() { "." + fileName[1] });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                Stream fileStream = await file.OpenStreamForWriteAsync();
                responseStream.CopyTo(fileStream);
                success = true;
            }
            else
            {
                success = false;
            }

            return success;
        }
    }
}
