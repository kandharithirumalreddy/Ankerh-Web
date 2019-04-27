using Ankerh.Web.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Ankerh.Web.Controllers;
namespace Ankerh.Web.Controllers
{
    public static class Test
    {
        public static Expression<Func<T, bool>> AndAlso1<T>(
 this Expression<Func<T, bool>> left,
 Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                    Expression.Invoke(left, param),
                    Expression.Invoke(right, param)
                );
            var lambda = Expression.Lambda<Func<T, bool>>(body, param);
            return lambda;
        }
    }



    [Authorize]
    public class AnkerhController : Controller
    {

        public async Task<ActionResult> Data(AnkerhRequest request, string button)
        {

            Expression<Func<AnkerDTO, bool>> predicate = null;
            if (button == "Clear")
            {
                request = new AnkerhRequest();
                //if (!string.IsNullOrEmpty(request.SearchText) && (request.StartDate.HasValue && request.EndDate.HasValue))
                //{

                //    predicate = p =>
                //    (p.Sagsnummer == request.SearchText
                //    || p.SagsNavn.Contains(request.SearchText)
                //    || p.AfsnitTekst.Contains(request.SearchText)
                //    || p.AktivitetTekst.Contains(request.SearchText)
                //    || p.Kreditornummer == request.SearchText
                //    || p.Kreditornavn.Contains(request.SearchText)
                //    || p.Filnavn.Contains(request.SearchText))
                //    && p.Fakturadato >= request.StartDate.Value && p.Fakturadato <= request.EndDate.Value.AddDays(1);

                //}
                //else if (!string.IsNullOrEmpty(request.SearchText))
                //{
                //    predicate = p =>
                // (p.Sagsnummer == request.SearchText
                // || p.SagsNavn.Contains(request.SearchText)
                // || p.AfsnitTekst.Contains(request.SearchText)
                // || p.AktivitetTekst.Contains(request.SearchText)
                // || p.Kreditornummer == request.SearchText
                // || p.Kreditornavn.Contains(request.SearchText)
                // || p.Filnavn.Contains(request.SearchText));
                //}
                //else if ((request.StartDate.HasValue && request.EndDate.HasValue))
                //{
                //    predicate = p => p.Fakturadato >= request.StartDate.Value && p.Fakturadato <= request.EndDate.Value.AddDays(1);
                //}
            }
            var data = await DocumentDBRepository<AnkerDTO>.GetItemsAsync(request, request.PageSize, request.Token);
          //  data.SearchText = request.SearchText;
            data.StartDate = request.StartDate;
            data.EndDate = request.EndDate;
            return View(data);
        }


        public async Task<ActionResult> Details(string id)
        {
            var data = await DocumentDBRepository<AnkerDTO>.GetItemAsync(id);
            return View(data);
        }

        public async Task<FileResult> GetPdf(string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create a CloudFileClient object for credentialed access to Azure Files.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("ankerhpdf");

            // Ensure that the share exists.
            if (share.Exists())
            {
                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("Faktura");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference(fileName);

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        MemoryStream ms = new MemoryStream();
                        await file.DownloadToStreamAsync(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        return File(ms, "application/pdf");
                    }
                }
            }
            return null;
        }
    }
}