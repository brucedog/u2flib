using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public byte[] KeyHandle { get; set; }

        public byte[] PublicKey { get; set; }

        public byte[] AttestationCert { get; set; }

        public int Counter { get; set; }

        public bool IsCompromised { get; set; }
    }
}