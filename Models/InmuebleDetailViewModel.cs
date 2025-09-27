namespace PC02.Models
{
  public class InmuebleDetailViewModel
  {
    public Inmueble Inmueble { get; set; } = null!;
    public bool HasActiveReservation { get; set; }
    public VisitaInputModel VisitaInput { get; set; } = new();
    public string? Feedback { get; set; }
    public bool Success { get; set; }
  }
}
