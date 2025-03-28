﻿using EMS.Helpers;
using System.ComponentModel.DataAnnotations;

namespace EMS.DTOs
{
    public class GenerateTokenDTO
    {
        public string Id { get; set; }
        [Required]
        [ValidEmailHelper]
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
