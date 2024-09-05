using Microsoft.AspNetCore.SignalR;

namespace BackendReact.Server.HUB
{
    public class EstacionHUB : Hub
    {
        public async Task EnviarCambiosEstatus(int idEstacion, int nuevoEstatus)
        {
            await Clients.All.SendAsync("RecibirCambiosEstatus", idEstacion, nuevoEstatus);
        }
    }
}
