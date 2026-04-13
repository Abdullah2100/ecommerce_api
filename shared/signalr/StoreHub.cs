using api.Presentation.dto;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.SignalR;

namespace api.shared.signalr;

public class StoreHub : Hub
{
    public async Task UpdatingStoreStatus(StoreStatusDto status)
    {
        await Clients.All.SendAsync("storeStatus", status);
    }
}