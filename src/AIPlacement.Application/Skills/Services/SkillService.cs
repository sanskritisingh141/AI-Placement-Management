using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;

namespace AIPlacement.Application.Skills.Services;

public class SkillService : ISkillService
{
    public Task<SkillDto?> GetByIdAsync(int skillId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SkillDto>> GetByStudentIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<SkillDto> CreateAsync(SkillDto skill)
    {
        throw new NotImplementedException();
    }

    public Task<SkillDto?> UpdateAsync(int skillId, SkillDto skill)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int skillId)
    {
        throw new NotImplementedException();
    }
}