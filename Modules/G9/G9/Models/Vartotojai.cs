
namespace PowerApps.Modules.G9.Models;


public class User {
	public Guid ID { get; set; }
	public string? Vardas { get; set; }
	public string? Pavarde { get; set; }
	public string? Email { get; set; }
	public int? Apygarda { get; set; }
	public bool Active { get; set; }
	public DateTime? LastLogin { get; set; }
}



