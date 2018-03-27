using System.Collections.Generic;
using grimtol_checkpoint.Models;

namespace grimtol_checkpoint.Interfaces
{
    public interface IItem
    {
        string Name { get; set; }
        string Description { get; set; }
    }
}