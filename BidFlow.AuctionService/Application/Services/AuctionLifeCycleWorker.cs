using BidFlow.AuctionService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BidFlow.AuctionService.Application.Services
{
    public class AuctionLifeCycleWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuctionLifeCycleWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
            do
            {
                try
                {
                    await UpdateAuctionStatusesAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Auction lifecycle error:{ex.Message}");
                }
            }
            while (await timer.WaitForNextTickAsync(cancellationToken));
        }
        private async Task UpdateAuctionStatusesAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
            var now = DateTime.UtcNow;
            var activatedCount =await context.Auctions.Where(a =>
            a.status == Domain.AuctionTypesEnums.Scheduled &&
            a.StartingTime <= now &&
            a.EndTime > now).ExecuteUpdateAsync(setters => setters.SetProperty(a => a.status, Domain.AuctionTypesEnums.Open), cancellationToken);

            Console.WriteLine($"Activated auctions: {activatedCount}");

            var completedCount = await context.Auctions.Where(a =>
            (a.status == Domain.AuctionTypesEnums.Open ||
            a.status == Domain.AuctionTypesEnums.Scheduled) &&
            a.EndTime <= now).ExecuteUpdateAsync(setters => setters.SetProperty(a => a.status, Domain.AuctionTypesEnums.Completed), cancellationToken);
            Console.WriteLine($"Completed auctions: {completedCount}");
        }
    }
}
