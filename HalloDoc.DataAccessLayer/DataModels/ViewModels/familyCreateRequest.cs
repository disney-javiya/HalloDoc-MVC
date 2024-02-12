using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.DataAccessLayer.DataModels.ViewModels
{
    public class familyCreateRequest
    {
        [StringLength(100)]
        public string? FamilyFirstName { get; set; }

         [StringLength(100)]
        public string? FamilyLastName { get; set; }

        [StringLength(23)]
        public string? FamilyPhoneNumber { get; set; } = null;

        [StringLength(50)]
        public string? FamilyEmail { get; set; }

        [StringLength(100)]
        public string? RelationName { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        public string? LastName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        [StringLength(23)]
        public string? PhoneNumber { get; set; } = null;

        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? ZipCode { get; set; }
    }
}
