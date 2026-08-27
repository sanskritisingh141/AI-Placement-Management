using AIPlacement.Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Application.Jobs.Services
{
    public interface IEligibilityCriteriaService
    {
        Task<EligibilityCriteria?> GetByJobDriveIdAsync(int jobDriveId);
        Task AddAsync(EligibilityCriteria eligibilityCriteria);
        Task UpdateAsync(EligibilityCriteria eligibilityCriteria);
        Task DeleteAsync(int eligibilityId);
    }
}
