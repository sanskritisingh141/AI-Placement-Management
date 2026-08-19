using AIPlacement.Application.Skills.DTOs;

namespace AIPlacement.Application.Skills.Interfaces;

public interface ISkillService
{
    Task<SkillDto?> GetByIdAsync(int skillId);

    Task<IEnumerable<SkillDto>> GetByStudentIdAsync(int studentId);

    Task<SkillDto> CreateAsync(SkillDto skill);

    Task<SkillDto?> UpdateAsync(int skillId, SkillDto skill);

    Task<bool> DeleteAsync(int skillId);
}