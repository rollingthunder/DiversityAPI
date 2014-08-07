using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiversityService.API.Model
{   
    public interface IGuidIdentifiable
    {
        Guid RowGUID { get; set; }
    }

    public interface IIdentifiable
    {
        int Id { get; }
    }
}
