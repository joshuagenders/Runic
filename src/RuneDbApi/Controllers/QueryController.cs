using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RuneDbApi.Controllers
{
    [Route("api/[controller]")]
    public class QueryController : Controller
    {
        // POST api/query
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/query/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/query/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
