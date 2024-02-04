using DocumentManagementStore.Common;
using DocumentManagementStore.Common.Core.Events;
using DocumentManagementStore.Options;
using DocumentManagementStore.Projections;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Marten.PLv8;
using Marten.Services.Json;
using Microsoft.Extensions.Options;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
            .AddMarten(
                (sp) =>
                {
                    var domainEvents = typeof(IDomainEvent).Assembly
                        .GetTypes()
                        .Where(p => typeof(IDomainEvent).IsAssignableFrom(p))
                        .ToList();
                    var o = new StoreOptions();
                    var databaseConnection = sp.GetService<IOptions<DatabaseConnection>>();
                    o.Connection(databaseConnection!.Value!.Connection!);
                    o.UseJavascriptTransformsAndPatching();
                    o.UseDefaultSerialization(
                        serializerType: SerializerType.SystemTextJson,
                        enumStorage: EnumStorage.AsString,
                        casing: Casing.CamelCase
                    );
                    o.DatabaseSchemaName = "documents";
                    o.Events.DatabaseSchemaName = "documents";
                    o.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                    o.Events.StreamIdentity = StreamIdentity.AsString;
                    o.Events.AddEventTypes(domainEvents);
                    o.Projections.Add<FolderReadModelBuilder>(ProjectionLifecycle.Async);
                    o.Projections.Add<DocumentReadModelBuilder>(ProjectionLifecycle.Async);
                    o.Projections.Add<UserReadModelBuilder>(ProjectionLifecycle.Async);
                    
                    o.Projections.AddDefaultProjectionExceptionHandling(
                        sp.GetRequiredService<ILogger<Program>>()
                    );
                    o.GeneratedCodeMode = JasperFx.CodeGeneration.TypeLoadMode.Auto;
                    o.SourceCodeWritingEnabled = true;
                    return o;
                }
            )
            .UseLightweightSessions()
            .AddAsyncDaemon(Marten.Events.Daemon.Resiliency.DaemonMode.HotCold);

var app = builder.Build();

app.LoadFeatures();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
