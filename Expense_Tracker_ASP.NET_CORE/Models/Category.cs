﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker_ASP.NET_CORE.Models
{
    public class Category
    {
        [Key]
        public int CatogeryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string? Icon { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string? Type { get; set; } = "Expense";

    }
}
 