using AbmProductos.Models.DTOs.Requests;
using AbmProductos.Models.DTOs.Responses;
using AbmProductos.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AbmProductos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<ProductForReadDto>> GetAll()
        {
            return Ok(_service.GetAllProducts());
        }

        [HttpGet("{id}")]
        public ActionResult<ProductForReadDto> GetById(int id)
        {
            var product = _service.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("search")]
        public ActionResult<List<ProductForReadDto>> Search([FromQuery] string name)
        {
            return Ok(_service.SearchProductsByName(name));
        }

        [HttpGet("stats")]
        public ActionResult<ProductStatsDto> GetStats()
        {
            return Ok(_service.GetStats());
        }

        [HttpPost]
        public ActionResult<ProductForReadDto> Create([FromBody] ProductForCreateDto dto)
        {
            try
            {
                var created = _service.CreateProduct(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductForUpdateDto dto)
        {
            if (_service.GetProductById(id) == null)
            {
                return NotFound();
            }

            try
            {
                _service.UpdateProduct(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_service.GetProductById(id) == null)
            {
                return NotFound();
            }

            _service.DeleteProduct(id);
            return NoContent();
        }
    }
}
