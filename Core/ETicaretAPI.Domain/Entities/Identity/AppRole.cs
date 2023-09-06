﻿using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Domain.Entities.Identity;

public class AppRole : IdentityRole<string>
{
	public ICollection<Endpoint> Endpoints { get; set; }
}
