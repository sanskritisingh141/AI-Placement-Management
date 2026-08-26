using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Skills.Interfaces;

public interface ISkillRepository
{
    Task<SkillDto?> GetByIdAsync(int skillId);

    Task<IEnumerable<SkillDto>> GetByStudentIdAsync(int studentId);

    Task<SkillDto> CreateAsync(
        Skill skill,
        StudentSkill studentSkill);

    Task<SkillDto?> UpdateAsync(
        int skillId,
        Skill skill,
        StudentSkill studentSkill);

    Task<bool> DeleteAsync(int skillId);
}