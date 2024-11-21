using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Noble_Candles.Models
{

	/*
		id	INT (PK, AI)	Identificador único do utilizador.
		name	VARCHAR(100)	Nome do utilizador.
		telemovel	VARCHAR(50)	Telemovel do utilizador
		morada	VARCHAR(255)	Morada do utilizador.
		email	VARCHAR(150)	Email único do utilizador.
		password	VARCHAR(255)	Senha hash do utilizador.
		created_at	DATETIME	Data de criação da conta.
		updated_at	DATETIME	Data da última atualização.
	*/

	public class User
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		[Column(TypeName = "VARCHAR(100)")]
		public required string Name { get; set; }

		[MaxLength(50)]
		[Column(TypeName = "VARCHAR(50)")]
		public string? Telemovel { get; set; }

		[MaxLength(255)]
		[Column(TypeName = "VARCHAR(255)")]
		public string? Morada { get; set; }

		[Required]
		[MaxLength(150)]
		[Column(TypeName = "VARCHAR(150)")]
		[EmailAddress]
		public required string Email { get; set; }

		[Required]
		[MaxLength(255)]
		[Column(TypeName = "VARCHAR(255)")]
		public required string PasswordHash { get; set; }

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		[Required]
		[Column(TypeName = "DATETIME")]
		public DateTime UpdatedAt { get; set; } = DateTime.Now;

	}
}
