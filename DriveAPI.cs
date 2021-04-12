using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace GoogleDriveAPI
{
    public class DriveAPI
    {
        public DriveService getDriveServie()
        {

            // FROM JSON
            //string PathToServiceAccountKeyFile = @"D:\Test\GoogleDriveAPIv3\GoogleDriveAPI\ServiceCredentials.json";
            //var credential = GoogleCredential.FromFile(PathToServiceAccountKeyFile)
            //    .CreateScoped(DriveService.ScopeConstants.Drive);


            // FROM P12
            var certificate = new X509Certificate2(@"certificate.p12", "notasecret", X509KeyStorageFlags.Exportable);
            var serviceAccountEmail = "driveapiservice@protean-torus-310411.iam.gserviceaccount.com";
            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = new[] { DriveService.Scope.Drive }
               }.FromCertificate(certificate));

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            return service;
        }

        public void makeFolder(DriveService service)
        {
            // Test Folder Id from my drive
            string parentFolderId = "1-GyVALzjtFj7gJ4RIvUAj1smZPG2973W";

            var fileMetadata = new File()
            {
                Name = "First /Last Names",
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>(new string[] { parentFolderId })
            };

            // create request to drive
            var request = service.Files.Create(fileMetadata);
            // get id folder was created
            request.Fields = "id";
            var folder = request.Execute();

            var folderId = folder.Id;

            // upload file to newly created folder by its id
            uploadFileToFolder(service, folderId);
        }

        public void uploadFileToFolder(DriveService service, string folderId)
        {
            // new file object
            var file = new File()
            {
                Name = "photo.jpg",
                MimeType = "image/jpeg",
                Parents = new List<string>(new string[] { folderId })
            };

            // new CreateMediaUpload request object
            FilesResource.CreateMediaUpload request;

            string path = "D:\\Test\\GoogleDriveAPIv3\\GoogleDriveAPI\\photo.jpg";

            using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
            {
                request = service.Files.Create(file, stream, "image/jpeg");
                // get the id of image after upload
                request.Fields = "id";
                request.Upload();
            }
             
            var createdFile = request.ResponseBody;
        }
    }
}
