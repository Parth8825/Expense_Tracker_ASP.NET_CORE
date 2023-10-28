using Expense_Tracker_ASP.NET_CORE.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker_ASP.NET_CORE.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string? Icon { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string? Type { get; set; } = "Expense";

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [NotMapped]
        public string? TitleWithIcon 
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }
    }
}
 