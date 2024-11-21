using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{

	/*
		id	INT (PK, AI)	Identificador único da cor.
		name	VARCHAR(150)	Nome da cor.
	 */
	public class Color
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		[Column(TypeName = "VARCHAR(150)")]
		public required string Name { get; set; }
	}
}
