using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels
{
    public class AuthenticationRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string KeyHandle { get; set; }

        public string Challenge { get; set; }
        
        public string AppId { get; set; }
        
        public string Version { get; set; }
    }
}