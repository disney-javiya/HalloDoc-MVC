using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataAccessLayer.DataModels.ViewModels
{
    public class createPatientRequest
    {
             public string? Symptoms { get; set; }

            [StringLength(100)]
            public string FirstName { get; set; } = null!;

            [StringLength(100)]
            public string? LastName { get; set; }


            [Column("dateofbirth")]
            public DateOnly? Dateofbirth { get; set; }

            [StringLength(50)]
            public string Email { get; set; } = null!;

            [StringLength(20)]
            public string? Mobile { get; set; }

            [StringLength(100)]
            public string? Street { get; set; }

            [StringLength(100)]
            public string? City { get; set; }

            [StringLength(100)]
            public string? State { get; set; }

            [StringLength(10)]
            public string? ZipCode { get; set; }
            
            [Column("noofrooms")]
            public int? Noofrooms { get; set; }


            [Column("documents")]
            public byte[]? Documents { get; set; }


    }
}
