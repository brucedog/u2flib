using System;
using System.ComponentModel.DataAnnotations;

namespace DemoU2FSite.Repository
{
    public class Device
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public byte[] KeyHandle { get; set; }

        public byte[] PublicKey { get; set; }

        public byte[] AttestationCert { get; set; }

        public uint Counter { get; set; }
    }
}