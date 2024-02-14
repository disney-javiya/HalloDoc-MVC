using Microsoft.AspNetCore.Http;
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
            [StringLength(250)]
            public string Symptoms { get; set; } = null!;

            public DateOnly DateOfBirth { get; set; }

            [StringLength(100)]
            public string FirstName { get; set; } = null!;

            [StringLength(100)]
            public string? LastName { get; set; }

            [Column(TypeName = "character varying")]
            public string? PasswordHash { get; set; }

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

            public int RequestTypeId { get; set; }
            public IFormFile[] MultipleFiles { get; set; }
    }
}
