using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Infrastructure.Repositories
{
    public class EligibilityCriteriaRepository : IEligibilityCriteriaRepository
    {
        private readonly ApplicationDbContext _context;

        public EligibilityCriteriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EligibilityCriteria?> GetByJobDriveIdAsync(int jobDriveId)
        {
            return await _context.EligibilityCriterias
                .FirstOrDefaultAsync(e => e.JobDriveId == jobDriveId);
        }

        public async Task<EligibilityCriteria?> GetByIdAsync(int eligibilityId)
        {
            return await _context.EligibilityCriterias
                .FirstOrDefaultAsync(e => e.EligibilityId == eligibilityId);
        }

        public async Task AddAsync(EligibilityCriteria eligibilityCriteria)
        {
            _context.EligibilityCriterias.Add(eligibilityCriteria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EligibilityCriteria eligibilityCriteria)
        {
            _context.EligibilityCriterias.Update(eligibilityCriteria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int eligibilityId)
        {
            var entity = await _context.EligibilityCriterias.FindAsync(eligibilityId);
            if (entity != null)
            {
                _context.EligibilityCriterias.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
