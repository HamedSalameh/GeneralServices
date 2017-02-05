namespace GeneralServices.Interfaces
{
    public interface IEntity
    {
        int Id { get; set; }

        EntityState EntityState { get; set; }
    }

    public enum EntityState
    {
        Unchanged,
        Added,
        Modified,
        Deleted
    }
}
