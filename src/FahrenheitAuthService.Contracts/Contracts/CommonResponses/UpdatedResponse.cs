namespace FahrenheitAuthService.Contracts.Contracts.CommonResponses;

public class UpdatedResponse
{
    public UpdatedResponse(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}