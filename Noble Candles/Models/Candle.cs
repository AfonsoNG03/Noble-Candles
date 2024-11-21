using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{
	/*
		id	INT (PK, AI)	Identificador único da vela.
		name	VARCHAR(150)	Nome da vela.
		description	TEXT	Descrição detalhada da vela.
		price	DECIMAL(10, 2)	Preço da vela.
		stock	INT	Quantidade em estoque.
		category_id	INT (FK)	Categoria da vela (e.g., pequena).
		color_id	INT (FK)	Cor da vela.
		fragance_id	INT (FK)	Aroma da vela.
		created_at	DATETIME	Data de criação do registro da vela.
		updated_at	DATETIME	Data da última atualização do registro.
	 */
	public class Candle
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

		[Required]
		[Column(TypeName = "DECIMAL(10, 2)")]
		public required decimal Price { get; set; }

		[Required]
		public required int Stock { get; set; }

		[Required]
		public required int CategoryId { get; set; }
		public required Category Category { get; set; }

		[Required]
		public required int ColorId { get; set; }
		public required Color Color { get; set; }

		[Required]
		public required int FraganceId { get; set; }
		public required Fragrance Fragance { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[Column(TypeName = "DATETIME")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

	}
}
