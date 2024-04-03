namespace LandedMVC.Hubs
{
    public interface IConsoleHub
    {
        Task SendLogAsync(params object[]? data);
        Task SendInfoAsync(params object[]? data);
        Task SendWarnAsync(params object[]? data);
        Task SendErrorAsync(params object[]? data);

    }
}
