namespace WorkBeast.Core.Models;

public class Exercise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<BodyPart> TargetedBodyParts { get; set; } = new();
}

public class BodyPart : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Exercise> Exercises { get; set; } = new();
}

public class ExerciseBodyPart
{
    public int ExerciseOid { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int BodyPartOid { get; set; }
    public BodyPart BodyPart { get; set; } = null!;
}