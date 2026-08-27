using AIPlacement.Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Application.Jobs.Services
{
    public interface IJobEligibleBranchService
    {
        Task<IEnumerable<JobEligibleBranch>> GetByJobDriveIdAsync(int jobDriveId);
        Task AddAsync(JobEligibleBranch branch);
        Task DeleteAsync(int jobBranchId);
    }
}
