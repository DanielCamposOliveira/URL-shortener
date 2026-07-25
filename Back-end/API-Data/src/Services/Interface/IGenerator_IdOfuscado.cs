namespace API_Data.src.Services.Interface
{
    public interface IGenerator_IdOfuscado
    {
        Task<IdGeneratorResponse?> GenerateIdAsync();
    }
}
