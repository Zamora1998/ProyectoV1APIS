using DataCliente;
using System.Linq;
using System.Web.Http;
using ProyectoV1.Models;

namespace ProyectoV1.Controllers
{
    public class NEWEstadoController : ApiController
    {
        private tiusr30pl_PeliculasProgra11Entities EstadosNew = new tiusr30pl_PeliculasProgra11Entities();

        public NEWEstadoController()
        {
            EstadosNew.Configuration.LazyLoadingEnabled = true;
            EstadosNew.Configuration.ProxyCreationEnabled = true;
        }

        // GET: api/GetEstado/{estadoId}
        [HttpGet]
        [Route("api/GetEstado/{estadoId}")]
        public IHttpActionResult GetEstadoNew(int estadoId)
        {
            // Buscar el estado en la base de datos por su ID
            var estado = EstadosNew.Estado.FirstOrDefault(e => e.EstadoID == estadoId);

            if (estado == null)
            {
                // Si el estado no se encuentra, devolver una respuesta 404 (Not Found)
                return NotFound();
            }

            // Devolver el nombre del estado encontrado
            return Ok(estado.NombreEstado);
        }
    }
}
