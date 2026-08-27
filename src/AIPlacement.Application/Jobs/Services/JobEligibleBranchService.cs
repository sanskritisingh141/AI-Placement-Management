using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Application.Jobs.Services
{
    public class JobEligibleBranchService : IJobEligibleBranchService
    {
        private readonly IJobEligibleBranchRepository _repository;

        public JobEligibleBranchService(IJobEligibleBranchRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<JobEligibleBranch>> GetByJobDriveIdAsync(int jobDriveId)
        {
            return await _repository.GetByJobDriveIdAsync(jobDriveId);
        }

        public async Task AddAsync(JobEligibleBranch branch)
        {
            await _repository.AddAsync(branch);
        }

        public async Task DeleteAsync(int jobBranchId)
        {
            await _repository.DeleteAsync(jobBranchId);
        }
    }
}
