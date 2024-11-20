using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using ODataAPI.Filter;
using ODataAPI.Models;
using ODataAPI.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var odataSettings = builder.Configuration.GetSection("ODataSettings").Get<ODataSettings>()!;

builder.Services.AddDbContext<AdventureWorksLT2019Context>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton(odataSettings);
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddControllers()
    .AddOData(options => options.Select().Filter().OrderBy().Expand().Count());
builder.Services
    .AddControllers()
   .AddNewtonsoftJson(options =>
   {
       options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
       options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
       options.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
       options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
       options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
       options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
   })
   .AddODataNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    c.OperationFilter<ODATAParamsFilter>();
});

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();