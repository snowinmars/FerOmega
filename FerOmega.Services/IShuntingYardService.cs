using System.Collections.Generic;
using FerOmega.Entities;

namespace FerOmega.Services
{
    public interface IShuntingYardService<T>
    {
        T Parse(string equation);
    }
}