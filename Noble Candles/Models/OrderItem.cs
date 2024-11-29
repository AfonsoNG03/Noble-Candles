using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{
	/*
		id	INT (PK, AI)	Identificador único do item.
		order_id	INT (FK)	Referência ao pedido.
		candle_id	INT (FK)	Referência à vela comprada.
		quantity	INT	Quantidade comprada do item.
		price	DECIMAL(10, 2)	Preço do item no momento da compra.
	 */
	public class OrderItem
	{

		[Key]
		public int Id { get; set; }

		public required int OrderId { get; set; }
		public Order Order { get; set; }

		public required int CandleId { get; set; }
		public Candle Candle { get; set; }

		[Required]
		public required int Quantity { get; set; }

		[Required]
		[Column(TypeName = "DECIMAL(10, 2)")]
		public required decimal Price { get; set; }

	}
}
