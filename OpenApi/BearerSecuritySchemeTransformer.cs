using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace api.OpenApi;

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    private const string SchemaId = JwtBearerDefaults.AuthenticationScheme;

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemaId] = new OpenApiSecurityScheme()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token",
            Name = "Authorization",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = SchemaId
            }
        };
        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (!context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
            return Task.CompletedTask;
        operation.Security ??= [];
        var key = new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = SchemaId
            }
        };
        var requirement = new OpenApiSecurityRequirement()
        {
            { key, [] }
        };
        operation.Security.Add(requirement);
        return Task.CompletedTask;
    }
}