﻿using Microsoft.AspNetCore.Mvc;

namespace CitiesManager.WebAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")] 
    [ApiController]
    public class CustomControllerBase : ControllerBase //CustomControllerBase will be parent class to rest of WebApi Controllers, which don't require use of [Route()] & [ApiController] attributes any
    {
    }
}