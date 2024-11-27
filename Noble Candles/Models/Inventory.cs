using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Models
{
	public class Inventory
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int CandleId { get; set; } // Foreign key reference to Candle
		public Candle Candle { get; set; }

		[Required]
		public int Quantity { get; set; } // Stock quantity for this candle

		[Column(TypeName = "DATETIME")]
		public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
	}
}
