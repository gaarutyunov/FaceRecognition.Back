using System.Threading.Tasks;
using FaceRecognition.Back.Api.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FaceRecognition.Back.Api.Hubs
{
    public class QrHub : Hub<IQrClient>
    {
        private readonly IQrService _qrService;

        public QrHub(IQrService qrService)
        {
            _qrService = qrService;
        }

        public async Task GenerateQrUrl(string url)
        {
            var qrUrl = _qrService.GenerateQrUrl(url);
            await Clients.Caller.GenerateQrUrl(qrUrl);
        }
    }
}