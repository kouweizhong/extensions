﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Signum.Services;
using Signum.Utilities;
using Signum.Entities;
using Signum.Web;
using Signum.Engine;
using Signum.Engine.Operations;
using Signum.Entities.Operations;
using Signum.Engine.Basics;
using Signum.Web.Extensions.Properties;
using System.IO;
using Signum.Entities.Files;
using Signum.Entities.Basics;
using System.Text;
using System.Net;
using Signum.Engine.Files;
using System.Reflection;

namespace Signum.Web.Files
{
    [HandleException]
    public class FileController : Controller
    {

        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult PartialView(
            string prefix,
            string fileType,
            int? sfId)
        {
            Type type = typeof(FilePathDN);
            string sfUrl = Navigator.Manager.EntitySettings[type].PartialViewName;
            FilePathDN entity = null;
            if (entity == null || entity.GetType() != type || sfId != (entity as IIdentifiable).TryCS(e => e.IdOrNull))
            {
                if (sfId.HasValue)
                    entity = Database.Retrieve<FilePathDN>(sfId.Value);
                else
                {
                    entity = new FilePathDN(EnumLogic<FileTypeDN>.ToEnum(fileType));
                }
            }
            ViewData["IdValueField"] = prefix;
            ViewData["FileType"] = fileType;

            if (typeof(EmbeddedEntity).IsAssignableFrom(entity.GetType()))
            {
                this.ViewData[ViewDataKeys.EmbeddedControl] = true;

                Type ts = typeof(TypeSubContext<>).MakeGenericType(new Type[] { entity.GetType() });
                TypeContext tc = (TypeContext)Activator.CreateInstance(ts, new object[] { entity, Modification.GetTCforEmbeddedEntity(Request.Form, entity, ref prefix), new PropertyInfo[] { } });

                return Navigator.PartialView(this, tc, "", sfUrl); //No prefix as its info is in the TypeContext
            }
            return Navigator.PartialView(this, entity, prefix, sfUrl);
        }

        public ActionResult Upload()
        {
            FilePathDN fp = null;
            string formFieldId = "";
            foreach (string file in Request.Files)
            {
                if (((string)Request.Form[TypeContext.Compose(file, TypeContext.StaticType)]) != "FilePathDN")
                    continue;

                string idStr = (string)Request.Form[TypeContext.Compose(file, TypeContext.Id)];
                int id;
                if (int.TryParse(idStr, out id))
                    continue; //Only new files will come with content

                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;

                string fileType = (string)Request.Form[TypeContext.Compose(file, FileLineKeys.FileType)];
                if (!fileType.HasText())
                    throw new ApplicationException(Resources.CouldntCreateFilePathWithUnknownFileTypeForField0.Formato(file));

                formFieldId = file; //This is the uploaded file

                fp = new FilePathDN(EnumLogic<FileTypeDN>.ToEnum(fileType))
                {
                    FileName = Path.GetFileName(hpf.FileName),
                    BinaryFile = hpf.InputStream.ReadAllBytes()
                }.Save();
            }

            StringBuilder sb = new StringBuilder();
            //Use plain javascript not to have to add also the reference to jquery in the result iframe
            sb.AppendLine("<html><head><title>-</title></head><body>");
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("var parDoc = window.parent.document;");

            if (fp.TryCS(f => f.IdOrNull) != null)
            {
                sb.AppendLine("parDoc.getElementById('{0}loading').style.display='none';".Formato(formFieldId));
                sb.AppendLine("parDoc.getElementById('{0}').innerHTML='{1}';".Formato(TypeContext.Compose(formFieldId, EntityBaseKeys.ToStrLink), fp.FileName));
                sb.AppendLine("parDoc.getElementById('{0}').value='FilePathDN';".Formato(TypeContext.Compose(formFieldId, TypeContext.RuntimeType)));
                sb.AppendLine("parDoc.getElementById('{0}').value='{1}';".Formato(TypeContext.Compose(formFieldId, TypeContext.Id), fp.Id.ToString()));
                sb.AppendLine("parDoc.getElementById('div{0}New').style.display='none';".Formato(formFieldId));
                sb.AppendLine("parDoc.getElementById('div{0}Old').style.display='block';".Formato(formFieldId));
            }
            else
            {
                sb.AppendLine("parDoc.getElementById('{0}loading').style.display='none';".Formato(formFieldId));
                sb.AppendLine("window.alert('Error guardando el archivo');");
            }

            sb.AppendLine("</script>");
            sb.AppendLine("</body></html>");

            return Content(sb.ToString());
        }

        public FileResult Download(int? filePathID)
        { 
            if (filePathID == null)
                throw new ArgumentException(Resources.ArgumentFilePathIDWasNotPassedToTheController);

            FilePathDN fp = Database.Retrieve<FilePathDN>(filePathID.Value);

            if (fp == null)
                throw new ArgumentException(Resources.ArgumentFilePathIDWasNotPassedToTheController);

            byte[] binaryFile;

            binaryFile = fp.WebPath != null ? new WebClient().DownloadData(fp.WebPath)
                : FilePathLogic.GetByteArray(fp);

            return File(binaryFile, GetMimeType(Path.GetExtension(fp.FileName)), fp.FileName);
        }

        private string GetMimeType(string extension)
        {
            string mimeType = String.Empty;

            // Attempt to get the mime-type from the registry.
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);

            if (regKey != null)
            {
                string type = (string)regKey.GetValue("Content Type");

                if (type != null)
                    mimeType = type;
            }

            return mimeType;
        }
    }
}
