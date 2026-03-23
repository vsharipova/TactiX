using System.Text.Json;

namespace TactiX.Models.ViewModels
{
    public class BriefingSaveDto
    {
        public int StageId { get; set; }
        public string StageType { get; set; } 
        public JsonDocument BriefingData { get; set; }
    }
}