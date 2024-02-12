using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc.DataAccessLayer.DataModels.ViewModels
{
    public class conciergeCreateRequest
    {

        [StringLength(100)]
        public string? ConciergeFirstName { get; set; }

        [StringLength(100)]
        public string? ConciergeLastName { get; set; }

        [StringLength(23)]
        public string? ConciergePhoneNumber { get; set; } = null;

        [StringLength(50)]
        public string? ConciergeEmail { get; set; }

        [StringLength(100)]
        public string? ConciergePropertyName { get; set; }

        [StringLength(100)]
        public string? ConciergeStreet { get; set; }

        [StringLength(100)]
        public string? ConciergeCity { get; set; }

        [StringLength(100)]
        public string? ConciergeState { get; set; }

        [StringLength(10)]
        public string? ConciergeZipCode { get; set; }

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
    }
}
