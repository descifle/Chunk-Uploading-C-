using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChunkUpload.Controllers
{
    public class FileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.OperationId == "ApiFileUploadUserByUserIdSessionBySessionIdPut")
            {
                var p = operation.Parameters.Where(op => op.Schema.Type == "formData").ToList();
                p.ForEach(item => operation.Parameters.Remove(item));

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "files",
                    In = ParameterLocation.Header,
                    Description = "Upload File",
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "file"
                    }
                });

                var apiResponse = new OpenApiResponse
                {
                    Description = "Ok",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType()
                    }
                };

                operation.Responses.Add("200", apiResponse);
            }
        }
    }
}