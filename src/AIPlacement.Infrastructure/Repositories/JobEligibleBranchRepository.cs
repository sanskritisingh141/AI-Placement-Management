using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Infrastructure.Repositories
{
    public class JobEligibleBranchRepository : IJobEligibleBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public JobEligibleBranchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobEligibleBranch>> GetByJobDriveIdAsync(int jobDriveId)
        {
            return await _context.JobEligibleBranches
                .Where(b => b.JobDriveId == jobDriveId)
                .ToListAsync();
        }

        public async Task<JobEligibleBranch?> GetByIdAsync(int jobBranchId)
        {
            return await _context.JobEligibleBranches
                .FirstOrDefaultAsync(b => b.JobBranchId == jobBranchId);
        }

        public async Task AddAsync(JobEligibleBranch branch)
        {
            _context.JobEligibleBranches.Add(branch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int jobBranchId)
        {
            var entity = await _context.JobEligibleBranches.FindAsync(jobBranchId);
            if (entity != null)
            {
                _context.JobEligibleBranches.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
