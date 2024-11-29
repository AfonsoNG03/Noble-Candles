using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{

	/*
		id	INT (PK, AI)	Identificador único do pedido.
		user_id	INT (FK)	Referência ao usuário que fez o pedido.
		total_price	DECIMAL(10, 2)	Valor total do pedido.
		status_id	INT (FK)	Status do pedido.
		created_at	DATETIME	Data do pedido.
		updated_at	DATETIME	Data da última atualização.
	 */
	public class Order
	{

		[Key]
		public int Id { get; set; }

		public required string UserId { get; set; }
		public User User { get; set; }

		[Column(TypeName = "DECIMAL(10, 2)")]
		public required decimal TotalPrice { get; set; }

		public required int StatusId { get; set; }
		public Status Status { get; set; }

		[Column(TypeName = "DATETIME")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[Column(TypeName = "DATETIME")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}
}
