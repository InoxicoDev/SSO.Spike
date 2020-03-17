using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace InoxicoIdentity.Controllers
{
    public class ReferenceCodeController : ApiController
    {
        [HttpPut]
        public async Task<string> Create()
        {
            return "1234";
        }
    }
}