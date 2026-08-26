using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Skills;

public class SkillRepository : ISkillRepository
{
    private readonly ApplicationDbContext _context;

    public SkillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SkillDto?> GetByIdAsync(int skillId)
    {
        return await (
            from studentSkill in _context.StudentSkills
            join skill in _context.Skills
                on studentSkill.SkillId equals skill.SkillId
            where skill.SkillId == skillId
            select new SkillDto
            {
                SkillId = skill.SkillId,
                StudentId = studentSkill.StudentId,
                SkillName = skill.SkillName,
                ProficiencyLevel = studentSkill.ProficiencyLevel ?? string.Empty
            }
        ).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SkillDto>> GetByStudentIdAsync(int studentId)
    {
        return await (
            from studentSkill in _context.StudentSkills
            join skill in _context.Skills
                on studentSkill.SkillId equals skill.SkillId
            where studentSkill.StudentId == studentId
            select new SkillDto
            {
                SkillId = skill.SkillId,
                StudentId = studentSkill.StudentId,
                SkillName = skill.SkillName,
                ProficiencyLevel = studentSkill.ProficiencyLevel ?? string.Empty
            }
        ).ToListAsync();
    }

    public async Task<SkillDto> CreateAsync(
    Skill skill,
    StudentSkill studentSkill)
    {
        var existingSkill = await _context.Skills
            .FirstOrDefaultAsync(x => x.SkillName == skill.SkillName);

        if (existingSkill != null)
        {
            studentSkill.SkillId = existingSkill.SkillId;
            skill = existingSkill;
        }
        else
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            studentSkill.SkillId = skill.SkillId;
        }

        _context.StudentSkills.Add(studentSkill);

        await _context.SaveChangesAsync();

        return new SkillDto
        {
            SkillId = skill.SkillId,
            StudentId = studentSkill.StudentId,
            SkillName = skill.SkillName,
            ProficiencyLevel = studentSkill.ProficiencyLevel ?? string.Empty
        };
    }

    public async Task<SkillDto?> UpdateAsync(
        int skillId,
        Skill skill,
        StudentSkill studentSkill)
    {
        var existingSkill = await _context.Skills
            .FirstOrDefaultAsync(x => x.SkillId == skillId);

        if (existingSkill == null)
            return null;

        var existingStudentSkill = await _context.StudentSkills
            .FirstOrDefaultAsync(x =>
                x.SkillId == skillId &&
                x.StudentId == studentSkill.StudentId);

        if (existingStudentSkill == null)
            return null;

        existingSkill.SkillName = skill.SkillName;
        existingSkill.Category = skill.Category;

        existingStudentSkill.ProficiencyLevel =
            studentSkill.ProficiencyLevel;

        await _context.SaveChangesAsync();

        return new SkillDto
        {
            SkillId = existingSkill.SkillId,
            StudentId = existingStudentSkill.StudentId,
            SkillName = existingSkill.SkillName,
            ProficiencyLevel =
                existingStudentSkill.ProficiencyLevel ?? string.Empty
        };
    }

    public async Task<bool> DeleteAsync(int skillId)
    {
        var studentSkills = await _context.StudentSkills
            .Where(x => x.SkillId == skillId)
            .ToListAsync();

        var skill = await _context.Skills
            .FirstOrDefaultAsync(x => x.SkillId == skillId);

        if (skill == null)
            return false;

        _context.StudentSkills.RemoveRange(studentSkills);
        _context.Skills.Remove(skill);

        await _context.SaveChangesAsync();

        return true;
    }
}