using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RepairIT.Client;

// Front Blazor WebAssembly de RepairIT (port 5000).
// En V1 il ne fournit que le squelette de navigation ; la consommation des APIs arrive en V4.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();
