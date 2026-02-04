using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using WorkBeast.Core.Models;
using WorkBeast.Data.Context;

namespace WorkBeast.AdminConsole.Commands;

public class DataCommands
{
    private readonly AppDbContext _context;

    public DataCommands(AppDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Data Management:[/]")
                .AddChoices(new[]
                {
                    "List Exercises",
                    "List Body Parts",
                    "Add Custom Exercise",
                    "Add Custom Body Part",
                    "View Workout Sessions",
                    "Delete Non-System Data",
                    "Export Data to JSON",
                    "Back to Main Menu"
                }));

        switch (choice)
        {
            case "List Exercises":
                await ListExercises();
                break;
            case "List Body Parts":
                await ListBodyParts();
                break;
            case "Add Custom Exercise":
                await AddCustomExercise();
                break;
            case "Add Custom Body Part":
                await AddCustomBodyPart();
                break;
            case "View Workout Sessions":
                await ViewWorkoutSessions();
                break;
            case "Delete Non-System Data":
                await DeleteNonSystemData();
                break;
            case "Export Data to JSON":
                await ExportDataToJson();
                break;
        }
    }

    private async Task ListExercises()
    {
        await AnsiConsole.Status()
            .StartAsync("Loading exercises...", async ctx =>
            {
                var exercises = await _context.Exercises
                    .Include(e => e.TargetedBodyParts)
                    .Where(e => !e.IsDeleted)
                    .OrderBy(e => e.Name)
                    .ToListAsync();

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("System");
                table.AddColumn("Body Parts");

                foreach (var exercise in exercises)
                {
                    table.AddRow(
                        exercise.Oid.ToString(),
                        exercise.Name,
                        exercise.IsSystem ? "[green]Yes[/]" : "No",
                        string.Join(", ", exercise.TargetedBodyParts.Select(bp => bp.Name))
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"\n[grey]Total: {exercises.Count} exercises[/]");
            });
    }

    private async Task ListBodyParts()
    {
        await AnsiConsole.Status()
            .StartAsync("Loading body parts...", async ctx =>
            {
                var bodyParts = await _context.BodyParts
                    .Where(bp => !bp.IsDeleted)
                    .OrderBy(bp => bp.Name)
                    .ToListAsync();

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("Description");
                table.AddColumn("System");

                foreach (var bodyPart in bodyParts)
                {
                    table.AddRow(
                        bodyPart.Oid.ToString(),
                        bodyPart.Name,
                        bodyPart.Description.Length > 50 
                            ? bodyPart.Description.Substring(0, 47) + "..." 
                            : bodyPart.Description,
                        bodyPart.IsSystem ? "[green]Yes[/]" : "No"
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"\n[grey]Total: {bodyParts.Count} body parts[/]");
            });
    }

    private async Task AddCustomExercise()
    {
        var name = AnsiConsole.Ask<string>("Enter exercise name:");
        var description = AnsiConsole.Ask<string>("Enter description:");

        var bodyParts = await _context.BodyParts.Where(bp => !bp.IsDeleted).ToListAsync();
        var selectedBodyParts = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select targeted body parts:")
                .Required()
                .PageSize(10)
                .AddChoices(bodyParts.Select(bp => bp.Name)));

        var targetedBodyParts = bodyParts
            .Where(bp => selectedBodyParts.Contains(bp.Name))
            .ToList();

        var exercise = new Exercise
        {
            Name = name,
            Description = description,
            IsSystem = false,
            TargetedBodyParts = targetedBodyParts,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        AnsiConsole.MarkupLine($"[green]Exercise '{name}' created successfully! ID: {exercise.Oid}[/]");
    }

    private async Task AddCustomBodyPart()
    {
        var name = AnsiConsole.Ask<string>("Enter body part name:");
        var description = AnsiConsole.Ask<string>("Enter description:");

        var bodyPart = new BodyPart
        {
            Name = name,
            Description = description,
            IsSystem = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.BodyParts.AddAsync(bodyPart);
        await _context.SaveChangesAsync();

        AnsiConsole.MarkupLine($"[green]Body part '{name}' created successfully! ID: {bodyPart.Oid}[/]");
    }

    private async Task ViewWorkoutSessions()
    {
        await AnsiConsole.Status()
            .StartAsync("Loading workout sessions...", async ctx =>
            {
                var sessions = await _context.WorkoutSessions
                    .Include(ws => ws.LoggedExercises)
                    .Where(ws => !ws.IsDeleted)
                    .OrderByDescending(ws => ws.Date)
                    .Take(20)
                    .ToListAsync();

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Date");
                table.AddColumn("Exercises");
                table.AddColumn("Notes");

                foreach (var session in sessions)
                {
                    table.AddRow(
                        session.Oid.ToString(),
                        session.Date.ToString("yyyy-MM-dd HH:mm"),
                        session.LoggedExercises.Count.ToString(),
                        session.Notes.Length > 30 
                            ? session.Notes.Substring(0, 27) + "..." 
                            : session.Notes
                    );
                }

                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"\n[grey]Showing last 20 sessions (Total: {await _context.WorkoutSessions.CountAsync(ws => !ws.IsDeleted)})[/]");
            });
    }

    private async Task DeleteNonSystemData()
    {
        AnsiConsole.MarkupLine("[red]WARNING: This will delete all non-system exercises and body parts![/]");
        var confirm = AnsiConsole.Confirm("Are you sure?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Deleting non-system data...", async ctx =>
            {
                var customExercises = await _context.Exercises
                    .Where(e => !e.IsSystem && !e.IsDeleted)
                    .ToListAsync();

                var customBodyParts = await _context.BodyParts
                    .Where(bp => !bp.IsSystem && !bp.IsDeleted)
                    .ToListAsync();

                foreach (var exercise in customExercises)
                {
                    exercise.IsDeleted = true;
                    exercise.UpdatedAt = DateTime.UtcNow;
                }

                foreach (var bodyPart in customBodyParts)
                {
                    bodyPart.IsDeleted = true;
                    bodyPart.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                AnsiConsole.MarkupLine($"[green]Deleted {customExercises.Count} exercises and {customBodyParts.Count} body parts[/]");
            });
    }

    private async Task ExportDataToJson()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var filename = $"workbeast_export_{timestamp}.json";

        await AnsiConsole.Status()
            .StartAsync("Exporting data...", async ctx =>
            {
                try
                {
                    var data = new
                    {
                        ExportDate = DateTime.UtcNow,
                        Exercises = await _context.Exercises
                            .Include(e => e.TargetedBodyParts)
                            .Where(e => !e.IsDeleted)
                            .Select(e => new
                            {
                                e.Oid,
                                e.Name,
                                e.Description,
                                e.IsSystem,
                                BodyParts = e.TargetedBodyParts.Select(bp => bp.Name)
                            })
                            .ToListAsync(),
                        BodyParts = await _context.BodyParts
                            .Where(bp => !bp.IsDeleted)
                            .Select(bp => new
                            {
                                bp.Oid,
                                bp.Name,
                                bp.Description,
                                bp.IsSystem
                            })
                            .ToListAsync(),
                        WorkoutSessions = await _context.WorkoutSessions
                            .Where(ws => !ws.IsDeleted)
                            .Select(ws => new
                            {
                                ws.Oid,
                                ws.Date,
                                ws.Notes,
                                ExerciseCount = ws.LoggedExercises.Count
                            })
                            .ToListAsync()
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    await File.WriteAllTextAsync(filename, json);
                    AnsiConsole.MarkupLine($"[green]Data exported successfully to: {filename}[/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error exporting data: {ex.Message}[/]");
                }
            });
    }

    // Non-interactive mode support for testing
    public async Task ExecuteDirectAsync(string command)
    {
        switch (command)
        {
            case "List Exercises":
                await ListExercises();
                break;
            case "List Body Parts":
                await ListBodyParts();
                break;
            case "View Workout Sessions":
                await ViewWorkoutSessions();
                break;
            default:
                AnsiConsole.MarkupLine($"[red]Unknown command: {command}[/]");
}
