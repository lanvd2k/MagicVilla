using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;

namespace MagicVilla_Web.Controllers
{
    public class VillaNumberController : Controller
    {
        private readonly IVillaNumberService _villaNumberService;
        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
        {
            _villaNumberService = villaNumberService;
            _mapper = mapper;
            _villaService = villaService;
        }
        public async Task<IActionResult> IndexVillaNumber()
        {
            List<VillaNumberDTO> list = new();
            var respone = await _villaNumberService.GetAllAsync<APIRespone>(HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(respone.Result));
            }
            return View(list);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
        {
            VillaNumberCreateVM villaNumberCreateVM = new();
            var respone = await _villaService.GetAllAsync<APIRespone>(HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                villaNumberCreateVM.VillaList = JsonConvert
                    .DeserializeObject<List<VillaDTO>>(Convert.ToString(respone.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(villaNumberCreateVM);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var respone = await _villaNumberService.CreateAsync<APIRespone>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if (respone != null && respone.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (respone.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", respone.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var resp = await _villaService.GetAllAsync<APIRespone>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.IsSuccess)
            {
                model.VillaList = JsonConvert
                    .DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
        {
            VillaNumberUpdateVM villaNumberUpdateVM = new();
            var respone = await _villaNumberService.GetAsync<APIRespone>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(respone.Result));
                villaNumberUpdateVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
            }

            respone = await _villaService.GetAllAsync<APIRespone>(HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                villaNumberUpdateVM.VillaList = JsonConvert
                    .DeserializeObject<List<VillaDTO>>(Convert.ToString(respone.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(villaNumberUpdateVM);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var respone = await _villaNumberService.UpdateAsync<APIRespone>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
                if (respone != null && respone.IsSuccess)
                {
                    return RedirectToAction(nameof(IndexVillaNumber));
                }
                else
                {
                    if (respone.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", respone.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var resp = await _villaService.GetAllAsync<APIRespone>(HttpContext.Session.GetString(SD.SessionToken));
            if (resp != null && resp.IsSuccess)
            {
                model.VillaList = JsonConvert
                    .DeserializeObject<List<VillaDTO>>(Convert.ToString(resp.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
        {
            VillaNumberDeleteVM villaNumberUpdateVM = new();
            var respone = await _villaNumberService.GetAsync<APIRespone>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(respone.Result));
                villaNumberUpdateVM.VillaNumber = model;
            }

            respone = await _villaService.GetAllAsync<APIRespone>(HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                villaNumberUpdateVM.VillaList = JsonConvert
                    .DeserializeObject<List<VillaDTO>>(Convert.ToString(respone.Result))
                    .Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    });
                return View(villaNumberUpdateVM);
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
        {
            var respone = await _villaNumberService.DeleteAsync<APIRespone>(model.VillaNumber.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
            if (respone != null && respone.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }
            return View(model);
        }
    }
}
