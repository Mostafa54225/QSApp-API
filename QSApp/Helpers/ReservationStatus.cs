using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QSApp.DataContext;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QSApp.Helpers
{
    public class ReservationStatus : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory scopeFactory;

        private Timer timer;
        public ReservationStatus(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }
        public static async Task ChangeReservationStatus(QsDbContext _context)
        {

            var today = DateTime.Today.Date;
            try
            {
                var reservations = await _context.Reservations.Where(r => r.DateTime == today && r.StatusId == 1).ToListAsync();
                reservations.ForEach(r =>
                {
                    r.StatusId = 3;
                });
                await _context.SaveChangesAsync();
            }
            catch (IOException e)
            {
                Console.WriteLine($"The file could not be opened: '{e}'");
            }

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            timer = new Timer(async o =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<QsDbContext>();
                    await ChangeReservationStatus(dbContext);
                }
                //Interlocked.Increment(ref number);
                //logger.LogInformation($"printing from worker {number}");
            }, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //logger.LogInformation("Printing Worker Stopped");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
