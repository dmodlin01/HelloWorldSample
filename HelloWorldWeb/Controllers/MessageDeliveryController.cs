using AutoMapper;
using DTOs;
using HelloWorldWeb.Models;
using HelloWorldWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repositories;
using System.Linq;

namespace HelloWorldWeb.Controllers
{
   
    public class MessageDeliveryController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserInfoRepository _userInfoRepository;

        public MessageDeliveryController(ILogger<HomeController> logger, IMapper mapper, IUserInfoRepository userInfoRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfoRepository = userInfoRepository;
        }

        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = "CanViewDeliveryInfo")]
        public IActionResult GetMessageDeliveryInfo()
        {
            _logger.LogInformation("GetMessageDeliveryInfo fired");
            var fullName = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var claims = _userInfoRepository.RetrieveUserInfoClaims();
            var address = claims.FirstOrDefault(c => c.Type == "address")?.Value;
            var phone = claims
                .FirstOrDefault(c => c.Type == "phone_number")?.Value;
            var email = claims
                .FirstOrDefault(c => c.Type == "email")?.Value;
            AddressDTO addressDto = null;
            if (address != null)
                addressDto = JsonConvert.DeserializeObject<AddressDTO>(address);
            return View("MessageDeliveryInfo", new MessageDeliveryInfoVM(fullName, _mapper.Map<AddressVM>(addressDto), phone, email));
        }
    }
}