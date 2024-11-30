namespace CalendarioEventi.Models;

public class CalendarEvent
{
	public int Id { get; set; }
	public string Title { get; set; } = null!;
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
}