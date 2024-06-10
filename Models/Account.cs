using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MultiAccountMVP.Models;

public class Account : IdentityUser<long>
{
    [MaxLength(50)] public string HumanName { get; init; } = "Ivan";
}