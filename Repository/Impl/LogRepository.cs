using System.Collections.Immutable;
using FluentResults;
using LedBlinker.Data;
using LedBlinker.Models;
using Microsoft.EntityFrameworkCore;

namespace LedBlinker.Repository.Impl
{
    public class LogRepository : ILogRepo
    {

        private readonly ApplicationDbContext _db;

        public LogRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Logs> AddLogAsync(Logs log)
        {

            /* _db.Logs.Add(new Logs
            {
                Date = DateTime.Now,
                State = dto.State.ToString()
            });*/

            var result = _db.Logs.Add(log);
            await _db.SaveChangesAsync();

            return result.Entity; // vrátí přidaný log
            
        }

        public async Task<List<Logs>> GetAllLogsAsync()
        {
            return await _db.Logs.ToListAsync();

        }

        
    }
}
