using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MultiAccountMVP.Models;

public class DevAccount : IdentityUser<long>
{
    [MaxLength(50)] public string DevName { get; init; } = "xd";
}