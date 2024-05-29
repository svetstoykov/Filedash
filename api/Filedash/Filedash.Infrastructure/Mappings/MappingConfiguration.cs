using AutoMapper;
using Filedash.Domain.Models;

namespace Filedash.Infrastructure.Mappings;

public class MappingConfiguration : Profile
{
    public MappingConfiguration()
    {
        CreateMap<UploadedFile, UploadedFileDetails>()
            .ConstructUsing(c =>
                new UploadedFileDetails(
                    c.Id, $"{c.Name}{c.Extension}", c.ContentLength, c.CreatedDateUtc, c.EncodingType));
    }
}