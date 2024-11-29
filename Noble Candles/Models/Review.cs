using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{

	/*
		id	INT (PK, AI)	Identificador único da avaliação.
		user_id	string (FK)	Referência ao utilizador que avaliou.
		candle_id	INT (FK)	Referência à vela avaliada.
		rating	TINYINT	Nota de avaliação (1 a 5).
		comment	TEXT	Comentário da avaliação.
		created_at	DATETIME	Data da avaliação.
	    updated_at	DATETIME	Data da última atualização da avaliação.
	 */
	public class Review
	{

		[Key]
		public int Id { get; set; }

		public required string UserId { get; set; }
		public User User { get; set; }

		public required int CandleId { get; set; }
		public Candle Candle { get; set; }

		[Required]
		[Column(TypeName = "TINYINT")]
		public required byte Rating { get; set; }

		[Column(TypeName = "TEXT")]
		public string? Comment { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[Column(TypeName = "DATETIME")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}
}
