using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Runic.RuneDbApi.Controllers
{
    [Route("api/[controller]")]
    public class RunesController : Controller
    {
        // POST api/runes
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/runes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/runes/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
