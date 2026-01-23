using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Repositories
{
    public  interface IGenericRepository<T> where T : BaseModel
    {
        
    }
}
