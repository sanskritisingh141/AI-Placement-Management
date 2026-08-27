using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPlacement.Application.Jobs.Services
{
    public class EligibilityCriteriaService : IEligibilityCriteriaService
    {
        private readonly IEligibilityCriteriaRepository _repository;

        public EligibilityCriteriaService(IEligibilityCriteriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<EligibilityCriteria?> GetByJobDriveIdAsync(int jobDriveId)
        {
            return await _repository.GetByJobDriveIdAsync(jobDriveId);
        }

        public async Task AddAsync(EligibilityCriteria eligibilityCriteria)
        {
            // Business rule: only one EligibilityCriteria per JobDrive
            var existing = await _repository.GetByJobDriveIdAsync(eligibilityCriteria.JobDriveId);
            if (existing != null)
                throw new InvalidOperationException("EligibilityCriteria already exists for this JobDrive.");

            await _repository.AddAsync(eligibilityCriteria);
        }

        public async Task UpdateAsync(EligibilityCriteria eligibilityCriteria)
        {
            await _repository.UpdateAsync(eligibilityCriteria);
        }

        public async Task DeleteAsync(int eligibilityId)
        {
            await _repository.DeleteAsync(eligibilityId);
        }
    }
}
