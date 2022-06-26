using Osprey;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var bindings = new List<Binding>();
config.GetSection("Bindings").Bind(bindings);

// todo: many bindings
var firstBinding = bindings.FirstOrDefault();
Server.Start(firstBinding, firstBinding.Routes);