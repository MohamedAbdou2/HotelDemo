using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    public class GenericRepository<T>:IGenericRepository<T> where T : BaseModel
    {
    }
}
