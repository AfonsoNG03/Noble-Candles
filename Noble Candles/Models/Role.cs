using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Models
{
	public class Role
	{
		[Key]
		public int RoleId { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;
	}

}
