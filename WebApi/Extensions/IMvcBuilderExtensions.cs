using Entities.Utilities.Formatters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApi.Extensions
{
    public static class IMvcBuilderExtensions
    {
        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder)=>
            builder.AddMvcOptions(config=>
            config.OutputFormatters
            .Add(new CsvOutPutFormatter()));
    }
}
