using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoU2FSite.Repository
{
    public class DeviceRegistration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public byte[] KeyHandle { get; set; }

        public byte[] PublicKey { get; set; }

        public byte[] AttestationCert { get; set; }

        public uint Counter { get; set; }
    }
}