using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{
	/*
		id	INT (PK, AI)	Identificador único da categoria.
		name	VARCHAR(100)	Nome da categoria.
		description	TEXT	Descrição da categoria.
	 */
	public class Category
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[Column(TypeName = "VARCHAR(100)")]
		public required string Name { get; set; }

		[Required]
		[Column(TypeName = "TEXT")]
		public required string Description { get; set; }
	}
}
