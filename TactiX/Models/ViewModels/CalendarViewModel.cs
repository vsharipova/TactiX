using System.Collections.Generic;

namespace TactiX.Models.ViewModels
{
    public class CalendarViewModel
    {
        public List<CalendarEvent> Matches { get; set; }
        public List<CalendarEvent> Trainings { get; set; }

        public List<CalendarEvent> Events => Matches.Concat(Trainings).ToList();
    }

    public class CalendarEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
    }
}