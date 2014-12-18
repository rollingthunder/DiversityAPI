namespace DiversityService.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;

    public interface IFieldDataController<TEntityBindingModel>
    {
        Task<IHttpActionResult> Get();

        Task<IHttpActionResult> Get(int id);

        Task<IHttpActionResult> Post(TEntityBindingModel value);
    }
}