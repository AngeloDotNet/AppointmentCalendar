using CalendarioEventi.Enums;
using CalendarioEventi.Models;
using System.Globalization;

namespace CalendarioEventi.Pages;

public partial class Calendar
{
	private readonly CultureInfo currentCulture = CultureInfo.CurrentCulture;
	private readonly DayOfWeek[] dayNames = Enum.GetValues<DayOfWeek>();
	private readonly List<CalendarEvent> events = [];

	private DateTime currentMonth = DateTime.Now;
	private List<List<DateTime?>> weeks = [];
	private Dictionary<string, string> dayTranslations = [];

	private static readonly Dictionary<string, Type> cultureToEnumMap = new()
	{
		{ "it-IT", typeof(EnumWeekITA) },
		{ "es-ES", typeof(EnumWeekESP) },
		{ "fr-FR", typeof(EnumWeekFRA) },
		{ "de-DE", typeof(EnumWeekGER) },
		{ "default", typeof(EnumWeekENG) }
	};

	protected override async Task OnInitializedAsync()
	{
		var enumType = cultureToEnumMap.TryGetValue(currentCulture.Name, out var type) ? type : cultureToEnumMap["default"];
		GenerateDayTranslations(enumType);

		await LoadEvents();
		GenerateCalendar();
	}

	private void GenerateDayTranslations(Type enumType)
	{
		dayTranslations = Enum.GetValues<DayOfWeek>()
			.ToDictionary(day => day.ToString(), day
				=> Enum.GetName(enumType, (int)day + 1))!;
	}

	private async Task LoadEvents()
	{
		// Load events from API
		//events = await Http.GetFromJsonAsync<List<CalendarEvent>>("api/calendar/events");

		// Simulate loading events
		await Task.Delay(100);

		var numberOfEvents = 10;
		var random = new Random();

		for (int i = 0; i < numberOfEvents; i++)
		{
			var nextDay = DateTime.Now.AddDays(i);

			events.Add(new CalendarEvent
			{
				Id = i,
				Title = $"Event {i + 1}",
				StartDate = nextDay,
				EndDate = nextDay.AddHours(random.Next(10))
			});
		}
	}

	private void PreviousMonth()
	{
		currentMonth = currentMonth.AddMonths(-1);
		GenerateCalendar();
	}

	private void NextMonth()
	{
		currentMonth = currentMonth.AddMonths(1);
		GenerateCalendar();
	}

	private void GenerateCalendar()
	{
		weeks = [];

		var firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
		var firstDayOfCalendar = firstDayOfMonth.AddDays(-(int)firstDayOfMonth.DayOfWeek);
		var lastDayOfCalendar = firstDayOfMonth.AddMonths(1)
			.AddDays(6 - (int)firstDayOfMonth.AddMonths(1)
			.AddDays(-1).DayOfWeek);

		for (var date = firstDayOfCalendar; date <= lastDayOfCalendar; date = date.AddDays(7))
		{
			var week = new List<DateTime?>();

			for (var i = 0; i < 7; i++)
			{
				week.Add(date.Month == currentMonth.Month ? date : (DateTime?)null);
				date = date.AddDays(1);
			}

			weeks.Add(week);
		}
	}

	private string TranslateDay(DayOfWeek day)
		=> dayTranslations.TryGetValue(day.ToString(), out var translatedDay) ? translatedDay : day.ToString();

	private string CalendarioTitle
		=> currentCulture.Name switch
		{
			"it-IT" => "Calendario Eventi",
			"es-ES" => "Calendario de Eventos",
			"fr-FR" => "Calendrier des événements",
			"de-DE" => "Veranstaltungskalender",
			_ => "Events Calendar"
		};

	private string NextMonthText
		=> currentCulture.Name switch
		{
			"it-IT" => "Mese successivo",
			"es-ES" => "Mes siguiente",
			"fr-FR" => "Mois suivant",
			"de-DE" => "Nächster Monat",
			_ => "Next Month"
		};

	private string CurrentMonthText
		=> CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentMonth.ToString("MMMM yyyy"));

	private string PreviousMonthText
		=> currentCulture.Name switch
		{
			"it-IT" => "Mese precedente",
			"es-ES" => "Mes anterior",
			"fr-FR" => "Mois précédent",
			"de-DE" => "Vorheriger Monat",
			_ => "Previous Month"
		};

	private string CheckOutEventDayText
		=> currentCulture.Name switch
		{
			"it-IT" => "Visualizza eventi del giorno",
			"es-ES" => "Ver eventos del día",
			"fr-FR" => "Voir les événements du jour",
			"de-DE" => "Veranstaltungen des Tages anzeigen",
			_ => "Check out events of the day"
		};

	private string NoEventDayText
		=> currentCulture.Name switch
		{
			"it-IT" => "Nessun evento",
			"es-ES" => "Ningún evento",
			"fr-FR" => "Aucun événement",
			"de-DE" => "Keine Veranstaltung",
			_ => "No event"
		};
}