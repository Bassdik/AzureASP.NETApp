using AzureASPNETApp.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace AzureASPNETApp.Controllers
{
    public class HomeController : Controller
    {
        Customer cust;
        public HomeController()
        {
            cust = new Customer();
        }
        public ActionResult Index(Customer customer)
        {
            return View(customer);
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Fail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateToken(string Mail, HttpPostedFileBase CustomerFile)
        {
            if (!string.IsNullOrEmpty(Mail) && CustomerFile != null)
            {
                cust.Mail = Mail;
                cust.CustomerFile = CustomerFile;
                cust.PathToFile = CustomerFile.InputStream;
                if (TokenCreating() && SendMessage())
                {
                    return View("Success", cust);
                }
                else
                {
                    return RedirectToAction("Fail", "Home");
                }
            }
            return RedirectToAction("Fail", "Home");
        }
        bool TokenCreating()
        {
            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=azureblobstorageaccount5;AccountKey=E7LDRxyZPRvrB40q+eltcsHUi2SYDCtFv2vAdYflSrVMnEfGnPNIPHJ7uTKKeMf9SSd/JGRP5z7S+AStj7PEdQ==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference("container");
                CloudBlockBlob blob = container.GetBlockBlobReference("New_Blob");

                using (var fileStream = cust.PathToFile)
                {
                    blob.UploadFromStream(fileStream);
                }
                var sasPolicy = new SharedAccessBlobPolicy
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1) // Устанавливаем срок действия на 1 час
                };

                // Генерируем токен SAS
                var sasToken = blob.GetSharedAccessSignature(sasPolicy);



                // Возвращаем URL с токеном
                string fileUrlWithSasToken = blob.Uri + sasToken;
                cust.UploadedFileToken = fileUrlWithSasToken;
                return true;
            }
            catch
            {
                return false;
            }

        }
        bool SendMessage()
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            // Учетная запись отправителя
            string fromEmail = "abdulname75@gmail.com";

            // Адрес получателя
            string toEmail = cust.Mail;

            // Создаем объект SmtpClient
            SmtpClient client = new SmtpClient(smtpServer, smtpPort);
            client.EnableSsl = true; // Используем SSL для безопасной передачи данных

            // Устанавливаем учетные данные отправителя
            client.Credentials = new NetworkCredential(fromEmail, "kdalhlitjmwhxlnh");

            // Создаем объект MailMessage
            MailMessage message = new MailMessage(fromEmail, toEmail);

            // Задаем тему и текст сообщения
            message.Subject = "Token sreated successfully!";
            message.Body = "Your token: " + cust.UploadedFileToken;

            try
            {
                // Отправляем сообщение
                client.Send(message);
            }
            catch
            {
                return false;
            }
            finally
            {
                // Закрываем соединение с SMTP-сервером
                client.Dispose();

            }
            return true;
        }
    }
}