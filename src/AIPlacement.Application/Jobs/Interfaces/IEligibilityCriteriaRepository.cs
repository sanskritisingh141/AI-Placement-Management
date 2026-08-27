using AIPlacement.Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Application.Jobs.Interfaces
{
    public interface IEligibilityCriteriaRepository
    {
        Task<EligibilityCriteria?> GetByJobDriveIdAsync(int jobDriveId);
        Task<EligibilityCriteria?> GetByIdAsync(int eligibilityId);
        Task AddAsync(EligibilityCriteria eligibilityCriteria);
        Task UpdateAsync(EligibilityCriteria eligibilityCriteria);
        Task DeleteAsync(int eligibilityId);
    }
}
