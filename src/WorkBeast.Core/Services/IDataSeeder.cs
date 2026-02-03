namespace WorkBeast.Core.Services;

/// <summary>
/// Interface for seeding initial data into the database
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Seeds the database with initial data including system exercises and body parts
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task SeedAsync();
}
