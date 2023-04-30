using System.ComponentModel.DataAnnotations;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.AboutMe.Components.Skill;

public sealed class AddSkillModel
{
    [Required]
    public string Skill { get; set; }

    public string ImageUrl { get; set; }

    [Required]
    public string Proficiency { get; set; } = ProficiencyLevel.Familiar.Key;

    [Required]
    public string Capability { get; set; }
}