namespace Server.Entities;

public class RoleEntity
{
    public required Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}