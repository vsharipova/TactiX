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
    public class MatchAnalysisController : Controller
    {
        private readonly TactiXDB _context;
        private readonly MatchAnalysisService _analysisService;

        public MatchAnalysisController(TactiXDB context, MatchAnalysisService analysisService)
        {
            _context = context;
            _analysisService = analysisService;
        }

         [HttpGet]
    public async Task<IActionResult> MatchAnalysisList()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var matches = await _context.Matches
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.MatchDate)
            .ToListAsync();

        return View("MatchAnalysisList", matches);
    }

    [HttpGet("Analyze/{matchId}")]
    public async Task<IActionResult> Analyze(int matchId)
    {
        try
        {
            var analysis = await _analysisService.AnalyzeMatch(matchId);
            var userMatches = await _context.Matches
                .Where(m => m.UserId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .Where(m => m.MatchId != matchId)
                .ToListAsync();

            ViewBag.UserMatches = userMatches;
            return View("Index", analysis);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("MatchAnalysisList");
        }
    }

        [HttpGet("{matchId}")]
        public async Task<ActionResult<MatchAnalysisDto>> GetAnalysis(int matchId)
        {
            try
            {
                var analysis = await _analysisService.AnalyzeMatch(matchId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("compare")]
        public async Task<ActionResult<List<ComparisonDto>>> CompareMatches(
    [FromQuery] int baseMatchId,
    [FromQuery] int[] compareWithIds)
        {
            try
            {
                if (compareWithIds == null || compareWithIds.Length == 0)
                {
                    return BadRequest("Не указаны матчи для сравнения");
                }

                var comparisons = new List<ComparisonDto>();
                foreach (var matchId in compareWithIds)
                {
                    var comparison = await _analysisService.CompareMatches(baseMatchId, matchId);
                    comparisons.Add(comparison);
                }

                return Ok(comparisons);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    [HttpGet("ExportAllMatchesAnalysis")]
        public async Task<IActionResult> ExportAllMatchesAnalysis()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var matches = await _context.Matches
                .Where(m => m.UserId == userId)
                .Include(m => m.Analysis)
                .OrderByDescending(m => m.MatchDate)
                .ToListAsync();

            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
                {
                    // Добавляем основную часть документа
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    var body = mainPart.Document.AppendChild(new Body());

                    var titleParagraph = body.AppendChild(new Paragraph());
                    var titleRun = titleParagraph.AppendChild(new Run());
                    titleRun.AppendChild(new Text("Отчет об анализе всех матчей"));
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
                    totalStatsParagraph.AppendChild(new Run(new Text($"Всего матчей: {matches.Count}")));
                    totalStatsParagraph.ParagraphProperties = new ParagraphProperties(
                        new SpacingBetweenLines() { After = "100" }
                    );

                    if (!matches.Any())
                    {
                        body.AppendChild(new Paragraph(new Run(new Text("Нет данных о матчах для анализа"))));
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
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Матч")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дата")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Выстрелы")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Альфа %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Чарли %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Дельта %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Промахи %")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Hit Factor")) { RunProperties = new RunProperties(new Bold()) })));
                        headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Рейтинг")) { RunProperties = new RunProperties(new Bold()) })));

                        foreach (var match in matches)
                        {
                            var row = summaryTable.AppendChild(new TableRow());

                            row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.MatchName ?? "Без названия")))));

                            row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.MatchDate.ToString("dd.MM.yyyy"))))));

                            if (match.Analysis != null)
                            {
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.TotalShots.ToString())))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.AlphaPercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.CharliePercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.DeltaPercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.MissPercentage.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.AvgHitFactor.ToString("0.00"))))));
                                row.AppendChild(new TableCell(new Paragraph(new Run(new Text(match.Analysis.PerformanceScore.ToString("0.0"))))));
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

                        foreach (var match in matches.Where(m => m.Analysis != null))
                        {
                            var matchHeader = body.AppendChild(new Paragraph());
                            matchHeader.AppendChild(new Run(new Text($"{match.MatchName} - {match.MatchDate.ToString("dd.MM.yyyy")}")));
                            matchHeader.ParagraphProperties = new ParagraphProperties(
                                new RunProperties(new Bold()),
                                new SpacingBetweenLines() { Before = "200", After = "100" }
                            );

                            var advice = GenerateMatchAdvice(match.Analysis);
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
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет об анализе всех матчей_{DateTime.Now}.docx");
            }
        }

        private string GenerateMatchAdvice(MatchAnalysis analysis)
        {
            var advice = new List<string>();

            if (analysis.MissPercentage > 15)
                advice.Add("Слишком много промахов - работайте над точностью");

            if (analysis.AlphaPercentage < 40)
                advice.Add("Низкий процент попаданий в Альфа-зону - тренируйте прицеливание");

            if (analysis.AvgHitFactor < 2.0m)
                advice.Add("Низкий Hit Factor - работайте над скоростью при сохранении точности");

            return advice.Count > 0 ? string.Join("; ", advice) : "Хороший результат! Продолжайте в том же духе";
        }
    }
}
