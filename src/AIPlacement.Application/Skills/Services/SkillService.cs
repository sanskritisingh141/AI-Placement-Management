using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Skills.Services;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;

    public SkillService(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<IReadOnlyList<SkillDto>> GetAllAsync()
    {
    return await _skillRepository.GetAllAsync();
    }

    public async Task<SkillDto?> GetByIdAsync(int skillId)
    {
        return await _skillRepository.GetByIdAsync(skillId);
    }

    public async Task<IEnumerable<SkillDto>> GetByStudentIdAsync(int studentId)
    {
        return await _skillRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<SkillDto> CreateAsync(SkillDto skill)
    {
        var entity = new Skill
        {
            SkillName = skill.SkillName
        };

        var studentSkill = new StudentSkill
        {
            StudentId = skill.StudentId,
            ProficiencyLevel = skill.ProficiencyLevel
        };

        return await _skillRepository.CreateAsync(
            entity,
            studentSkill);
    }

    public async Task<SkillDto?> UpdateAsync(
        int skillId,
        SkillDto skill)
    {
        var entity = new Skill
        {
            SkillId = skillId,
            SkillName = skill.SkillName
        };

        var studentSkill = new StudentSkill
        {
            StudentId = skill.StudentId,
            SkillId = skillId,
            ProficiencyLevel = skill.ProficiencyLevel
        };

        return await _skillRepository.UpdateAsync(
            skillId,
            entity,
            studentSkill);
    }

    public async Task<bool> DeleteAsync(int studentId, int skillId)
    {
        return await _skillRepository.DeleteStudentSkillAsync(studentId, skillId);
    }
}
