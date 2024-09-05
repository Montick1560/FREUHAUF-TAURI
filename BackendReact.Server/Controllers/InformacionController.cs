using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NEGOCIO.DATAACCESS;
using NEGOCIO.MODELO;
using System.Runtime.InteropServices;

namespace BackendReact.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformacionController : ControllerBase
    {
        private readonly DAL _dal;
        [DllImport("msxml2.dll")]
        private static extern void XMLHTTPCreate(out object ppObject);
        public InformacionController(DAL dal)
        {
            _dal = dal;
        }
        [HttpPost]
        [Route("Estacion")]
        public IActionResult Estacion()
        {
            List<cEstacion> lstEstacion = new List<cEstacion>();
            string sError = string.Empty;
            if (_dal.SeleccionaClienteTenedor(ref sError, lstEstacion))
            {
                return Ok(lstEstacion);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
