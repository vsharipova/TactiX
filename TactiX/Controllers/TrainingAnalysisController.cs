using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TactiX.DBContext;
using TactiX.Models.ViewModels;
using TactiX.Services;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TactiX.Models;

namespace TactiX.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingAnalysisController : Controller
    {
        private readonly TactiXDB _context;
        private readonly TrainingAnalysisService _analysisService;

        public TrainingAnalysisController(TactiXDB context, TrainingAnalysisService analysisService)
        {
            _context = context;
            _analysisService = analysisService;
        }

        [HttpGet]
        public async Task<IActionResult> TrainingAnalysisList()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var trainings = await _context.Trainings
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TrainingDate)
                .ToListAsync();

            return View("TrainingAnalysisList", trainings);
        }

        [HttpGet("Analyze/{trainingId}")]
        public async Task<IActionResult> Analyze(int trainingId)
        {
            try
            {
                var analysis = await _analysisService.AnalyzeTraining(trainingId);
                var userTrainings = await _context.Trainings
                    .Where(t => t.UserId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                    .Where(t => t.TrainingId != trainingId)
                    .ToListAsync();

                ViewBag.UserTrainings = userTrainings;
                return View("Index", analysis);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("TrainingAnalysisList");
            }
        }

        [HttpGet("{trainingId}")]
        public async Task<ActionResult<TrainingAnalysisDto>> GetAnalysis(int trainingId)
        {
            try
            {
                var analysis = await _analysisService.AnalyzeTraining(trainingId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("compare")]
        public async Task<ActionResult<List<TrainingComparisonDto>>> CompareTrainings(
            [FromQuery] int baseTrainingId,
            [FromQuery] int[] compareWithIds)
        {
            try
            {
                if (compareWithIds == null || compareWithIds.Length == 0)
                {
                    return BadRequest("Не указаны тренировки для сравнения");
                }

                var comparisons = new List<TrainingComparisonDto>();
                foreach (var trainingId in compareWithIds)
                {
                    var comparison = await _analysisService.CompareTrainings(baseTrainingId, trainingId);
                    comparisons.Add(comparison);
                }

                return Ok(comparisons);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    [HttpGet("ExportAllTrainingsAnalysis")]
        public async Task<IActionResult> ExportAllTrainingsAnalysis()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var trainings = await _context.Trainings
                .Where(t => t.UserId == userId)
                .Include(t => t.Analysis)
                .OrderByDescending(t => t.TrainingDate)
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    var titleParagraph = body.AppendChild(new Paragraph());
                    var titleRun = titleParagraph.AppendChild(new Run());
                    titleRun.AppendChild(new Text("Отчет об анализе тренировок"));
                    titleRun.RunProperties = new RunProperties(new Bold(), new FontSize() { Val = "28" });
                    titleParagraph.ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center },
                        new SpacingBetweenLines() { After = "200" }
                    );

                    var dateParagraph = body.AppendChild(new Paragraph());
                    dateParagraph.AppendChild(new Run(new Text($"Дата формирования отчета: {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}")));
                    dateParagraph.ParagraphProperties = new ParagraphProperties(
                        new SpacingBetweenLines() { After = "200" }
                    );

                    var totalStatsParagraph = body.AppendChild(new Paragraph());
                    totalStatsParagraph.AppendChild(new Run(new Text($"Всего тренировок: {trainings.Count}")));
                    totalStatsParagraph.ParagraphProperties = new ParagraphProperties(
                        new SpacingBetweenLines() { After = "100" }
                    );

                    if (!trainings.Any())
                    {
                        body.AppendChild(new Paragraph(new Run(new Text("Нет данных о тренировках для анализа"))));
                    }
                    else
                    {
                        var summaryTable = body.AppendChild(new Table());

                        var tableProperties = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                            ),
                            new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
                        );
                        summaryTable.AppendChild(tableProperties);

                        var headerRow = summaryTable.AppendChild(new TableRow());
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Тип тренировки")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дата")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Выстрелы")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Альфа %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Чарли %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дельта %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Промахи %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Hit Factor")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Рейтинг")) { RunProperties = new RunProperties(new Bold()) })));

                        foreach (var training in trainings)
                        {
                            var row = summaryTable.AppendChild(new TableRow());
                            
                            row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.TypeOfTraining ?? "Не указан")))));

                            row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.TrainingDate.ToString("dd.MM.yyyy"))))));

                            if (training.Analysis != null)
                            {
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.TotalShots.ToString())))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.AlphaPercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.CharliePercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.DeltaPercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.MissPercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.AvgHitFactor.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(training.Analysis.PerformanceScore.ToString("0.0"))))));
                            }
                            else
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text("-")))));
                                }
                            }
                        }

                        body.AppendChild(new Paragraph(new Run(new Text(""))));

                        foreach (var training in trainings.Where(t => t.Analysis != null))
                        {
                            var trainingHeader = body.AppendChild(new Paragraph());
                            trainingHeader.AppendChild(new Run(new Text($"{training.TypeOfTraining} - {training.TrainingDate.ToString("dd.MM.yyyy")}")));
                            trainingHeader.ParagraphProperties = new ParagraphProperties(
                                new RunProperties(new Bold()),
                                new SpacingBetweenLines() { Before = "200", After = "100" }
                            );

                            var advice = GenerateTrainingAdvice(training.Analysis);
                            if (!string.IsNullOrEmpty(advice))
                            {
                                var adviceParagraph = body.AppendChild(new Paragraph());
                                adviceParagraph.AppendChild(new Run(new Text($"Рекомендации: {advice}")));
                                adviceParagraph.ParagraphProperties = new ParagraphProperties(
                                    new SpacingBetweenLines() { After = "100" }
                                );
                            }

                            body.AppendChild(new Paragraph(new Run(new Text(""))));
                        }
                    }

                    wordDocument.Save();
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет об анализе всех тренировок_{DateTime.Now}.docx");
            }
        }

        private string GenerateTrainingAdvice(TrainingAnalysis analysis)
        {
            var advice = new List<string>();

            if (analysis.MissPercentage > 15)
                advice.Add("Слишком много промахов - работайте над точностью");

            if (analysis.AlphaPercentage < 40)
                advice.Add("Низкий процент попаданий в Альфа-зону - тренируйте прицеливание");

            if (analysis.TrainingType == "Скорость" && analysis.AvgHitFactor < 5)
                advice.Add("Низкий Hit Factor - работайте над скоростью без потери точности");
            else if (analysis.TrainingType == "Точность" && analysis.AlphaPercentage < 60)
                advice.Add("Тренируйте плавный спуск курка");
            else if (analysis.TrainingType == "Комбинация")
                advice.Add("Уделите внимание переходам между мишенями");
            else if (analysis.TrainingType == "Силовая")
                advice.Add("Работайте над силой хвата и выносливостью");

            return advice.Count > 0 ? string.Join("; ", advice) : "Хороший результат! Продолжайте в том же духе";
        }
    }
}