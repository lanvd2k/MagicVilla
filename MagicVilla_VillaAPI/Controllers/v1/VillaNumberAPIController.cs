using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        protected APIResponse _respone;
        public VillaNumberAPIController(IMapper mapper, IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;
            _respone = new();
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
                _respone.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                _respone.StatusCode = HttpStatusCode.OK;
                return Ok(_respone);
            }
            catch (Exception ex)
            {
                _respone.IsSuccess = false;
                _respone.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _respone;
        }




        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _respone.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_respone);
                }
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
                    _respone.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_respone);
                }
                _respone.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _respone.StatusCode = HttpStatusCode.OK;
                return Ok(_respone);
            }
            catch (Exception ex)
            {
                _respone.IsSuccess = false;
                _respone.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _respone;
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (await _dbVillaNumber.GetAsync(v => v.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa number already exists!");
                    return BadRequest(ModelState);
                }
                if (await _dbVilla.GetAsync(v => v.Id == createDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Id is invalid!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

                await _dbVillaNumber.CreateAsync(villaNumber);

                _respone.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _respone.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _respone);
            }
            catch (Exception ex)
            {
                _respone.IsSuccess = false;
                _respone.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _respone;
        }

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == id);
                if (villaNumber == null)
                {
                    return NotFound();
                }
                await _dbVillaNumber.RemoveAsync(villaNumber);
                _respone.StatusCode = HttpStatusCode.NoContent;
                _respone.IsSuccess = true;
                return Ok(_respone);
            }
            catch (Exception ex)
            {
                _respone.IsSuccess = false;
                _respone.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _respone;
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.VillaNo != id)
                {
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(v => v.Id == updateDTO.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Id is invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);

                await _dbVillaNumber.UpdateAsync(villaNumber);
                _respone.StatusCode = HttpStatusCode.NoContent;
                _respone.IsSuccess = true;

                return Ok(_respone);
            }
            catch (Exception ex)
            {
                _respone.IsSuccess = false;
                _respone.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _respone;
        }


    }
}
