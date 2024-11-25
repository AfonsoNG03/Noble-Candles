using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{
	public class User : IdentityUser
	{

		[MaxLength(255)]
		public string? Morada { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[Column(TypeName = "DATETIME")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}
}
