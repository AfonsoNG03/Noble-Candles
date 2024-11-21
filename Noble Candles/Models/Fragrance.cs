using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{
	/*
	id	INT (PK, AI)	Identificador único do aroma.
	name	VARCHAR(150)	Nome do aroma.
	description	TEXT	Descrição detalhada do aroma.
	 */
	public class Fragrance
	{

		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		[Column(TypeName = "VARCHAR(150)")]
		public required string Name { get; set; }
		
		[Required]
		[Column(TypeName = "TEXT")]
		public required string Description { get; set; }
	}
}
