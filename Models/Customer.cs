using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Web;

namespace AzureASPNETApp.Models
{
    public class Customer
    {
        [Required(ErrorMessage = "Field can not be empty!")]
        public string Mail { get; set; }
        public string UploadedFileToken { get; set; }
        public HttpPostedFileBase CustomerFile { get; set; }
        public Stream PathToFile { get; set; }
    }
}