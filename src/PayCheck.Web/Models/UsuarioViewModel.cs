namespace PayCheck.Web.Models;

using System;
using System.ComponentModel.DataAnnotations;

public class UsuarioViewModel
{
    [Key]
    public Guid? Guid { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }
}