using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{

	/*
		id	INT (PK, AI)	Identificador único do status.
		name	VARCHAR(50)	Nome do status (pendente, pago, enviado).
	 */
	public class Status
	{

		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		[Column(TypeName = "VARCHAR(50)")]
		public required string Name { get; set; }
	}
}
