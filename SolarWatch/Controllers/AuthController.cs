using Microsoft.AspNetCore.Mvc;
using SolarWatch.Contracts;
using SolarWatch.Service.Authentication;

namespace SolarWatch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly string _defaultRole = "User";
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.RegisterAsync(request.Email, request.Username, request.Password, _defaultRole);
            if (!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.LoginAsync(request.Email, request.Password);
            if(!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new AuthenticationResponse(result.Email, result.UserName, result.Token));
        }

        [HttpPost("CheckTokenIfValid")]
        public async Task<ActionResult<AuthenticationResponse>> ValidateToken([FromBody]string tokenAsString)
        {
            var result = await _authenticationService.CheckTokenValidityAsync(tokenAsString);
            if (!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new AuthenticationResponse(result.Email, result.UserName, result.Token));
        }

        private void AddErrors(AuthenticationResult result)
        {
            foreach(var error in result.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }
    }
}
