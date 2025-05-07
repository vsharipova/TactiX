using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TactiX.DBContext;
using TactiX.Models;
using TactiX.Models.ViewModels;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TactiX.Models;

namespace TactiX.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly TactiXDB _context;

        public CalendarController(TactiXDB context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var model = new CalendarViewModel
            {
                Matches = await _context.Matches
                    .Where(m => m.UserId == userId) 
                    .Select(m => new CalendarEvent
                    {
                        Id = m.MatchId,
                        Title = m.MatchName,
                        Date = m.MatchDate,
                        Type = "match",
                        Color = "#e63946"
                    })
                    .ToListAsync(),

                Trainings = await _context.Trainings
                    .Where(t => t.UserId == userId) 
                    .Select(t => new CalendarEvent
                    {
                        Id = t.TrainingId,
                        Title = t.TypeOfTraining,
                        Date = t.TrainingDate,
                        Type = "training",
                        Color = "#457b9d"
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMatch(DateTime date, string title)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var match = new Match
            {
                MatchName = title,
                MatchDate = date,
                UserId = userId
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> MatchDetails(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var match = await _context.Matches
                .Where(m => m.UserId == userId)
                .Include(m => m.Stages)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null) return NotFound();

            return View(match);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var match = await _context.Matches
                .Where(m => m.UserId == userId) 
                .Include(m => m.Stages)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMatch(Match model)
        {
            var match = await _context.Matches.FindAsync(model.MatchId);
            if (match == null) return NotFound();

            match.MatchName = model.MatchName;
            match.MatchDate = model.MatchDate;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddMatchStage([FromForm] MatchStageCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.HitFactor = (5 * model.AlphasCount + 3 * model.CharliesCount + 1 * model.DeltasCount)
                / Math.Max(model.StageTime.TotalSeconds, 0.1f);

            var stage = new MatchStage
            {
                MatchId = model.MatchId,
                StageName = model.StageName,
                HitFactor = model.HitFactor,
                StageType = model.StageType,
                NumOfSpots = model.NumOfSpots,
                NumOfPoppers = model.NumOfPoppers,
                NumOfPlates = model.NumOfPlates,
                AlphasCount = model.AlphasCount,
                DeltasCount = model.DeltasCount,
                CharliesCount = model.CharliesCount,
                MissesCount = model.MissesCount,
                StageTime = model.StageTime
            };

            var matchId = stage.MatchId;
            _context.MatchStages.Add(stage);
            await _context.SaveChangesAsync();

            return RedirectToAction("MatchDetails", new { id = matchId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStage(int id)
        {
            try
            {
                var stage = await _context.MatchStages.FindAsync(id);
                if (stage == null)
                {
                    return NotFound();
                }

                var matchId = stage.MatchId; 
                _context.MatchStages.Remove(stage);
                await _context.SaveChangesAsync();

                return RedirectToAction("MatchDetails", new { id = matchId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при удалении этапа: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditStage(int id)
        {
            var stage = await _context.MatchStages.FindAsync(id);
            if (stage == null) return NotFound();

            var model = new MatchStageEditDto
            {
                MatchStageId = stage.MatchStageId,
                StageName = stage.StageName,
                StageType = stage.StageType,
                HitFactor = stage.HitFactor,
                NumOfSpots = stage.NumOfSpots,
                NumOfPoppers = stage.NumOfPoppers,
                NumOfPlates = stage.NumOfPlates,
                AlphasCount = stage.AlphasCount,
                DeltasCount = stage.DeltasCount,
                CharliesCount = stage.CharliesCount,
                MissesCount = stage.MissesCount,
                StageTime = stage.StageTime,
                MatchId = stage.MatchId 
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditStage(MatchStageEditDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var stage = await _context.MatchStages.FindAsync(model.MatchStageId);
            if (stage == null) return NotFound();

            model.HitFactor = (5 * model.AlphasCount + 3 * model.CharliesCount + 1 * model.DeltasCount)
                / Math.Max(model.StageTime.TotalSeconds, 0.1f);

            stage.StageName = model.StageName;
            stage.StageType = model.StageType;
            stage.HitFactor = model.HitFactor;
            stage.NumOfSpots = model.NumOfSpots;
            stage.NumOfPoppers = model.NumOfPoppers;
            stage.NumOfPlates = model.NumOfPlates;
            stage.AlphasCount = model.AlphasCount;
            stage.DeltasCount = model.DeltasCount;
            stage.CharliesCount = model.CharliesCount;
            stage.MissesCount = model.MissesCount;
            stage.StageTime = model.StageTime;

            await _context.SaveChangesAsync();

            return RedirectToAction("MatchDetails", new { id = model.MatchId });
        }

        [HttpPost]
        public async Task<IActionResult> AddTraining([FromForm] DateTime date, [FromForm] string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                ModelState.AddModelError("type", "Тип тренировки обязателен");
                return View("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var training = new Training
            {
                TrainingDate = date,
                TypeOfTraining = type,
                UserId = userId
            };

            try
            {
                _context.Trainings.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "Не удалось сохранить тренировку");
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> TrainingDetails(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var training = await _context.Trainings
                .Where(m => m.UserId == userId) 
                .Include(t => t.Stages)
                .FirstOrDefaultAsync(t => t.TrainingId == id);

            if (training == null) return NotFound();

            return View(training);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTraining(Training model)
        {
            var training = await _context.Trainings.FindAsync(model.TrainingId);
            if (training == null) return NotFound();

            training.TypeOfTraining = model.TypeOfTraining;
            training.TrainingDate = model.TrainingDate;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddTrainingStage([FromForm] TrainingStageCreateDto model)
        {
            model.HitFactor = (5 * model.AlphasCount + 3 * model.CharliesCount + 1 * model.DeltasCount)
                / Math.Max(model.StageTime.TotalSeconds, 0.1f);

            model.HitFactor = (float)Math.Round(model.HitFactor, 4);
            var stage = new TrainingStage
            {
                TrainingId = model.TrainingId,
                StageName = model.StageName,
                StageType = model.StageType,
                HitFactor = model.HitFactor,
                NumOfSpots = model.NumOfSpots,
                NumOfPoppers = model.NumOfPoppers,
                NumOfPlates = model.NumOfPlates,
                AlphasCount = model.AlphasCount,
                DeltasCount = model.DeltasCount,
                CharliesCount = model.CharliesCount,
                MissesCount = model.MissesCount,
                StageTime = model.StageTime
            };

            _context.TrainingStages.Add(stage);
            await _context.SaveChangesAsync();

            return RedirectToAction("TrainingDetails", new { id = model.TrainingId });
        }

        [HttpGet]
        public async Task<IActionResult> EditTrainingStage(int id)
        {
            var stage = await _context.TrainingStages.FindAsync(id);
            if (stage == null) return NotFound();

            var model = new TrainingStageEditDto
            {
                TrainingStageId = stage.TrainingStageId,
                TrainingId = stage.TrainingId,
                StageName = stage.StageName,
                StageType = stage.StageType,
                HitFactor = stage.HitFactor,
                NumOfSpots = stage.NumOfSpots,
                NumOfPoppers = stage.NumOfPoppers,
                NumOfPlates = stage.NumOfPlates,
                AlphasCount = stage.AlphasCount,
                DeltasCount = stage.DeltasCount,
                CharliesCount = stage.CharliesCount,
                MissesCount = stage.MissesCount,
                StageTime = stage.StageTime
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrainingStage(TrainingStageEditDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.HitFactor = (5 * model.AlphasCount + 3 * model.CharliesCount + 1 * model.DeltasCount)
                 / Math.Max(model.StageTime.TotalSeconds, 0.1f);
            model.HitFactor = (float)Math.Round(model.HitFactor, 4);

            var stage = await _context.TrainingStages.FindAsync(model.TrainingStageId);
            if (stage == null) return NotFound();

            stage.StageName = model.StageName;
            stage.StageType = model.StageType;
            stage.HitFactor = model.HitFactor;
            stage.NumOfSpots = model.NumOfSpots;
            stage.NumOfPoppers = model.NumOfPoppers;
            stage.NumOfPlates = model.NumOfPlates;
            stage.AlphasCount = model.AlphasCount;
            stage.DeltasCount = model.DeltasCount;
            stage.CharliesCount = model.CharliesCount;
            stage.MissesCount = model.MissesCount;
            stage.StageTime = model.StageTime;

            await _context.SaveChangesAsync();

            return RedirectToAction("TrainingDetails", new { id = model.TrainingId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTrainingStage(int id)
        {
            var stage = await _context.TrainingStages.FindAsync(id);
            if (stage == null) return NotFound();

            var trainingId = stage.TrainingId;
            _context.TrainingStages.Remove(stage);
            await _context.SaveChangesAsync();

            return RedirectToAction("TrainingDetails", new { id = trainingId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTraining(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var training = await _context.Trainings
                .Where(m => m.UserId == userId) 
                .Include(m => m.Stages)
                .FirstOrDefaultAsync(m => m.TrainingId == id);

            if (training != null)
            {
                _context.Trainings.Remove(training);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportCalendarReport()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var matches = await _context.Matches
                .Where(m => m.UserId == userId)
                .Include(m => m.Stages)
                .ToListAsync();

            var trainings = await _context.Trainings
                .Where(t => t.UserId == userId)
                .Include(t => t.Stages)
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
                    titleRun.AppendChild(new Text("Отчет о событиях календаря"));
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

                    if (matches.Any())
                    {
                        var matchesHeading = body.AppendChild(new Paragraph());
                        matchesHeading.AppendChild(new Run(new Text("Матчи")));
                        matchesHeading.ParagraphProperties = new ParagraphProperties(
                            new RunProperties(new Bold(), new Underline() { Val = UnderlineValues.Single }),
                            new SpacingBetweenLines() { Before = "400", After = "200" }
                        );

                        foreach (var match in matches)
                        {
                            var matchParagraph = body.AppendChild(new Paragraph());
                            matchParagraph.AppendChild(new Run(new Text($"{match.MatchName} - {match.MatchDate.ToString("dd.MM.yyyy HH:mm")}")));
                            matchParagraph.ParagraphProperties = new ParagraphProperties(
                                new RunProperties(new Bold()),
                                new SpacingBetweenLines() { Before = "200", After = "100" }
                            );

                            if (match.Stages.Any())
                            {
                                var stagesTable = body.AppendChild(new Table());

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
                                stagesTable.AppendChild(tableProperties);

                                var headerRow = stagesTable.AppendChild(new TableRow());
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Название этапа")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Хит-фактор")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Время")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Результат")) { RunProperties = new RunProperties(new Bold()) })));

                                foreach (var stage in match.Stages)
                                {
                                    var row = stagesTable.AppendChild(new TableRow());
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.StageName)))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.HitFactor.ToString("F4"))))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.StageTime.ToString(@"ss\.ff"))))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text($"A:{stage.AlphasCount} C:{stage.CharliesCount} D:{stage.DeltasCount} M:{stage.MissesCount}")))));
                                }
                            }
                            else
                            {
                                body.AppendChild(new Paragraph(new Run(new Text("Нет данных об этапах"))));
                            }

                            body.AppendChild(new Paragraph(new Run(new Text(""))));
                        }
                    }

                    if (trainings.Any())
                    {
                        var trainingsHeading = body.AppendChild(new Paragraph());
                        trainingsHeading.AppendChild(new Run(new Text("Тренировки")));
                        trainingsHeading.ParagraphProperties = new ParagraphProperties(
                            new RunProperties(new Bold(), new Underline() { Val = UnderlineValues.Single }),
                            new SpacingBetweenLines() { Before = "400", After = "200" }
                        );

                        foreach (var training in trainings)
                        {
                            var trainingParagraph = body.AppendChild(new Paragraph());
                            trainingParagraph.AppendChild(new Run(new Text($"{training.TypeOfTraining} - {training.TrainingDate.ToString("dd.MM.yyyy HH:mm")}")));
                            trainingParagraph.ParagraphProperties = new ParagraphProperties(
                                new RunProperties(new Bold()),
                                new SpacingBetweenLines() { Before = "200", After = "100" }
                            );

                            if (training.Stages.Any())
                            {
                                var stagesTable = body.AppendChild(new Table());

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
                                stagesTable.AppendChild(tableProperties);

                                var headerRow = stagesTable.AppendChild(new TableRow());
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Название этапа")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Тип")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Хит-фактор")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Время")) { RunProperties = new RunProperties(new Bold()) })));
                                headerRow.AppendChild(new TableCell(new Paragraph(new Run(new Text("Результат")) { RunProperties = new RunProperties(new Bold()) })));

                                foreach (var stage in training.Stages)
                                {
                                    var row = stagesTable.AppendChild(new TableRow());
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.StageName)))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.StageType)))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.HitFactor.ToString("F2"))))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text(stage.StageTime.ToString(@"ss\.ff"))))));
                                    row.AppendChild(new TableCell(new Paragraph(new Run(new Text($"A:{stage.AlphasCount} C:{stage.CharliesCount} D:{stage.DeltasCount} M:{stage.MissesCount}")))));
                                }
                            }
                            else
                            {
                                body.AppendChild(new Paragraph(new Run(new Text("Нет данных об этапах"))));
                            }

                            body.AppendChild(new Paragraph(new Run(new Text(""))));
                        }
                    }

                    if (!matches.Any() && !trainings.Any())
                    {
                        body.AppendChild(new Paragraph(new Run(new Text("Нет данных о событиях в календаре"))));
                    }

                    wordDocument.Save();
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Отчет о календаре_{DateTime.Now}.docx");
            }
        }

    }
}