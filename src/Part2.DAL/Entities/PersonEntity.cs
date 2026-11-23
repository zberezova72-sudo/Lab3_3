using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Lab33.DAL.Entities;

[Serializable]
[XmlInclude(typeof(StudentEntity))]
[XmlInclude(typeof(MusicianEntity))]
[XmlInclude(typeof(PilotEntity))]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StudentEntity), "student")]
[JsonDerivedType(typeof(MusicianEntity), "musician")]
[JsonDerivedType(typeof(PilotEntity), "pilot")]
public abstract class PersonEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
