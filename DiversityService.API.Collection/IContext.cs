using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.WebHost
{    

    public interface IIdentifiable
    {
        int Id { get; }
    }
}
