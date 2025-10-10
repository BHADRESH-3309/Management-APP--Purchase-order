using System.ComponentModel.DataAnnotations;

namespace WebManagementApp.Models
{
    public class MappingSkuFileModel
    {
        [Required(ErrorMessage = "*required")]
        public IFormFile File { get; set; }
    }
}
