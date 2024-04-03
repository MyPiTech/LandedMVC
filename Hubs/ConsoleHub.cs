using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Hubs
{
    public class ConsoleHub : Hub<IConsoleHub>
    {
       
        public async Task SendLogAsync(object? data)
        {
            await Clients.All.SendLogAsync(data ?? string.Empty);
        }
        public async Task SendInfoAsync(object? data)
        {
            await Clients.All.SendInfoAsync(data ?? string.Empty);
        }
        public async Task SendWarnAsync(object? data)
        {
            await Clients.All.SendWarnAsync(data ?? string.Empty);
        }
        public async Task SendErrorAsync(object? data)
        {
            await Clients.All.SendErrorAsync(data ?? string.Empty);
        }
    }
}
