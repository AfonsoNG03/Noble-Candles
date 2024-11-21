using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{

	/*
		id	INT (PK, AI)	Identificador único do registro.
		user_id	INT (FK)	Referência ao utilizador.
		candle_id	INT (FK)	Referência ao produto (vela) favorito.
		created_at	DATETIME	Data em que o item foi adicionado.
	 */
	public class Favorite
	{

		[Key]
		public int Id { get; set; }

		public required int UserId { get; set; }
		public required User User { get; set; }

		public required int CandleId { get; set; }
		public required Candle Candle { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}
