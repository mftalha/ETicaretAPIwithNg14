using ETicaretAPI.Application.Enums;
using System;

namespace ETicaretAPI.Application.CustomAttributes;

// yetkilendirmeye tabi tutulacak actionları tanımlıyacam
// controller veya action üstüdne tanımladığım [] => tagları.
// çağırıken => AuthorizeDefinition() bu şekilde çağırıyoruz : Attribute kısmını yazmasakda sorun yok : genel tanıda yazılmıyorda 
public class AuthorizeDefinitionAttribute : Attribute
{
    /// işaretlenen actionun hangi menüye ait olduğu
    public string Menu { get; set; }
    // tanımını tutuyoruz
    public string Definition { get; set; }
    //hangi actiona karşılık geliyor = reading, mi writing mi?
    public ActionType ActionType { get; set; }
}
